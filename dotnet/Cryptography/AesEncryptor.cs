using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace dotnet.Cryptography
{
    public static class AesEncryptor
    {
        public static bool AESEncrypt(string key, byte[] data, out byte[] encryptData)
        {
            encryptData = null;

            try
            {
                byte[] keyBytes = PasswordToKey(key);
                using (var aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        byte[] encrtptBytes = encryptor.TransformFinalBlock(data, 0, data.Length);
                        encryptData = aes.IV.Concat(encrtptBytes).ToArray();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool AESDecrypt(byte[] pack, string key, out byte[] originalData)
        {
            originalData = null;

            if (pack == null || pack.Length < 17 || string.IsNullOrEmpty(key))
                return false;

            try
            {
                byte[] keyBytes = PasswordToKey(key);
                byte[] ivBytes = pack.Take(16).ToArray();
                byte[] data = pack.Skip(16).ToArray();
                using (var aes = Aes.Create())
                {
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(keyBytes, ivBytes))
                    {
                        originalData = decryptor.TransformFinalBlock(data, 0, data.Length);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] PasswordToKey(string password)
        {
            HashAlgorithm md5 = HashAlgorithm.Create("MD5");
            byte[] hashData = md5.ComputeHash(Encoding.Unicode.GetBytes(password));
            return hashData;
        }
    }

    #region MS doc Examples

    class AesExample
    {
        public static void Test()
        {
            string original = "Here is some data to encrypt!";

            // Create a new instance of the Aes
            // class.  This generates a new key and initialization 
            // vector (IV).
            using (Aes myAes = Aes.Create())
            {

                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

                // Decrypt the bytes to a string.
                string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

                //Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", original);
                Console.WriteLine("Round Trip: {0}", roundtrip);
            }
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }

    #endregion
}
