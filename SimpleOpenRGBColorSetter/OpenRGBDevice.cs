using System;
using System.Collections.Generic;
using OpenRGB.NET.Enums;

namespace SimpleOpenRGBColorSetter
{
    public class OpenRGBDevice : IDevice
    {
        private OpenRGB.NET.OpenRGBClient _client;
        private int _controllerIndex;
        private int _zoneIndex;
        
        public OpenRGBDevice(OpenRGB.NET.OpenRGBClient client, int controllerIndex, int zoneIndex)
        {
            _client = client;
            _controllerIndex = controllerIndex;
            _zoneIndex = zoneIndex;

            var device = client.GetControllerData(controllerIndex);
            var zone = device.Zones[zoneIndex];
            _ledCount = zone.LedCount;
            switch (zone.Type)
            {
                case ZoneType.Single:
                    LedMatrixHeight = 1;
                    LedMatrixWidth = 1;
                    _leds = new List<OpenRGBLed>
                    {
                        new OpenRGBLed(0, 0, 0)
                    };
                    break;
                case ZoneType.Linear:
                {
                    LedMatrixHeight = 1;
                    LedMatrixWidth = zone.LedCount;
                    _leds = new List<OpenRGBLed>();
                    for (uint i = 0; i < LedMatrixWidth; i++)
                    {
                        _leds.Add(new OpenRGBLed(i, (int)i, 0));
                    }
                    break;
                }
                case ZoneType.Matrix:
                {
                    LedMatrixHeight = zone.MatrixMap.Height;
                    LedMatrixWidth = zone.MatrixMap.Width;
                    _leds = new List<OpenRGBLed>();
                    for (var x = 0; x < LedMatrixWidth; x++)
                    {
                        for (var y = 0; y < LedMatrixHeight; y++)
                        {
                            var index = zone.MatrixMap.Matrix[y, x];
                            if (index != uint.MaxValue)
                                _leds.Add(new OpenRGBLed(index, x, y));
                        }
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<OpenRGBLed> _leds;
        private uint _ledCount;

        public void SetColors()
        {
            var colors = new OpenRGB.NET.Models.Color[_ledCount];
            foreach (var led in _leds)
            {
                colors[led.LedIndex] = led.Color;
            }
            _client.UpdateZone(_controllerIndex, _zoneIndex, colors);
        }
        
        public uint LedMatrixWidth { get; }
        public uint LedMatrixHeight { get; }
        public IEnumerable<ILed> Leds => _leds;
    }
}