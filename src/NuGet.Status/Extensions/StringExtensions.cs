// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Linq;

namespace NuGet.Status.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a string that can be used as the value of an HTML id attribute.
        /// </summary>
        public static string ToHtmlSafeId(this string input)
        {
            return GetHtmlSafeFirstChar(input.First()) +
                string.Concat(input
                    .Skip(1)
                    .Select(GetHtmlSafeChar));
        }

        private static char GetHtmlSafeChar(char c)
        {
            return IsHtmlSafeChar(c) ? c : '_';
        }

        /// <remarks>
        /// The value of an HTML id attribute must only contain alphanumeric characters, hyphens, and underscores.
        /// </remarks>
        private static bool IsHtmlSafeChar(char c)
        {
            return IsInCharRange(c, '0', '9') || 
                c == '-' || 
                c == '_' || 
                IsHtmlSafeFirstChar(c);
        }

        private static char GetHtmlSafeFirstChar(char c)
        {
            return IsHtmlSafeFirstChar(c) ? c : 'x';
        }

        /// <remarks>
        /// The first character of the value of an HTML id attribute must be alphabetic.
        /// </remarks>
        private static bool IsHtmlSafeFirstChar(char c)
        {
            return IsInCharRange(c, 'a', 'z') || IsInCharRange(c, 'A', 'Z');
        }

        private static bool IsInCharRange(char target, char lower, char upper)
        {
            return target >= lower && target <= upper;
        }
    }
}