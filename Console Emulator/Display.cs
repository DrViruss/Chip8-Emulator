using Base_Lib;

namespace Console_Emulator;

public class Display : IDisplay
{
    public byte[,] _screen { get; /*private set;*/ }
    private byte[,] _prevScreen;

    public void Print()
    {
        for (int y = 0; y < 32; y++) {
            for (int x = 0; x < 8; x++) {
                if(_screen[y,x] != _prevScreen[y,x]) {

                    Console.SetCursorPosition(x * 8, y);
                    byte value = _screen[y,x];
                    for (int i = 0; i < 8; i++) {
                        string _out = " ";
                        int shift = 7 - i;
                        int flag = (value >> shift) & 0b1;
                        if (flag > 0)
                            _out = "*";
                        Console.Write(_out);

                        // string output = " ";
                        // int flag = 0;
                        // if(i == 0 && word == 1) {
                        //     output = "*";
                        // } else
                        //     flag = 1 << i;
                        //
                        // if((flag & word) > 0)
                        //     output = "*";
                        //
                        // Console.Write(output);
                    }
                }
            }
        }
        Array.Copy(_screen, _prevScreen, _screen.Length);
        Console.SetCursorPosition(0, 32);
    }

    public Display()
    {
        _screen = new byte[32,8];
        _prevScreen = new byte[32,8];
        Console.SetWindowSize(64, 32);
        Console.CursorVisible = false;
    }
    
}