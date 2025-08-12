using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using Standard.Licensing;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Teknomatrik.Lesen;

internal class Program
{
    public Program()
    {
        
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var lesen = LesenManager.Instance;

        Guid guid = lesen.GetGuid().guid;
        Console.WriteLine($"guid {guid}");

        var license = LesenManager.CreateLicense(guid);

        Console.WriteLine("License: {0}", license.ToString());

    }
}