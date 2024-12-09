using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Security.Cryptography;
using System.Text;

public class EncryptionConverter : ValueConverter<string, string>
{
    private const string EncryptionKey = "1234567890123456"; // Should be stored securely

    public EncryptionConverter()
        : base(
            v => Encrypt(v), // Convert value before saving to DB
            v => Decrypt(v)  // Convert value when retrieving from DB
        )
    { }

    private static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return null;

        using (Aes aesAlg = Aes.Create())
        {

            int expectedKeySize = 32; // AES 256-bit key size (32 bytes)

           // byte[] paddedKey = new byte[expectedKeySize];
           // byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(EncryptionKey);

            aesAlg.KeySize = 128;

            aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey);
            aesAlg.IV = new byte[16]; // IV is set to a fixed value for simplicity (this is not recommended for production)

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            byte[] encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
            return Convert.ToBase64String(encrypted);
        }
    }

    private static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return null;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.KeySize = 128;
            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aesAlg.IV = new byte[16]; // IV should be the same as used in encryption

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            byte[] buffer = Convert.FromBase64String(cipherText);
            byte[] decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
