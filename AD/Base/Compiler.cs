using System.Globalization;

namespace AD.Base;

public class Compiler
{
    public bool ParseLine(string line,int lineNumber, out int opCode)
    {
        string[]? code = line.ToUpper().Split(' ');
        for (int i=0; i< code.Length; i++)
        {
            string cLine = code[i];
            if (cLine.Contains(','))
            {
                code[i] = cLine.Remove(cLine.LastIndexOf(','));
            }
        }
        
        switch (code[0])
        {
            case "RET":
                opCode = 0x00EE;
                break;
            case "SYS":
                opCode = 0x0000 | ParseAddr(code[1],lineNumber);
                break;
            case "JP":
                opCode = ParseJP(code,lineNumber);
                break;
            case "CALL":
                opCode = 0x2000 | ParseAddr(code[1],lineNumber);
                break;
            case "SE":
                opCode = ParseSE(code,lineNumber);
                break;
            case "SNE":
                opCode = ParseSNE(code,lineNumber);
                break;
            case "LD":
                opCode = ParseLD(code,lineNumber);
                break;
            case "ADD":
                opCode = ParseADD(code,lineNumber);
                break;
            
            case "OR":
                opCode = 0x8001 | (ParseRegistres(code[1], lineNumber) << 8) | (ParseRegistres(code[2], lineNumber) << 4);
                break;
            case "AND":
                opCode = 0x8002 | (ParseRegistres(code[1], lineNumber) << 8) | (ParseRegistres(code[2], lineNumber) << 4);
                break;
            case "XOR":
                opCode = 0x8003 | (ParseRegistres(code[1], lineNumber) << 8) | (ParseRegistres(code[2], lineNumber) << 4);
                break;
            case "SUB":
                opCode = 0x8005 | (ParseRegistres(code[1], lineNumber) << 8) | (ParseRegistres(code[2], lineNumber) << 4);
                break;
            case "SHR":
                opCode = 0x8006 | (ParseRegistres(code[1], lineNumber) << 8) | (ParseRegistres(code[2], lineNumber) << 4);
                break;
            case "SUBN":
                opCode = 0x8007 | (ParseRegistres(code[1], lineNumber) << 8) | (ParseRegistres(code[2], lineNumber) << 4);
                break;
            case "SHL":
                opCode = 0x800E | (ParseRegistres(code[1], lineNumber) << 8) | (ParseRegistres(code[2], lineNumber) << 4);
                break;
            case "RND":
                opCode = 0xC000 | (ParseRegistres(code[1], lineNumber) << 8) | (ParseValue(code[2], lineNumber));
                break;
            case "DRW":
                opCode = 0xD000 | (ParseRegistres(code[1], lineNumber) << 8) | ParseRegistres(code[2], lineNumber) << 4 | 0x0f & Convert.ToInt16(code[3], 16);
                break;
            case "SKP":
                opCode = 0xE09E | ParseRegistres(code[1], lineNumber) << 8;
                break;
            case "SKNP":
                opCode = 0xE0A1 | ParseRegistres(code[1], lineNumber) << 8;
                break;
            
            
            default: // CLS
                opCode = 0x00E0;
                break;
        }

        return true;
    }
    
#region ParseInstructions

    private int ParseJP(string[] code,int lineNumber)
    {
        int opCode;
        if (code.Length == 2) // JP addr
        {
            opCode = 0x1000 | ParseAddr(code[1],lineNumber);
        }
        else if (code.Length == 3) // JP V0 addr
        {
            opCode = 0xB000;
            if(!code[1].Contains("V0"))
                throw new Exception("Error in line {0}. See docs of JP instruction.");
            opCode |= ParseAddr(code[2],lineNumber);
        }
        else
        {
            throw new Exception("Error in line #"+lineNumber+". Unknown JP instruction.");
        }
        
        return opCode;
    }

    private int ParseSE(string[] code, int lineNumber)
    {
        int opCode;
        if (code[2].Contains('V')) //SE Vx Vy
        {
            byte x = ParseRegistres(code[1], lineNumber);
            byte y = ParseRegistres(code[2], lineNumber);
            opCode = 0x5000 | x << 8 | y << 4;
        }
        else // SE Vx byte
        {
            byte x = ParseRegistres(code[1], lineNumber);
            byte b = ParseValue(code[2], lineNumber);
            opCode = 0x3000 | x << 8 | 0xff & b;
        }

        return opCode;
    }
    
    private int ParseSNE(string[] code, int lineNumber)
    {
        int opCode;
        if (code[1].Contains('V'))
        {
            if (code[2].Contains('V')) // SNE Vx Vy
            {
                byte x = ParseRegistres(code[1], lineNumber);
                byte y = ParseRegistres(code[2], lineNumber);
                opCode = 0x9000 | x << 8 | y << 4;
            }
            else // SNE Vx byte
            {
                byte x = ParseRegistres(code[1], lineNumber);
                byte b = ParseValue(code[2], lineNumber);
                opCode = 0x4000 | x << 8 | 0xff & b;
            }
        }
        else
        {
            throw new Exception("Error in line #" + lineNumber + ". Unknown SNE instruction.");
        }
        

        return opCode;
    }
    
    private int ParseLD(string[] code, int lineNumber)
    {
        int opCode = 0;
        if (code[1].Contains('V'))
        {
            if (code[2].Contains('#')) //LD Vx byte
            {
                byte x = ParseRegistres(code[1], lineNumber);
                byte b = ParseValue(code[2], lineNumber);
                opCode = 0x6000 | x << 8 | 0xff & b;
            }
            if (code[2].Contains('V')) //LD Vx Vy
            {
                byte x = ParseRegistres(code[1], lineNumber);
                byte y = ParseRegistres(code[2], lineNumber);
                opCode = 0x8000 | x << 8 | y << 4;
            }
            if (code[2] == "DT") // LD Vx DT
            {
                byte x = ParseRegistres(code[1], lineNumber);
                opCode = 0xF007 | x << 8;
            }
            if (code[2] == "K") // LD Vx K
            {
                byte x = ParseRegistres(code[1], lineNumber);
                opCode = 0xF00A | x << 8;
            }
            if (code[2] == "[I]")// LD Vx [I]
            {
                byte x = ParseRegistres(code[1], lineNumber);
                opCode = 0xF065 | x << 8;
            }
        }
        else if (code[2].Contains('V'))
        {
            if (code[1] == "DT") // LD DT VX
            {
                byte x = ParseRegistres(code[2], lineNumber);
                opCode = 0xF015 | x << 8;
            }
            if (code[1] == "ST")// LD ST Vx
            {
                byte x = ParseRegistres(code[2], lineNumber);
                opCode = 0xF018 | x << 8;
            }
            if (code[1] == "F" || code[2] == "I" )// LD F Vx (LD I Vx)
            {
                byte x = ParseRegistres(code[2], lineNumber);
                opCode = 0xF029 | x << 8;
            }
            if (code[1] == "B")// LD B Vx
            {
                byte x = ParseRegistres(code[2], lineNumber);
                opCode = 0xF033 | x << 8;
            }
            if (code[1] == "[I]")// LD [I] Vx
            {
                byte x = ParseRegistres(code[2], lineNumber);
                opCode = 0xF055 | x << 8;
            }
        }
        else if (code[1] == "I") // LD I addr
        {
            ushort addr = ParseAddr(code[2], lineNumber);
            opCode = 0xA000 | addr;
        }
        else 
        {
            throw new Exception("Error in line #"+lineNumber+". Unknown LD instruction.");
        }

        return opCode;
    }
    
    private int ParseADD(string[] code, int lineNumber)
    {
        int opCode;
        if (code[1].Contains('V'))
        {
            if (code[2].Contains('V')) // ADD Vx Vy
            {
                byte x = ParseRegistres(code[1], lineNumber);
                byte y = ParseRegistres(code[2], lineNumber);
                opCode = 0x8004 | x << 8 | y << 4;
            }
            else // ADD Vx byte
            {
                byte x = ParseRegistres(code[1], lineNumber);
                byte b = ParseValue(code[2], lineNumber);
                opCode = 0x7000 | x << 8 | 0xff & b;
            }
        }
        else if (code[1].Contains('I')) // ADD I Vx
        {
            byte x = ParseRegistres(code[2], lineNumber);
            opCode = 0xF01E | x << 8;
        }
        else
        {
            throw new Exception("Error in line #"+lineNumber+". Unknown ADD instruction.");
        }

        return opCode;
    }

#endregion


    #region ParseValues

    private ushort ParseAddr(string code,int lineNumber)
    {
        
        if (!ushort.TryParse(code,NumberStyles.HexNumber,null, out var result))
            throw new Exception("Value parsing failed!\n Error in addr parsing. Line:"+lineNumber);
        return result;
    }

    private byte ParseRegistres(string code,int lineNumber)
    {
        if (!code.Contains('V'))
            Console.WriteLine("Using Register number without 'V' is not recommended!");
        else
            code = code.Remove(0, 1);

        if(!byte.TryParse(code,NumberStyles.HexNumber,null,out var register))
            throw new Exception("Value parsing failed!\nError in register parsing. Line:"+lineNumber );
        return register;
    }

    private byte ParseValue(string code, int lineNumber)
    {
        if (!code.Contains('#'))
            Console.WriteLine("Using byte value without '#' is not recommended!");
        else
            code = code.Remove(0, 1);

        
        if(!byte.TryParse(code,NumberStyles.HexNumber,null,out var value))
            throw new Exception("Value parsing failed!\nError in byte value parsing. Line:"+lineNumber );
        return value;
    }


    #endregion
}