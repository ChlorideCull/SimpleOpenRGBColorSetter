using OpenRGB.NET.Models;

namespace SimpleOpenRGBColorSetter
{
    public class OpenRGBLed : ILed
    {
        public OpenRGBLed(uint ledIndex, int x, int y)
        {
            LedIndex = ledIndex;
            X = x;
            Y = y;
        }
        
        public Color Color { get; set; }
        public int X { get; }
        public int Y { get; }
        public uint LedIndex { get; }
    }
}