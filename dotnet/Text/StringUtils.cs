using System;
using System.Text;
using System.Security.Cryptography;

namespace dotnet.Text
{
    public static class StringUtils
    {
        public static string ToMd5(this string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.Default.GetBytes(text);
                byte[] hash = md5.ComputeHash(bytes);

                //return string.Join("", bytes.Select(b => b.ToString("x2")));
                return BitConverter.ToString(hash).ToLower().Replace("-", ""); 
            }
        }
    }
}
