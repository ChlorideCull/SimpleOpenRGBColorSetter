using OpenRGB.NET.Models;

namespace SimpleOpenRGBColorSetter
{
    public class StaticLedPattern : ILedPattern
    {
        private readonly IDevice _device;
        private readonly Color _color;

        public StaticLedPattern(IDevice device, Color color)
        {
            _device = device;
            _color = color;
        }
        
        public void Tick(uint deltaMilliSeconds, ulong totalMilliSeconds)
        {
            foreach (var led in _device.Leds)
            {
                led.Color = _color;
            }
        }
    }
}