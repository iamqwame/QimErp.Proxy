using System.Security.Cryptography;
using System.Text;

namespace QimErp.Proxy.Mobile.Shared.Services;

public interface IProxyCommandPayloadProtector
{
    string Protect(string plaintext);
    string Unprotect(string ciphertext);
}

public sealed class ProxyCommandPayloadProtector(IConfiguration configuration) : IProxyCommandPayloadProtector
{
    private readonly byte[] _key = ReadKey(configuration);

    public string Protect(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(12);
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[16];
        using var aes = new AesGcm(_key, tag.Length);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);
        return Convert.ToBase64String(nonce.Concat(tag).Concat(cipherBytes).ToArray());
    }

    public string Unprotect(string ciphertext)
    {
        var payload = Convert.FromBase64String(ciphertext);
        if (payload.Length < 28)
            throw new CryptographicException("Invalid protected command payload.");
        var nonce = payload[..12];
        var tag = payload[12..28];
        var cipherBytes = payload[28..];
        var plainBytes = new byte[cipherBytes.Length];
        using var aes = new AesGcm(_key, tag.Length);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
        return Encoding.UTF8.GetString(plainBytes);
    }

    private static byte[] ReadKey(IConfiguration configuration)
    {
        var encoded = configuration["ProxyCommandProtection:Key"];
        if (string.IsNullOrWhiteSpace(encoded))
            throw new InvalidOperationException("ProxyCommandProtection:Key is required.");
        var key = Convert.FromBase64String(encoded);
        if (key.Length is not (16 or 24 or 32))
            throw new InvalidOperationException("ProxyCommandProtection:Key must be a base64-encoded AES key.");
        return key;
    }
}
