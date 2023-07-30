namespace AD.Base;

public class Decompiler
{
    public string ParseLine(ushort instruction)
    {
        switch (InstructionParser.GetOpCode(instruction))
        {
            case 0x0:
                if (instruction == 0x00E0)
                {
                    return "CLS";
                }
                if (instruction == 0x00EE)
                {
                    return "RET";
                }
                return "SYS " + Convert.ToString(InstructionParser.GetAddress(instruction), toBase: 16);
            case 1:
                return "JP " + Convert.ToString(InstructionParser.GetAddress(instruction), toBase: 16);
            case 0xB:
                return "JP V0, " + Convert.ToString(InstructionParser.GetAddress(instruction), toBase: 16);
            case 2:
                return "CALL " + Convert.ToString(InstructionParser.GetAddress(instruction), toBase: 16);
            case 3:
                return "SE V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", #"+Convert.ToString(InstructionParser.GetValue(instruction), toBase: 16);
            case 5:
                return "SE V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 4:
                return "SNE V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", #"+Convert.ToString(InstructionParser.GetValue(instruction), toBase: 16);
            case 9:
                return "SNE V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 6:
                return "LD V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", #"+Convert.ToString(InstructionParser.GetValue(instruction), toBase: 16);
            case 8:
                return DecodeEighthInstruction(instruction);
            case 0xF:
                return DecodeFthInstruction(instruction);
            case 0xA:
                return "LD I, " + Convert.ToString(InstructionParser.GetAddress(instruction), toBase: 16);
            case 0x7:
                return "ADD V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", #"+Convert.ToString(InstructionParser.GetValue(instruction), toBase: 16);
            case 0xC:
                return "RND V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", #"+Convert.ToString(InstructionParser.GetValue(instruction), toBase: 16);
            case 0xD:
                return "RND V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16)+", #"+Convert.ToString(InstructionParser.GetSubArg(instruction), toBase: 16);
            case 0xE:
                if (InstructionParser.GetValue(instruction) == 0x9E)
                {
                    return "SKP V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
                }
                return "SKNP V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
            
            
        }

        return "";
    }

    private string DecodeEighthInstruction(ushort instruction)
    {
        byte type = InstructionParser.GetSubArg(instruction);
        switch (type)
        {
            case 0:
                return "LD V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 1:
                return "OR V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 2:
                return "AND V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 3:
                return "XOR V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 4:
                return "ADD V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 5:
                return "SUB V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 6:
                return "SHR V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 7:
                return "SUBN V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
            case 0xE:
                return "SHL V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+ ", V"+Convert.ToString(InstructionParser.GetY(instruction), toBase: 16);
        }
        return "";
    }
    private string DecodeFthInstruction(ushort instruction)
    {
        byte type = InstructionParser.GetValue(instruction);
        switch (type)
        {
            case 0x07:
                return "LD V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16) + ", DT";
            case 0x0A:
                return "LD V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16) + ", K";
            case 0x15:
                return "LD DT, V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
            case 0x18:
                return "LD ST, V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
            case 0x29:
                return "LD F, V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
            case 0x33:
                return "LD B, V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
            case 0x55:
                return "LD [I], V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
            case 0x65:
                return "LD V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16)+", [I]";
            case 0x1E:
                return "ADD I, V" + Convert.ToString(InstructionParser.GetX(instruction), toBase: 16);
        }
        return "";
    }
}

sealed class InstructionParser
{
    public static byte GetOpCode(ushort instruction)
    {
        return (byte) (instruction >> 12);
    }
    
    public static byte GetValue(ushort instruction)
    {
        return (byte) (instruction & 0xff);
    }
    
    public static ushort GetAddress(ushort instruction)
    {
        return (ushort) (instruction & 0xfff);
    }
    
    public static byte GetY(ushort instruction)
    {
        return (byte)(instruction >> 4 & 0x0f);
    }
    
    public static byte GetX(ushort instruction)
    {
        return (byte)(instruction >> 8 & 0x0f);
    }
    
    public static byte GetSubArg(ushort instruction)
    {
        return (byte) (instruction & 0x0f);
    }
}