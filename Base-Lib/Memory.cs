namespace Base_Lib;

public class Memory
{
    private byte[] buffer; //4kb
    
    public int MemSize => 4096;

    public Memory()
    {
        buffer = new byte[MemSize];
    }

    public byte this[int memIndex]
    {
        get => buffer[memIndex];
        set => buffer[memIndex] = value;
    }

    /*
     Load Interpreter(Sprite set) to memory
     from 0x200 to 0x4096
     */
    public void LoadProgram(byte[] program)
    {
        program.CopyTo(buffer,0x200);
    }

    /*
     Load Interpreter(Sprite set) to memory
     from 0x0 to 0x200
     */
    public void LoadROM(byte[] rom)
    {
        rom.CopyTo(buffer,0x0); //load interpreter
    }
    
    /*
     Initialize standard interpreter.
     Adds base sprite set with values: 0-F 
     */
    public void InitializeInterpreterBuffer(){ //interpreter
        // 0
        buffer[0] = 0xF0;
        buffer[1] = 0x90;
        buffer[2] = 0x90;
        buffer[3] = 0x90;
        buffer[4] = 0xF0;
        // 1
        buffer[5] = 0x20;
        buffer[6] = 0x60;
        buffer[7] = 0x20;
        buffer[8] = 0x20;
        buffer[9] = 0x70;
        // 2
        buffer[10] = 0xF0;
        buffer[11] = 0x10;
        buffer[12] = 0xF0;
        buffer[13] = 0x80;
        buffer[14] = 0xF0;
        //3 
        buffer[15] = 0xF0;
        buffer[16] = 0x10;
        buffer[17] = 0xF0;
        buffer[18] = 0x10;
        buffer[19] = 0xF0;
        //4 
        buffer[20] = 0x90;
        buffer[21] = 0x90;
        buffer[22] = 0xF0;
        buffer[23] = 0x10;
        buffer[24] = 0x10;
        //5 
        buffer[29] = 0xF0;
        buffer[25] = 0x80;
        buffer[26] = 0xF0;
        buffer[27] = 0x10;
        buffer[28] = 0xF0;
        //6 
        buffer[30] = 0xF0;
        buffer[31] = 0x80;
        buffer[32] = 0xF0;
        buffer[33] = 0x90;
        buffer[34] = 0xF0;
        // 7
        buffer[35] = 0xF0;
        buffer[36] = 0x10;
        buffer[37] = 0x20;
        buffer[38] = 0x40;
        buffer[39] = 0x40;
        // 8
        buffer[40] = 0xF0;
        buffer[41] = 0x90;
        buffer[42] = 0xF0;
        buffer[43] = 0x90;
        buffer[44] = 0xF0;
        // 9
        buffer[45] = 0xF0;
        buffer[46] = 0x90;
        buffer[47] = 0xF0;
        buffer[48] = 0x10;
        buffer[49] = 0xF0;
        // A
        buffer[50] = 0xF0;
        buffer[51] = 0x90;
        buffer[52] = 0xF0;
        buffer[53] = 0x90;
        buffer[54] = 0x90;
        // B
        buffer[55] = 0xE0;
        buffer[56] = 0x90;
        buffer[57] = 0xE0;
        buffer[58] = 0x90;
        buffer[59] = 0xE0;
        // C
        buffer[60] = 0xF0;
        buffer[61] = 0x80;
        buffer[62] = 0x80;
        buffer[63] = 0x80;
        buffer[64] = 0xF0;
        // D
        buffer[65] = 0xE0;
        buffer[66] = 0x90;
        buffer[67] = 0x90;
        buffer[68] = 0x90;
        buffer[69] = 0xE0;
        // E
        buffer[70] = 0xF0;
        buffer[71] = 0x80;
        buffer[72] = 0xF0;
        buffer[73] = 0x80;
        buffer[74] = 0xF0;
        // F
        buffer[75] = 0xF0;
        buffer[76] = 0x80;
        buffer[77] = 0xF0;
        buffer[78] = 0x80;
        buffer[79] = 0x80;

    }
    
}