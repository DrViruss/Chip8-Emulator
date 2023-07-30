using System.Diagnostics;
using System.Text;
using Base_Lib;

namespace Console_Emulator;

abstract class Program
{
    static void Main(string[] args)
    {
        bool debugMode = false;
        bool customROM = false;
        string rompath = "";
        if (args.Length > 0)
        {
            if (args[0] == "-debug")
            {
                debugMode = true;
                if (args.Length > 1)
                {
                    customROM = checkFile(args[1],true);
                    if (customROM)
                    {
                        rompath = args[1];
                        Console.WriteLine("Found custom ROM");
                    }
                }
                    
            }
            else
            {
                customROM = checkFile(args[0],true);
                if (customROM)
                {
                    rompath = args[0];
                    Console.WriteLine("Found custom ROM");
                }
            }
        }
        FileStream program = GetPathFromInput();
        FileStream? rom = customROM ? File.OpenRead(rompath) : null;
        
        Emulator chipEmu = new Emulator();
        chipEmu.Start(debugMode,rom,program);
    }

    private static FileStream GetPathFromInput()
    {
        while (true)
        {
            Console.WriteLine("Input a path to program:");
            string? path = Console.ReadLine();

            if (checkFile(path,false))
                return File.OpenRead(path);
        }
    }
    private static bool checkFile(string? path,bool isROM)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Error.\nFile does not exist.\nPress any key to try again...\n");
            Console.ReadKey();
            Console.Clear();
            return false;
        }

        if (!isROM)
        {
            if (!path.Contains(".ch8"))
            {
                Console.WriteLine("Error.\nFile must to have .ch8 extension.\nPress any key to try again...\n");
                Console.ReadKey();
                Console.Clear();
                return false;
            }
        }
        return true;
    }
    
    private class Emulator
    {
        private CPU _cpu;
        private Memory _memory;
        // private Dissassembler _dissassembler;

        // private bool _debug;
        // private Disassembler _diser;
        
        public void Start(bool debug, FileStream? fsRom, FileStream fsProgram)
        {
            Init(debug);
            
            //ROM
            if (fsRom != null)
            {
                byte[] rom = new byte[512];
                fsRom.Read(rom, 0, 512);
                fsRom.Close();
                _memory.LoadROM(rom);
            }
            else
                _memory.InitializeInterpreterBuffer();
            
            //Program
            byte[] program = new byte[3584]; // 4096 - 512 = 3584
            fsProgram.Read(program, 0, 3584);
            _memory.LoadProgram(program);
            
            // Clock
            Stopwatch watch = new Stopwatch();
            var cpuSpeed = 6 * Stopwatch.Frequency / 1000;
            while (true)
            {
                if(!watch.IsRunning || watch.ElapsedTicks > cpuSpeed)
                {
                    if (debug)
                        Console.ReadKey();
                    if (_cpu.Tick())
                    {
                        watch.Restart();
                    }
                    else
                        break;
                }
            }
            Console.WriteLine("Program execution finalized.\nPress any key to exit.");
            Console.ReadKey();
        }

        private void Init(bool debug)
        {
            Console.Clear();
            
            Display display = new Display();
            Keyboard keyboard = new Keyboard();
            _memory = new Memory();
            if (debug)
            {
                _cpu = new CPU(_memory,display,keyboard,DebugFunc);
            }
            else
            {
                _cpu = new CPU(_memory,display,keyboard);
            }
        }

        private void DebugFunc(StringBuilder sb)
        {
            Console.SetCursorPosition(0, 32);
            Console.WriteLine(sb);
        }
    }
}

