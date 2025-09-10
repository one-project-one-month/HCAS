using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Shared
{
    public static class DevCode
    {
        public static string? ToJson(this object? obj, bool format = false)
        {
            if (obj == null) return string.Empty;
            string? result;
            if (obj is string)
            {
                result = obj.ToString();
                goto Result;
            }

            var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss.sssZ" };
            result = format
                ? JsonConvert.SerializeObject(obj, Formatting.Indented, settings)
                : JsonConvert.SerializeObject(obj, settings);
        Result:
            return result;
        }

        public static T ToObject<T>(this string jsonStr)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(jsonStr,
                    new JsonSerializerSettings { DateParseHandling = DateParseHandling.DateTimeOffset });
                return result!;
            }
            catch
            {
                return (T)Convert.ChangeType(jsonStr, typeof(T));
            }
        }

        public static string HashPassword(this string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(PasswordHasherKeys.SaltSize);

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }
            byte[] key = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: PasswordHasherKeys.Iterations,
                numBytesRequested: PasswordHasherKeys.SaltSize
            );

            var hashBytes = new byte[PasswordHasherKeys.SaltSize + PasswordHasherKeys.KeySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, PasswordHasherKeys.SaltSize);
            Buffer.BlockCopy(key, 0, hashBytes, PasswordHasherKeys.SaltSize, PasswordHasherKeys.KeySize);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(this string hashedPassword, string providedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[PasswordHasherKeys.SaltSize];
            byte[] storedKey = new byte[PasswordHasherKeys.KeySize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, PasswordHasherKeys.SaltSize);
            Buffer.BlockCopy(hashBytes, PasswordHasherKeys.SaltSize, storedKey, 0, PasswordHasherKeys.KeySize);
            byte[] derivedKey = KeyDerivation.Pbkdf2(
                password: providedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: PasswordHasherKeys.Iterations,
                numBytesRequested: PasswordHasherKeys.KeySize
            );
            return CryptographicOperations.FixedTimeEquals(storedKey, derivedKey);
        }
    }

    public class PasswordHasherKeys
    {
        public const int SaltSize = 128 / 8;
        public const int KeySize = 256 / 8;
        public const int Iterations = 100_000;
    } 
    
    public static class EncryptionHelper
    {
        public static string Decrypt(string encrypted)
        {
            // Replace with your decryption logic
            var decryptedBytes = Convert.FromBase64String(encrypted);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static string Encrypt(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }


    }
}
