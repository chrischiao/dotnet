using System;
using System.IO;
using System.Xml.Serialization;

namespace dotnet.IO
{
    public static class XmlHelper<T> where T : class
    {
        public static bool ExportToXml(T t, string filePath, FileMode fileMode = FileMode.Create)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, fileMode))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(fileStream, t);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static T ImportFromXml(string filePath)
        {
            T t = null;

            try
            {
                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        t = serializer.Deserialize(reader) as T;
                    }
                }
            }
            catch (Exception ex)
            {
                t = null;
                Console.WriteLine(ex.Message);
            }

            return t;
        }

        public static T ImportFromXmlText(string xmlText)
        {
            T t;
  
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                t = serializer.Deserialize(new StringReader(xmlText)) as T;
            }
            catch (Exception ex)
            {
                t = null;
                Console.WriteLine(ex.Message);
            }

            return t;
        }
    }
}
