using System.Text;

namespace Base_Lib;

public delegate void DebugFunc(StringBuilder inString);

delegate void Instruction(ushort instruction);

public class CPU
{
    private const byte BYTE_SIZE = 8;
    private const byte SPRITE_H = 5;
    
    private Memory _memory;
    private IDisplay _display;
    private IKeyboard _keyboard;

    private ushort InstructionPointer = 0x200;
    private ushort IndexRegister; //I (F)

    private byte[] Register = new byte[16]; //from V0 to VF

    private Timer _delayTimer;
    private Timer _soundTimer;

    public Stack<ushort> Stack = new(16);

    private Dictionary<byte, Instruction> InstructionSet = new Dictionary<byte, Instruction>();
    // private Dictionary<byte, Action<byte, byte>> RegisterCommands; //??

    private Func<byte> RandomGenerator;
    
    //Debug
    private readonly bool _debug;
    private readonly DebugFunc? _debugFunc;

    public CPU(Memory memory, IDisplay display, IKeyboard keyboard)
    {
        _memory = memory;
        _display = display;
        _keyboard = keyboard;

        _delayTimer = new Timer();
        _soundTimer = new Timer();

        RandomGenerator = RandomFunc;

        InstructionSet[0] = SYS;
        InstructionSet[1] = JP;
        InstructionSet[2] = CALL;
        InstructionSet[3] = SEB;
        InstructionSet[4] = SNEB;
        InstructionSet[5] = SE;
        InstructionSet[6] = LDB;
        InstructionSet[7] = ADDB;
        InstructionSet[8] = EthInstructions;
        InstructionSet[9] = SNE;
        InstructionSet[0xA] = LDI;
        InstructionSet[0xB] = JPV;
        InstructionSet[0xC] = RND;
        InstructionSet[0xD] = DRW;
        InstructionSet[0xE] = EInstructions;
        InstructionSet[0xF] = FInstructions;

    }
    public CPU(Memory memory, IDisplay display, IKeyboard keyboard,DebugFunc debug) : this(memory,display,keyboard)
    {
        _debug = true;
        _debugFunc = debug;
    }

    public bool Tick()
    {
        _soundTimer.Tick();
        _delayTimer.Tick();

        lock (this)
        {
            if (InstructionPointer > 4095)
                return false;
            ushort instruction = GetInstructionFromMem();

            byte opcode = InstructionParser.GetOpCode(instruction);
            // InstructionSet[opcode](instruction);
            Instruction ins = InstructionSet[opcode];
            ins(instruction);
            
            _display.Print();

            if (_debug)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Instruction Pointer:"+DecToHex(InstructionPointer));
                sb.AppendLine("Index Register:"+DecToHex(IndexRegister));
                sb.AppendLine("DT:"+DecToHex(_delayTimer.time));
                sb.AppendLine("ST:"+DecToHex(_soundTimer.time));
                sb.AppendLine("---------------------");
                sb.AppendLine("Registers:");
                for (int i = 0; i < 0xF; i+=2)
                {
                    sb.AppendLine("V"+DecToHex(i)+":"+Register[i]+
                            "\t V"+DecToHex(i+1)+":"+Register[i+1]);
                }
                sb.AppendLine("---------------------");
                sb.AppendLine("Stack:");
                foreach (var val in Stack)
                {
                    sb.AppendLine(DecToHex(val));
                }
                _debugFunc(sb);
            }
            return true;
        }
    }
    
    private ushort GetInstructionFromMem()
    { // ex: mem[0] = 0x80 && mem[1] = 0x20 -> 0x8020 -> LD V0, V2 
        byte operation_hi = _memory[InstructionPointer++];
        byte operation_lo = _memory[InstructionPointer++];
        var _tmp =  operation_hi << 8;
        _tmp |= operation_lo;
        return (ushort) _tmp;
    }
    
    private byte RandomFunc()
    {
        return (byte)new Random().Next(256); // [0-256)
    }

    #region Instructions

    private void SYS(ushort instruction) //SYS instructions
    {
        ushort address = InstructionParser.GetAddress(instruction);
        if (address == 0x00e0)
        {
            CLS();
            return;
        }
        if (address == 0x00eE)
        {
            RET();
            return;
        }

        if (address == 0x0f0)
        {
            Console.WriteLine("");
        }
    }
    private void CLS()
    {
        Console.Clear();
    }
    private void RET()
    {
        if (Stack.Count < 1)
        {
            return;
        }
        InstructionPointer = Stack.Pop();
    }
    private void JP(ushort instruction) // JP addr
    {
        ushort addr = InstructionParser.GetAddress(instruction);
        InstructionPointer = addr;
    }
    private void JPV(ushort instruction) //JP V0, addr
    {
        ushort addr = InstructionParser.GetAddress(instruction);
        InstructionPointer = (ushort) (addr + Register[0]);
    }
    private void CALL(ushort instruction) //CALL addr
    {
        ushort addr = InstructionParser.GetAddress(instruction);
        Stack.Push(InstructionPointer);
        InstructionPointer = addr;
    }
    private void SEB(ushort instruction) //SE Vx, byte
    {
        byte x = InstructionParser.GetX(instruction);
        byte b = InstructionParser.GetValue(instruction);
        if (Register[x] == b)
            InstructionPointer += 2;
    }
    private void SE(ushort instruction) //SE Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        if (Register[x] == Register[y])
            InstructionPointer += 2;
    }
    private void SNEB(ushort instruction) //SNE Vx, byte
    {
        byte x = InstructionParser.GetX(instruction);
        byte b = InstructionParser.GetValue(instruction);
        if (Register[x] != b)
            InstructionPointer += 2;
    }
    private void SNE(ushort instruction) //SNE Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        if (Register[x] != Register[y])
            InstructionPointer += 2;
    }
    private void LDB(ushort instruction) //LD Vx, byte
    {
        byte x = InstructionParser.GetX(instruction);
        byte b = InstructionParser.GetValue(instruction);

        Register[x] = b;
    }
    private void LDV(ushort instruction) //LD Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);

        Register[x] = Register[y];
    }
    private void LDI(ushort instruction) //LD I, addr
    {
        ushort addr = InstructionParser.GetAddress(instruction);
        IndexRegister = addr;
    }
    private void LD(ushort instruction)
    {
        byte type = InstructionParser.GetValue(instruction);
        byte x = InstructionParser.GetX(instruction);
        switch (type)
        {
            case 0x07: // LD Vx, DT
            {
                Register[x] = _delayTimer.time;
                break;
            }
            case 0x0A: // LD Vx, K
            {
                byte key = _keyboard.WaitForKey();
                Register[x] = key;
                break;
            }
            case 0x15: // LD DT, Vx
            {
                _delayTimer.time = Register[x];
                break;
            }
            case 0x18: // LD ST, Vx
            {
                _soundTimer.time = Register[x];
                break;
            }
            case 0x29: // LD F, Vx (LD I, Vx)
            {
                IndexRegister = (ushort) (Register[x] * SPRITE_H);
                break;
            }
            case 0x33: // LD B, Vx
            {
                int tmp = Register[x];

                for (int i = 2; i >= 0; i--)
                {
                    var digit = tmp % 10;
                    _memory[IndexRegister + i] = (byte) digit;
                    tmp /= 10;
                }
                break;
            }
            case 0x55: // LD [I], Vx
            {
                for (int i = 0; i <= x; i++)
                {
                    _memory[IndexRegister+i] = Register[i];
                }
                break;
            }
            case 0x65: // LD Vx, [I]
            {
                for (int i = 0; i <= x; i++)
                {
                    Register[i] = _memory[IndexRegister+i];
                }
                break;
            }
        }
    }
    private void ADDB(ushort instruction) //ADD Vx, byte
    {
        byte x = InstructionParser.GetX(instruction);
        byte b = InstructionParser.GetValue(instruction);
        Register[x] += b;
    }
    private void ADD(ushort instruction) //ADD Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        Register[x] += y;
    }
    private void ADDI(ushort instruction) //ADD I, Vx
    {
        byte x = InstructionParser.GetX(instruction);
        IndexRegister += x;
    }
    private void OR(ushort instruction) //OR Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        Register[x] =(byte) (Register[x] | Register[y]);
    }
    private void AND(ushort instruction) //AND Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        Register[x] =(byte) (Register[x] & Register[y]);
    }
    private void XOR(ushort instruction) //XOR Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        Register[x] =(byte) (Register[x] ^ Register[y]);
    }
    private void SUB(ushort instruction) //SUB Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        if (Register[x] > Register[y])
            Register[0xF] = 1;
        else
            Register[0xF] = 0;
        
        Register[x] -= Register[y];
    }
    private void SHR(ushort instruction) //SHR Vx {, Vy} ( Vx >> Vy if Vy exist)
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        byte shr = 1;
        if (y != 0)
        {
            shr = Register[y];
        }
        byte tmp = Register[x];
        Register[0xF] = (byte) (tmp & 0x0001);
        Register[x] = (byte) (tmp >> shr);

    }
    private void SUBN(ushort instruction) //XSUBN Vx, Vy
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        if (Register[y] > Register[x])
            Register[0xF] = 1;
        else
            Register[0xF] = 0;
        
        Register[x] =(byte)(Register[y] - Register[x]);
    }
    private void SHL(ushort instruction) //SHL Vx {, Vy}
    {
        byte x = InstructionParser.GetX(instruction);
        byte y = InstructionParser.GetY(instruction);
        byte shr = 1;
        if (y != 0)
        {
            shr = Register[y];
        }
        byte tmp = Register[x];
        Register[0xF] = (byte) (tmp & 0x1000);
        Register[x] = (byte) (tmp << shr);
    }
    private void RND(ushort instruction) //RND Vx, byte
    {
        byte x = InstructionParser.GetX(instruction);
        byte b = InstructionParser.GetValue(instruction);
        Register[x] = (byte)(RandomGenerator() & b);
    }
    private void DRW(ushort instruction) //DRW Vx, Vy, nibble
    {
        byte Vx = InstructionParser.GetX(instruction);
        byte Vy = InstructionParser.GetY(instruction);
        byte n = InstructionParser.GetSubArg(instruction);

        byte x_pos = Register[Vx];
        byte y_pos = Register[Vy];
        Register[0xF] = 0;

        for (int y = 0; y < n; y++)
        {
            if (SetValueOnScreen(x_pos, (y_pos+y) % 32, _memory[IndexRegister + y]))
                Register[0xF] = 1;
        }

    }
    private void SKP(ushort instruction) //SKP Vx
    {
        byte x = InstructionParser.GetX(instruction);
        byte key = _keyboard.WaitForKey();
        if (Register[x] == key)
            InstructionPointer += 2;
    }
    private void SKNP(ushort instruction) //SKNP Vx
    {
        byte x = InstructionParser.GetX(instruction);
        byte key = _keyboard.WaitForKey();
        if (Register[x]!= key)
            InstructionPointer += 2;
    }
    #endregion

    #region ConditionalInstructions
    private void EthInstructions(ushort instruction)
    {
        byte type = InstructionParser.GetSubArg(instruction);

        switch (type)
        {
            case 0:
            {
                LDV(instruction);
                break;
            }
            case 1:
            {
                OR(instruction);
                break;
            }
            case 2:
            {
                AND(instruction);
                break;
            }
            case 3:
            {
                XOR(instruction);
                break;
            }
            case 4:
            {
                ADD(instruction);
                break;
            }
            case 5:
            {
                SUB(instruction);
                break;
            }
            case 6:
            {
                SHR(instruction);
                break;
            }
            case 7:
            {
                SUBN(instruction);
                break;
            }
            case 0xE:
            {
                SHL(instruction);   
                break;
            }
        }
    }
    private void EInstructions(ushort instruction)
    {
        ushort type = InstructionParser.GetAddress(instruction);

        switch (type)
        {
            case 0x9E:
            {
                SKP(instruction);
                break;
            }
            case 0xA1:
            {
                SKNP(instruction);
                break;
            }
        }
    }
    private void FInstructions(ushort instruction)
    {
        ushort type = InstructionParser.GetAddress(instruction);
        
        if(type == 0x1E)
            ADDI(instruction);
        else
            LD(instruction);
    }
    #endregion

    #region Display
    private bool SetValueOnScreen(int x,int y, byte value)
    {
        bool col = false;
        double xToByte = x / (double) BYTE_SIZE;
        int bytePos = (int) Math.Ceiling(xToByte);
        if ( (bytePos-1) < xToByte)
        {
            int bitPos = x % BYTE_SIZE;
            byte byteVal = (byte) (value >> bitPos);

            if (SetAndCollideOnDisplay(bytePos-1, y, byteVal))
                col = true;
            _display.Print();
            
            byte endMask = (byte) (255 >> (BYTE_SIZE-bitPos));
            byteVal = (byte)(value & endMask);
            if (SetAndCollideOnDisplay(bytePos, y, (byte) (byteVal << (BYTE_SIZE-bitPos))))
                col = true;
            _display.Print();
        }
        else
        {
            if (SetAndCollideOnDisplay(bytePos, y, value))
                col = true;
        }
            
        _display.Print();

        return col;
    }

    private bool SetAndCollideOnDisplay(int x, int y, byte value)
    {
        byte oldValue = _display[x, y];
        _display[x, y] ^= value;
        if ((oldValue & _display[x, y]) > 0)
            return true;
        return false;
    }

    #endregion

    #region Debug

    private string DecToHex(int val)
    {
        return Convert.ToString(val, toBase: 16);
    }

    #endregion
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