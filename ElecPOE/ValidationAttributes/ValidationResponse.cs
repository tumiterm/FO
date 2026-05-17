// <copyright file="ValidationResponse.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    9/02/2024 13:09:27 PM
// Purpose:         Defines the ValidationResponse class

namespace ElecPOE.ValidationAttributes
{
    /// <summary>
    /// Represents the result of a validation operation.
    /// </summary>
    public class ValidationResponse
    {
        /// <summary>
        /// Indicates whether the validation encountered an error.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// The message associated with the validation result.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The type of error encountered (e.g., NullCheck, ModelState, etc.).
        /// </summary>
        public string ErrorType { get; set; }

        /// <summary>
        /// The URL to redirect to in case of an error.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Factory method to create a success response.
        /// </summary>
        public static ValidationResponse Success() => new ValidationResponse { IsError = false };

        /// <summary>
        /// Factory method to create an error response.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errorType">The type of error.</param>
        /// <param name="redirectUrl">The URL to redirect to if needed.</param>
        public static ValidationResponse Error(string message, string errorType, string redirectUrl = null) =>
            new ValidationResponse
            {
                IsError = true,
                Message = message,
                ErrorType = errorType,
                RedirectUrl = redirectUrl
            };
    }

}
