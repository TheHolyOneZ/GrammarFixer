using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GrammarFixer.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly string KeyPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GrammarFixer",
            ".key"
        );

        private static byte[] GetOrCreateKey()
        {
            try
            {
                if (File.Exists(KeyPath))
                {
                    return File.ReadAllBytes(KeyPath);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(KeyPath)!);
                
                byte[] key = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(key);
                }
                
                File.WriteAllBytes(KeyPath, key);
                File.SetAttributes(KeyPath, FileAttributes.Hidden);
                
                return key;
            }
            catch
            {
                byte[] fallbackKey = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(fallbackKey);
                }
                return fallbackKey;
            }
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                byte[] key = GetOrCreateKey();
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        msEncrypt.Write(aes.IV, 0, aes.IV.Length);
                        
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch
            {
                return plainText;
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                byte[] key = GetOrCreateKey();
                byte[] buffer = Convert.FromBase64String(cipherText);
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    
                    byte[] iv = new byte[aes.IV.Length];
                    Array.Copy(buffer, 0, iv, 0, iv.Length);
                    aes.IV = iv;
                    
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    
                    using (MemoryStream msDecrypt = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch
            {
                return cipherText;
            }
        }
    }
}