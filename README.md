# Simple OpenRGB Color Setter / Effects Engine

To change the pattern or refresh rate, you'll need to change the code and recompile :)
A ~30 Hz refresh rate causes OpenRGB to use 3% of my CPU, roughly.

# Running as a service

1. Download a recent pipeline build of [OpenRGB](https://gitlab.com/CalcProgrammer1/OpenRGB) (not a proper release version, it's too old)
2. Run it in server mode as a service using NSSM, using the name "OpenRGB"
3. Create a standalone build by running `dotnet publish -r win-x64 -c Release`
4. Create the service by running `New-Service -Name "SimpleOpenRGBColorSetter" -BinaryPathName "<path to SimpleOpenRGBColorSetter.exe>" -DependsOn "OpenRGB"` in PowerShell run as Administrator.