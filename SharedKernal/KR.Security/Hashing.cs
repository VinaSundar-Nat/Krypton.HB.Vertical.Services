using System;
using System.Security.Cryptography;
using System.Text;

namespace KR.Security;

public interface IHashing
{
    string GenerateSalt(string crypt);
    string Hash(string text, string salt);
}

public partial class Crypto : IHashing
{    
    public string GenerateSalt(string crypt)
    {
        byte[] salt = crypt.ToByteArray();
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);          
    }

    public string Hash(string text, string salt)
    {
        byte[] saltedPassword = Encoding.UTF8.GetBytes(salt + text);
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(saltedPassword);
        return hashBytes.ToString(false);   
    }
}

