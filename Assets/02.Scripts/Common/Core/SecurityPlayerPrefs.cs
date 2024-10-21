using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class SecurityPlayerPrefs
{
    private static byte[] _keys;
    private static byte[] _iv;
    private static int keySize = 256;
    private static int blockSize = 128;
    private static int _hashLen = 64;

    static SecurityPlayerPrefs()
    {
        GenerateKeys();
    }

    private static void GenerateKeys()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }

        string randomSeedForKey = "5b6fcb4aaa0a42acae649eba45a506ec";
        string randomSeedForValue = "2e327725789841b5bb5c706d6b2ad897";

        using (var key = new Rfc2898DeriveBytes(randomSeedForKey, saltBytes, 1000))
        {
            _keys = key.GetBytes(keySize / 8);
        }

        using (var key = new Rfc2898DeriveBytes(randomSeedForValue, saltBytes, 1000))
        {
            _iv = key.GetBytes(blockSize / 8);
        }
    }

    public static string MakeHash(string original)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(original);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            StringBuilder hashToString = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashToString.Append(b.ToString("x2"));
            }

            return hashToString.ToString();
        }
    }

    public static byte[] Encrypt(byte[] bytesToBeEncrypted)
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.KeySize = keySize;
            aes.BlockSize = blockSize;
            aes.Key = _keys;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform ct = aes.CreateEncryptor())
            {
                return ct.TransformFinalBlock(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
            }
        }
    }

    public static byte[] Decrypt(byte[] bytesToBeDecrypted)
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.KeySize = keySize;
            aes.BlockSize = blockSize;
            aes.Key = _keys;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform ct = aes.CreateDecryptor())
            {
                return ct.TransformFinalBlock(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
            }
        }
    }

    public static string Encrypt(string input)
    {
        byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
        byte[] bytesEncrypted = Encrypt(bytesToBeEncrypted);

        return System.Convert.ToBase64String(bytesEncrypted);
    }

    public static string Decrypt(string input)
    {
        byte[] bytesToBeDecrypted = System.Convert.FromBase64String(input);
        byte[] bytesDecrypted = Decrypt(bytesToBeDecrypted);

        return Encoding.UTF8.GetString(bytesDecrypted);
    }

    private static void SetSecurityValue(string key, string value)
    {
        string hideKey = MakeHash(key);
        string encryptValue = Encrypt(value + MakeHash(value));

        PlayerPrefs.SetString(hideKey, encryptValue);
    }

    private static string GetSecurityValue(string key)
    {
        string hideKey = MakeHash(key);

        string encryptValue = PlayerPrefs.GetString(hideKey);
        if (string.IsNullOrEmpty(encryptValue))
            return string.Empty;

        string valueAndHash = Decrypt(encryptValue);
        if (_hashLen > valueAndHash.Length)
            return string.Empty;

        string savedValue = valueAndHash.Substring(0, valueAndHash.Length - _hashLen);
        string savedHash = valueAndHash.Substring(valueAndHash.Length - _hashLen);

        if (MakeHash(savedValue) != savedHash)
            return string.Empty;

        return savedValue;
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(MakeHash(key));
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static void SetInt(string key, int value)
    {
        SetSecurityValue(key, value.ToString());
    }

    public static void SetLong(string key, long value)
    {
        SetSecurityValue(key, value.ToString());
    }

    public static void SetFloat(string key, float value)
    {
        SetSecurityValue(key, value.ToString());
    }

    public static void SetString(string key, string value)
    {
        SetSecurityValue(key, value);
    }

    public static int GetInt(string key, int defaultValue)
    {
        string originalValue = GetSecurityValue(key);
        if (string.IsNullOrEmpty(originalValue))
            return defaultValue;

        if (!int.TryParse(originalValue, out int result))
            return defaultValue;

        return result;
    }

    public static long GetLong(string key, long defaultValue)
    {
        string originalValue = GetSecurityValue(key);
        if (string.IsNullOrEmpty(originalValue))
            return defaultValue;

        if (!long.TryParse(originalValue, out long result))
            return defaultValue;

        return result;
    }

    public static float GetFloat(string key, float defaultValue)
    {
        string originalValue = GetSecurityValue(key);
        if (string.IsNullOrEmpty(originalValue))
            return defaultValue;

        if (!float.TryParse(originalValue, out float result))
            return defaultValue;

        return result;
    }

    public static string GetString(string key, string defaultValue)
    {
        string originalValue = GetSecurityValue(key);
        if (string.IsNullOrEmpty(originalValue))
            return defaultValue;

        return originalValue;
    }
}
