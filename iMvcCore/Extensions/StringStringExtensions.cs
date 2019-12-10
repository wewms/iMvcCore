using System.Collections.Generic;
using System.Linq;

namespace iMvcCore.Extensions
{
    public static class StringStringExtensions
    {
        /// <summary>
        ///     Split string
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="splitChar">split char</param>
        /// <returns>string array</returns>
        public static IEnumerable<string> SplitString(this string str, char splitChar)
        {
            if(string.IsNullOrEmpty(str)) return null;

            var split = from piece in str.Split(splitChar)
                let trimmed = piece.Trim()
                where !string.IsNullOrEmpty(trimmed)
                select trimmed;

            return split;
        }

        /// <summary>
        ///     Format string
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string target, params object[] args)
        {
            return string.IsNullOrEmpty(target) ? string.Empty : string.Format(target, args);
        }
    }
}