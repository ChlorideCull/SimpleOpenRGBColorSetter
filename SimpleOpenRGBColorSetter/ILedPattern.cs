namespace SimpleOpenRGBColorSetter
{
    public interface ILedPattern
    {
        void Tick(uint deltaMilliSeconds, ulong totalMilliSeconds);
    }
}