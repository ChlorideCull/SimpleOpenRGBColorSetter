using System.Collections.Generic;

namespace SimpleOpenRGBColorSetter
{
    public interface IDevice
    {
        uint LedMatrixWidth { get; }
        uint LedMatrixHeight { get; }
        IEnumerable<ILed> Leds { get; }
    }
}