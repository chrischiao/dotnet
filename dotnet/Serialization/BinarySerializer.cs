using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace dotnet.Serialization
{
    public static class BinarySerializer
    {
        public static byte[] ObjectToBytes(object obj, bool isCompress = false)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                var bytes = stream.ToArray(); // "ToArray" omits unused bytes in MemoryStream from the array. To get the entire buffer, use "GetBuffer" method.
                stream.Close();

                return isCompress ? Compress(bytes) : bytes;
            }
        }

        public static object GetObject(byte[] bytes, bool isDecompress = false)
        {
            if (isDecompress)
                bytes = Decompress(bytes);

            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }

        public static byte[] Compress(byte[] bytes)
        {
            using (var memStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Compress))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                    zipStream.Close();
                    memStream.Close();

                    return memStream.ToArray();
                }
            }
        }

        public static byte[] Decompress(byte[] bytes)
        {
            using (var inStream = new MemoryStream(bytes))
            {
                using (var zipStream = new GZipStream(inStream, CompressionMode.Decompress))
                {
                    using (var outStream = new MemoryStream())
                    {
                        var readData = new byte[4096];
                        int readLength;
                        while ((readLength = zipStream.Read(readData, 0, readData.Length)) > 0)
                        {
                            outStream.Write(readData, 0, readLength);
                        }
                        outStream.Position = 0;
                        inStream.Close();
                        zipStream.Close();
                        outStream.Close();

                        return outStream.ToArray();
                    }
                }
            }
        }
    }

    [Serializable]
    internal class Product
    {
        public string Name { get; set; }
    }
}
