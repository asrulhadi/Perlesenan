using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using Standard.Licensing;
using System;
using System.Security.Cryptography;

namespace Teknomatrik.Lesen;

public class LesenManager
{
    public Guid Guid { get; set; }
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }

    private static readonly LesenManager _instance = new LesenManager();
    public static LesenManager Instance => _instance;

    private LesenManager()
    {
        CreateNewKeyPair("Ini lesen yang ku mahu");
    }

    private void CreateNewKeyPair(string passPhrase)
    {
        var keyGenerator = new Standard.Licensing.Security.Cryptography.KeyGenerator(384);  // ECC P_384
        var keyPair = keyGenerator.GenerateKeyPair();
        PrivateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);
        PublicKey = keyPair.ToPublicKeyString();

        Console.WriteLine("------ Private Key ------\n{0}\n------ Private Key ------", PrivateKey);
        Console.WriteLine("------ Public  Key ------\n{0}\n------ Public  Key ------", PublicKey);
    }
    public License CreateLicense()
    {
        var passPhrase = "Ini lesen yang ku mahu";

        CreateNewKeyPair(passPhrase);

        return CreateLicense(Guid, passPhrase);
    }
    public License CreateLicense(Guid guid, string passPhrase) =>
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
            .CreateAndSignWithPrivateKey(PrivateKey, passPhrase);
    
    public static License CreateLicense(Guid guid)
        => LesenManager.Instance.CreateLicense(guid, "Ini lesen yang ku mahu");

    public (Guid guid, String deviceId) GetGuid()
    {
        var guidFormatter = new HashDeviceIdFormatter(() => MD5.Create(), new HexByteArrayEncoder());
        var stringFormatter = new StringDeviceIdFormatter(new PlainTextDeviceIdComponentEncoder(),"*");
        DeviceIdBuilder deviceIdBuilder = new DeviceIdBuilder()
            .AddMachineName()
            .AddOsVersion()
            //.AddMacAddress()
            .OnWindows(windows => windows
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .AddSystemUuid())
            .OnLinux(linux => linux
                .AddCpuInfo()
                .AddMotherboardSerialNumber()
                .AddProductUuid())
            ;
        string deviceId = deviceIdBuilder.UseFormatter(stringFormatter).ToString();
        Guid guid = new(deviceIdBuilder.UseFormatter(guidFormatter).ToString());
        Console.WriteLine($"device id = {deviceId} guid {guid}");
        return (guid, deviceId);
    }

    public void SaveKeyPair(string folder = null, string passPhrase = "Ini lesen yang ku mahu")
    {
        string privateKeyFilename = "private.key";
        string publicKeyFilename = "public.key";
        string directory = Environment.CurrentDirectory;
        // set folder to save
        if (!String.IsNullOrEmpty(folder) && Path.Exists(folder)) directory = folder;
        string privateKeyLocation = Path.Combine(directory, privateKeyFilename);
        string publicKeyLocation = Path.Combine(directory, publicKeyFilename);
        // Save the key pair
        File.WriteAllText(privateKeyLocation, PrivateKey);
        File.WriteAllText(publicKeyLocation, PublicKey);
    }
}
