using Base_Lib;

namespace Console_Emulator;

public class Keyboard : IKeyboard
{
    public byte WaitForKey()
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        return (byte) keyInfo.KeyChar;
    }
}