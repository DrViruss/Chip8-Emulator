namespace Base_Lib;

public interface ITimer
{
    byte time { get; set; }
}

public class Timer : ITimer
{
    public byte time { get; set; }

    public void Tick()
    {
        if (time > 0)
            time--;
    }
}