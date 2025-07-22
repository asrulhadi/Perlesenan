using System.Security.Cryptography;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        string deviceId = new DeviceIdBuilder()
            .AddMachineName()
            .AddOsVersion()
            .UseFormatter(new HashDeviceIdFormatter(() => MD5.Create(), new HexByteArrayEncoder()))
            .ToString();
        Guid guid = new(deviceId);
        Console.WriteLine($"device id = {deviceId} guid {guid}");
    }
}