using System;
using System.Text;
using System.Security.Cryptography;

namespace dotnet.Cryptography
{
    public static class RsaEncryptor
    {
        public static byte[] RSAEncrypt(string xmlPublicKey, byte[] data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);
                int keySize = rsa.KeySize / 8;
                int bufferSize = keySize - 11;

                var msInput = new System.IO.MemoryStream(data);
                var msOutput = new System.IO.MemoryStream();
                byte[] buffer = new byte[bufferSize];
                int readLen = msInput.Read(buffer, 0, bufferSize);
                while (readLen > 0)
                {
                    byte[] dataToEnc = new byte[readLen];
                    Array.Copy(buffer, 0, dataToEnc, 0, readLen);
                    byte[] encData = rsa.Encrypt(dataToEnc, false);
                    msOutput.Write(encData, 0, encData.Length);

                    readLen = msInput.Read(buffer, 0, bufferSize);
                }

                byte[] result = msOutput.ToArray();
                msInput.Close();
                msOutput.Close();

                return result;

                #region array operation
                //int length = data.Length;
                //int blockLength = rsa.KeySize / 8 - 11;

                //if (length <= blockLength)
                //    return rsa.Encrypt(data, false);

                //byte[] pack = null;
                //int count = length / blockLength;
                //for (int i = 0; i < count; i++)
                //{
                //    byte[] buffer = rsa.Encrypt(data.Skip(i*blockLength).Take(blockLength).ToArray(), false);
                //    AppendArray(ref pack, buffer);
                //}

                //if (length - count * blockLength > 0)
                //{
                //    byte[] buffer = rsa.Encrypt(data.Skip(count * blockLength).ToArray(), false);
                //    AppendArray(ref pack, buffer);
                //}

                //return pack;
                #endregion
            }
        }

        public static bool RSAVerifySignature(string xmlPublicKey, byte[] hashbytes, byte[] signatureBytes)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);
                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("MD5");
                return rsaDeformatter.VerifySignature(hashbytes, signatureBytes);
            }
        }


        public static bool RSADecrypt(string xmlPrivateKey, byte[] encryptData, out byte[] originalData)
        {
            originalData = null;

            if (encryptData == null || encryptData.Length == 0 || string.IsNullOrEmpty(xmlPrivateKey))
                return false;

            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(xmlPrivateKey);
                    int keySize = rsa.KeySize / 8;

                    var msInput = new System.IO.MemoryStream(encryptData);
                    var msOutput = new System.IO.MemoryStream();
                    byte[] buffer = new byte[keySize];
                    int readLen = msInput.Read(buffer, 0, keySize);
                    while (readLen > 0)
                    {
                        byte[] dataToDec = new byte[readLen];
                        Array.Copy(buffer, 0, dataToDec, 0, readLen);
                        byte[] data = rsa.Decrypt(dataToDec, false);
                        msOutput.Write(data, 0, data.Length);

                        readLen = msInput.Read(buffer, 0, keySize);
                    }

                    originalData = msOutput.ToArray();
                    msInput.Close();
                    msOutput.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static byte[] RSASignature(string xmlPrivateKey, byte[] hashbytes)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPrivateKey);
                RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("MD5");
                return rsaFormatter.CreateSignature(hashbytes);
            }
        }


        public static void GenerateKey(out string privateKey, out string publicKey)
        {
            CspParameters rsaParams = new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore };
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, rsaParams))
            {
                privateKey = rsa.ToXmlString(true);
                publicKey = rsa.ToXmlString(false);
            }
        }
    }

    #region MS doc Examples

    class RSACSPSample
    {
        public static void Test()
        {
            try
            {
                //Create a UnicodeEncoder to convert between byte array and string.
                UnicodeEncoding ByteConverter = new UnicodeEncoding();

                //Create byte arrays to hold original, encrypted, and decrypted data.
                byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
                byte[] encryptedData;
                byte[] decryptedData;

                //Create a new instance of RSACryptoServiceProvider to generate
                //public and private key data.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Pass the data to ENCRYPT, the public key information 
                    //(using RSACryptoServiceProvider.ExportParameters(false),
                    //and a boolean flag specifying no OAEP padding.
                    encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);

                    //Pass the data to DECRYPT, the private key information 
                    //(using RSACryptoServiceProvider.ExportParameters(true),
                    //and a boolean flag specifying no OAEP padding.
                    decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                    //Display the decrypted plaintext to the console. 
                    Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
                }
            }
            catch (ArgumentNullException)
            {
                //Catch this exception in case the encryption did
                //not succeed.
                Console.WriteLine("Encryption failed.");

            }
        }

        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }

        }

        public static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }

        public static void ExportKeyInfo(bool includePrivateParameters)
        {
            try
            {
                //Create a new RSACryptoServiceProvider object.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {


                    //Export the key information to an RSAParameters object.
                    //Pass false to export the public key information or pass
                    //true to export public and private key information.
                    RSAParameters RSAParams = RSA.ExportParameters(includePrivateParameters);
                }


            }
            catch (CryptographicException e)
            {
                //Catch this exception in case the encryption did
                //not succeed.
                Console.WriteLine(e.Message);

            }
        }
    }

    #endregion
}
