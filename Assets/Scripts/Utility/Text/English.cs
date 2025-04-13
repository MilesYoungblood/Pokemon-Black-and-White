using System;
using System.Linq;
using JetBrains.Annotations;

namespace Scripts.Utility
{
    /// <summary>
    ///     A collection of english-related functions.
    /// </summary>
    public static class English
    {
        /// <summary>
        ///     Gets the respective article, a(n), of an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>"an" if the word is a vowel and "a" if the word is a consonant.</returns>
        public static string GetIndefiniteArticle([NotNull] this object obj)
        {
            return IsVowel(obj.ToString().First()) ? "an" : "a";
        }

        /// <summary>
        ///     Pluralizes a regular word.
        /// </summary>
        /// <param name="obj">The object to pluralize.</param>
        /// <returns>The plural form of the word.</returns>
        /// <exception cref="ArgumentException">Thrown if the object passed is null.</exception>
        /// <remarks>This function for work for regular words. It will work inaccurately for irregular words.</remarks>
        public static string PluralizeRegularWord([NotNull] object obj)
        {
            var s = obj.ToString();
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException("Input string cannot be null or empty", nameof(s));
            }

            if (s.EndsWith("y") && !IsVowel(s[^2]))
            {
                return $"{s[..^1]}ies";
            }

            if (s.EndsWith("s") || s.EndsWith("sh") || s.EndsWith("ch") || s.EndsWith("x") || s.EndsWith("z"))
            {
                return $"{s}es";
            }

            if (s.EndsWith("f"))
            {
                return $"{s[..^1]}ves";
            }

            return s.EndsWith("fe") ? $"{s[..^2]}ves" : $"{s}s";
        }

        /// <summary>
        ///     Checks if a character is a vowel.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>True if the character is a vowel and false if it is a consonant.</returns>
        private static bool IsVowel(char c)
        {
            return char.ToUpper(c) is 'A' or 'E' or 'I' or 'O' or 'U';
        }
    }
}
