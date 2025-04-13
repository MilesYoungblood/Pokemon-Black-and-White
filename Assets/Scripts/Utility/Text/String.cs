using System.Globalization;
using UnityEngine;

namespace Scripts.Utility
{
    public static class String
    {
        public static string ToTitleCase(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string ColorText(this object text, Color color)
        {
            return text.ToString().ColorText(color);
        }

        public static string ColorText(this string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }

        public static string ColorText(this object text, string hex)
        {
            return text.ToString().ColorText(hex);
        }

        public static string ColorText(this string text, string hex)
        {
            return $"<color=#{hex}>{text}</color>";
        }

        public static string FontSize(this string text, uint size)
        {
            return $"<size={size.ToString()}>{text}</size>";
        }

        public static string BoldText(string text)
        {
            return $"<b>{text}</b>";
        }

        public static string ItalicsText(string text)
        {
            return $"<i>{text}</i>";
        }
    }
}