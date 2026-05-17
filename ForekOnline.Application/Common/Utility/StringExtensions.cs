// <copyright file="StringExtensions.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    10/03/2025 15:36
// Purpose:         Defines the StringExtensions class

namespace ForekOnline.Application.Common.Utility
{
    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Truncates the given string to the specified maximum length and appends an ellipsis ("...") if truncation occurs.
        /// </summary>
        /// <param name="value">The input string to truncate.</param>
        /// <param name="maxLength">The maximum allowed length of the string, including the ellipsis. Default is 30.</param>
        /// <returns>The truncated string with an ellipsis if truncation occurs; otherwise, the original string.</returns>
        /// <exception cref="ArgumentException">Thrown when maxLength is less than 3.</exception>
        public static string TruncateWithEllipsis(this string value, int maxLength = 30)
        {
            if(string.IsNullOrEmpty(value)) return value;
            if (maxLength < 3) throw new ArgumentException("Max lenght too short");

            if(value.Length <= maxLength) return value;

            int charsToKeep = maxLength - 3;
            return $"{value.Substring(0,charsToKeep)}...";
        }

        /// <summary>
        /// Truncates the given string to the specified maximum length, attempting to break at the last space before truncation.
        /// Appends an ellipsis ("...") if no suitable break point is found.
        /// </summary>
        /// <param name="value">The input string to truncate.</param>
        /// <param name="maxLength">The maximum allowed length of the string, including the ellipsis. Default is 30.</param>
        /// <returns>The truncated string, breaking at the last space if possible; otherwise, the truncated string with an ellipsis.</returns>
        /// <exception cref="ArgumentException">Thrown when maxLength is less than 3.</exception>
        public static string TruncateWithEllipsisSmart(this string value, int maxLength = 30)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (maxLength < 3) throw new ArgumentException("Max lenght too short");

            if (value.Length <= maxLength) return value;

            int lastSpace = value.Substring(0, maxLength - 3).LastIndexOf(' ');

            if(lastSpace <= 0)
            {
                return $"{value.Substring(0, maxLength - 3)}...";
            }

            return $"{value.Substring(0, lastSpace)}...";
        }
    }
}
