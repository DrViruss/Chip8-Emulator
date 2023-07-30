using System.Text;
using AD.Base;

namespace AD;

abstract class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string path;
            path = args.Length < 2 ? "code.asm" : args[1];

            if (File.Exists(path))
            {
                if (args[0] == "a")
                {
                    Console.WriteLine("Assembling... "+path);
                    Assemble(path);
                    return;
                }
                if (args[0] == "d")
                {
                    Console.WriteLine("Disassembling... "+path);
                    Disassemble(path);
                    return;
                }
                Help();
                return;
            }
            Console.WriteLine("File '"+path+"' does not exist!");
            return;

        }
        Help();
    }

    private static void Help()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Using:");
        sb.AppendLine("AD.exe a code.asm \t - \t Assemble code from file to .ch8 file.");
        sb.AppendLine("AD.exe d code.ch8 \t - \t Disassemble .ch8 to .asm file with code.");
        
        Console.WriteLine(sb);
    }

    private static void Assemble(string path)
    {
        byte[] result = new byte[3584];
        StreamReader sr = File.OpenText(path);
        StringReader strr = new StringReader(sr.ReadToEnd());
        sr.Close();
        int lineNumber = 1;
        int opPose = 0;
        string? codeLine = strr.ReadLine();
        int opCode;
        Compiler compiler = new Compiler();
        while (!string.IsNullOrWhiteSpace(codeLine))
        {
            if (compiler.ParseLine(codeLine.Trim(),lineNumber,out opCode))
            {
                if (opCode != 0)
                {
                    Console.WriteLine(lineNumber+": "+codeLine+" -> "+Convert.ToString(opCode, toBase: 16).ToUpper());
                    result[opPose++] = (byte)(opCode >> 8);
                    result[opPose++] = (byte) (opCode & 0xff);
                }
            }
            codeLine = strr.ReadLine();
            lineNumber++;
        }

        Console.WriteLine("EOF");
        strr.Close();
        
        string outPath = path.Replace(".asm",".ch8");
        Console.WriteLine("Writing to "+outPath+"...");
        FileStream fs = File.OpenWrite(outPath);
        byte[] program = result.Take(opPose).ToArray();
        fs.Write(program,0,program.Length);
        fs.Close();
        Console.WriteLine("Done!");
    }

    private static void Disassemble(string path)
    {
        StringBuilder sb = new StringBuilder();
        FileStream fs = File.OpenRead(path);
        byte[] program = new byte[3584];
        fs.Read(program, 0, 3584);
        fs.Close();
        int instructionPos = 0;
        ushort instruction = GetInstruction(instructionPos,program);
        Decompiler decompiler = new Decompiler();
        while (instruction > 0)
        {
            string codeLine = decompiler.ParseLine(instruction).ToUpper();
            Console.WriteLine((Convert.ToString(instruction, toBase: 16)+" -> "+codeLine).ToUpper());
            sb.AppendLine(codeLine);
            instructionPos += 2;
            
            instruction = GetInstruction(instructionPos,program);
        }
        
        string outPath = path.Replace(".ch8",".asm");
        Console.WriteLine("Writing to "+outPath+"...");
        StreamWriter sw = new StreamWriter(outPath);
        sw.WriteLine(sb);
        sw.Close();
        Console.WriteLine("Done!");
    }

    private static ushort GetInstruction(int index,byte[] program)
    { // ex: mem[0] = 0x80 && mem[1] = 0x20 -> 0x8020 -> LD V0, V2 
        byte operationHi = program[index++];
        byte operationLo = program[index++];
        var _tmp =  operationHi << 8;
        _tmp |= operationLo;
        return (ushort) _tmp;
    }
}
