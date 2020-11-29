using System.Linq;

namespace RafaelEstevam.Simple.Spider.Extensions
{
    /// <summary>
    /// Extensions to deal with text
    /// </summary>
    public static class TextExtensions
    {
        /// <summary>
        /// Decodes text using helper HtmlParseHelper.HtmlDecode
        /// </summary>
        public static string HtmlDecode(this string EncodedHtml)
        {
            return Helper.HtmlParseHelper.HtmlDecode(EncodedHtml);
        }

        /// <summary>
        /// Truncates a string to a specified max length
        /// </summary>
        /// <param name="Text">Text to be truncated</param>
        /// <param name="MaxLength">Length to be truncated at</param>
        /// <returns>Original string or new truncated one</returns>
        public static string TruncateMax(this string Text, int MaxLength)
        {
            if (Text == null) return null;
            if (Text.Length <= MaxLength) return Text;
            return Text[0..MaxLength];
        }

        /// <summary>
        /// Indicates whether all characters are categorized as white space
        /// </summary>
        public static bool IsWhiteSpace(this string Text)
        {
            return Text.All(c => char.IsWhiteSpace(c));
        }
        /// <summary>
        /// Indicates whether all characters are categorized as decimal digit
        /// </summary>
        public static bool IsDigit(this string Text)
        {
            return Text.All(c => char.IsDigit(c));
        }
        /// <summary>
        /// Indicates whether all characters are categorized as a leeter or a decimal digit
        /// </summary>
        public static bool IsLetterOrDigit(this string Text)
        {
            return Text.All(c => char.IsLetterOrDigit(c));
        }
        /// <summary>
        /// Indicates whether all characters are categorized as letter
        /// </summary>
        public static bool IsLetter(this string Text)
        {
            return Text.All(c => char.IsLetter(c));
        }

        /// <summary>
        /// Indicates whether all characters are categorized as lowercase letter
        /// </summary>
        public static bool IsLower(this string Text)
        {
            return Text.All(c => char.IsLower(c));
        }
        /// <summary>
        /// Indicates whether all characters are categorized as upperercase letter
        /// </summary>
        public static bool IsUpper(this string Text)
        {
            return Text.All(c => char.IsUpper(c));
        }
        
        /// <summary>
        /// Indicates whether any characters are categorized as white space
        /// </summary>
        public static bool HasAnyWhiteSpace(this string Text)
        {
            return Text.Any(c => char.IsWhiteSpace(c));
        }
        /// <summary>
        /// Indicates whether any characters are categorized as decimal digit
        /// </summary>
        public static bool HasAnyDigit(this string Text)
        {
            return Text.Any(c => char.IsDigit(c));
        }
        /// <summary>
        /// Indicates whether any characters are categorized as a leeter or a decimal digit
        /// </summary>
        public static bool HasAnyLetterOrDigit(this string Text)
        {
            return Text.Any(c => char.IsLetterOrDigit(c));
        }
        /// <summary>
        /// Indicates whether any characters are categorized as letter
        /// </summary>
        public static bool HasAnyLetter(this string Text)
        {
            return Text.Any(c => char.IsLetter(c));
        }
    }
}
