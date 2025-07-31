using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using Standard.Licensing;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Teknomatrik.Lesen;

public class LesenManager
{
    public Guid Guid { get; set; }
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }

    public LesenManager()
    {
        
    }

    public License CreateLicense()
    {
        var passPhrase = "Ini lesen yang ku mahu";

        var keyGenerator = new Standard.Licensing.Security.Cryptography.KeyGenerator(384);  // ECC P_384
        var keyPair = keyGenerator.GenerateKeyPair();
        PrivateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);
        PublicKey = keyPair.ToPublicKeyString();

        Console.WriteLine("------ Private Key ------\n{0}\n------ Private Key ------", PrivateKey);
        Console.WriteLine("------ Public  Key ------\n{0}\n------ Public  Key ------", PublicKey);

        return CreateLicense(Guid, PrivateKey, passPhrase);
    }
    public License CreateLicense(Guid guid, string privateKey, string passPhrase) => 
        License.New()
            .WithUniqueIdentifier(guid)
            .As(LicenseType.Trial)
            .ExpiresAt(DateTime.Now.AddDays(30))
            .WithMaximumUtilization(5)
            .WithProductFeatures(new Dictionary<string, string>
                {
                    {"Sales Module", "yes"},
                    {"Purchase Module", "yes"},
                    {"Maximum Transactions", "10000"}
                })
            .LicensedTo("John Doe", "john.doe@example.com")
            .CreateAndSignWithPrivateKey(privateKey, passPhrase);

}

internal class Program
{
    public Program()
    {
        
    }

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

        var license = (new LesenManager() { Guid = guid }).CreateLicense();

        Console.WriteLine("License: {0}", license.ToString());

    }
}