using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenRGB.NET;
using OpenRGB.NET.Models;
using SimpleOpenRGBColorSetter.LedPatternGenerators;

namespace SimpleOpenRGBColorSetter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var devices = new List<Tuple<OpenRGBDevice, ILedPattern>>();
            
            var client = new OpenRGBClient(name: "SimpleOpenRGBColorSetter", autoconnect: false);
            
            while (!stoppingToken.IsCancellationRequested) // Attempt to connect, forever.
            {
                try
                {
                    client.Connect();
                    break;
                }
                catch (TimeoutException)
                {
                    _logger.LogError("Failed to connect to OpenRGB, retrying...");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            if (stoppingToken.IsCancellationRequested)
                return;

            var maxDeviceIndex = client.GetControllerCount();
            for (var deviceIndex = 0; deviceIndex < maxDeviceIndex; deviceIndex++)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;
                
                var device = client.GetControllerData(deviceIndex);
                
                int? staticModeIndex = null;
                int? directModeIndex = null;
                for (int i = 0; i < device.Modes.Length; i++)
                {
                    if (device.Modes[i].Name.ToLowerInvariant() == "static")
                    {
                        staticModeIndex = i;
                    }
                    else if (device.Modes[i].Name.ToLowerInvariant() == "direct")
                    {
                        directModeIndex = i;
                    }

                    if (staticModeIndex != null && directModeIndex != null)
                    {
                        break;
                    }
                }
                
                if (staticModeIndex == null || directModeIndex == null)
                {
                    _logger.LogError($"Device '{device.Name}' is missing a Static or Direct mode.");
                    continue;
                }
                
                // First set Static then Direct to work around OpenRGB issue #444, which happens with basically random devices. 
                client.SetMode(deviceIndex, staticModeIndex.Value);
                await Task.Delay(100, stoppingToken); // Letting OpenRGB catch up a bit...
                client.SetMode(deviceIndex, directModeIndex.Value);

                for (int zoneIndex = 0; zoneIndex < device.Zones.Length; zoneIndex++)
                {
                    var abstractedDevice = new OpenRGBDevice(client, deviceIndex, zoneIndex);
                    var effect = new StaticLedPattern(abstractedDevice, new Color(0, 255, 255));
                    //var effect = new SineLedPattern(abstractedDevice, new Color(0, 255, 255), new Color(255, 80, 0));
                    devices.Add(new Tuple<OpenRGBDevice, ILedPattern>(abstractedDevice, effect));
                }
            }


            var lastTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;
                var now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                foreach (var deviceTuple in devices)
                {
                    deviceTuple.Item2.Tick((uint)(lastTime - now), (ulong)now);
                    deviceTuple.Item1.SetColors();
                }

                lastTime = now;
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}