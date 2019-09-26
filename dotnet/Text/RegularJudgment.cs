using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace dotnet.Text
{
    public static class RegularJudgment
    {
        private static readonly RegularPatterns RegularPatterns = new RegularPatterns();

        public static bool IsKindOf(this string str, string pattern)
        {
            return Regex.IsMatch(str, pattern);
        }

        public static bool IsKindOf(this string text, TextType textType)
        {
            if (textType == TextType.Common) return true;
            return Regex.IsMatch(text,
                RegularPatterns.GetValue(Enum.GetName(typeof(TextType), textType) + "Pattern").ToString());
        }

        public static bool IsEmail(this string email)
        {
            return Regex.IsMatch(email, RegularPatterns.MailPattern);
        }

        public static bool IsIp(this string ip, IpType ipType)
        {
            switch (ipType)
            {
                case IpType.A: return Regex.IsMatch(ip, RegularPatterns.IpAPattern);
                case IpType.B: return Regex.IsMatch(ip, RegularPatterns.IpBPattern);
                case IpType.C: return Regex.IsMatch(ip, RegularPatterns.IpCPattern);
                case IpType.D: return Regex.IsMatch(ip, RegularPatterns.IpDPattern);
                case IpType.E: return Regex.IsMatch(ip, RegularPatterns.IpEPattern);
                default: return false;
            }
        }

        public static bool IsIp(this string ip)
        {
            return Regex.IsMatch(ip, RegularPatterns.IpPattern);
        }

        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch(str, RegularPatterns.ChinesePattern);
        }

        public static bool IsUrl(this string str)
        {
            return Regex.IsMatch(str, RegularPatterns.UrlPattern);
        }
    }

    public enum TextType
    {
        Common,
        Phone,
        Mail,
        Url,
        Chinese,
        Number,
        Digits,
        PInt,
        NInt,
        Int,
        NnInt,
        NpInt,
        PDouble,
        NDouble,
        Double,
        NnDouble,
        NpDouble,
    }

    public enum IpType
    {
        A = 0,
        B,
        C,
        D,
        E
    }
}
