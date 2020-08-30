using System;
using OpenRGB.NET.Models;

namespace SimpleOpenRGBColorSetter.LedPatternGenerators
{
    public class SineLedPattern : ILedPattern
    {
        private IDevice _device;
        private Color _color1;
        private Color _color2;
        
        public SineLedPattern(IDevice device, Color color1, Color color2)
        {
            _device = device;
            _color1 = color1;
            _color2 = color2;
        }
        
        public void Tick(uint deltaMilliSeconds, ulong totalMilliSeconds)
        {
            var sin = Math.Sin((double)totalMilliSeconds / 1000);

            var targetR = ((_color2.R - _color1.R) * sin) + _color1.R;
            var targetG = ((_color2.G - _color1.G) * sin) + _color1.G;
            var targetB = ((_color2.B - _color1.B) * sin) + _color1.B;
            var targetColor = new Color((byte)targetR, (byte)targetG, (byte)targetB);
            foreach (var led in _device.Leds)
            {
                led.Color = targetColor;
            }
        }
    }
}