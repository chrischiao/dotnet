using System.IO;
using System.Xml.Serialization;

namespace dotnet.Serialization
{
    public static class XmlSerializer<T> where T : class
    {
        public static byte[] Serialize(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, obj);
                var bytes = stream.GetBuffer();
                stream.Close();
                return bytes;
            }
        }

        public static T Deserialize(byte[] bytes)
        {
            T obj = default(T);

            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                var serializer = new XmlSerializer(typeof(T));
                obj = serializer.Deserialize(stream) as T;
            }

            return obj;
        }
    }
}
