using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace dotnet.IO
{
    public class FileUtils
    {
        public static void WriteToFile(string filePath, string data)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(data);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
        }

        public static void WriteToFile(string filePath, byte[] data)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
            }
        }

        public static byte[] ReadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                byte[] data = new byte[fs.Length];
                int numBytesToRead = (int)fs.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fs.Read(data, numBytesRead, numBytesToRead);
                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                return data;
            }
        }
    }
}
