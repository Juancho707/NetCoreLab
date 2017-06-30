using System.Text.RegularExpressions;

namespace NetCoreLab.InterpreterModels
{
    /// <summary>
    /// Helper that exposes several tools to work with Regular expressions.
    /// </summary>
    internal class RegExHelper
    { 
        /// <summary>
        /// Clears all tabs, and line skips from a string.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <returns>Formatted string.</returns>
        public static string ClearStringFormatting(string raw)
        {
            return raw.Replace("\n", "").Replace("\t", "").Replace("\r", "");
        }

        /// <summary>
        /// Trims a string from the start until it reaches a given character.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <param name="target">Stop character.</param>
        /// <returns>Formatted string.</returns>
        public static string TrimUntilChar(string raw, char target)
        {
            var ch = raw[0];
            var result = raw;
            while(ch != target)
            {
                result = result.Remove(0, 1);
                ch = result[0];
            }

            return result;
        }

        /// <summary>
        /// Formats a string to be line divided after special characters.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatLines(string raw)
        {
            return ClearStringFormatting(raw).Replace("}", "}\n").Replace(">", ">\n");
        }

        /// <summary>
        /// Gets a string contained between two other strings.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <param name="start">Starting limiter.</param>
        /// <param name="finish">Ending limiter.</param>
        /// <returns>Formatted string.</returns>
        public static MatchCollection GetContainedString(string raw, string start, string finish)
        {
            var regex = @"(?s)(?<=" + start + ").*?(?=" + finish + ")";
            return Regex.Matches(raw, regex);
        }        
    }
}
