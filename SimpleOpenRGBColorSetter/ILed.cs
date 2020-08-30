using System;
using OpenRGB.NET.Models;

namespace SimpleOpenRGBColorSetter
{
    public interface ILed
    {
        Color Color { get; set; }
        int X { get; }
        int Y { get; }
    }
}