namespace Base_Lib;

public interface IDisplay
{
    byte[,] _screen { get; }
    void Print();

    byte this[int x, int y]
    {
        get => _screen[y,x];
        set => _screen[y,x] = value;
    }
}