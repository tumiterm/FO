// <copyright file="MaxFileSizeAttribute.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    9/02/2024 13:09:27 PM
// Purpose:         Defines the MaxFileSizeAttribute class

using System.ComponentModel.DataAnnotations;

namespace ElecPOE.ValidationAttributes
{

    /// <summary>
    /// Attribute to specify the maximum allowable file size for an uploaded file.
    /// </summary>
    public class MaxFileSizeAttribute : ValidationAttribute
    {

        private readonly int _maxFileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxFileSizeAttribute"/> class.
        /// </summary>
        /// <param name="maxFileSize">The maximum allowable file size in megabytes.</param>
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        /// <summary>
        /// Validates the specified file to ensure it does not exceed the maximum allowable size.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> that contains the result of the validation.
        /// If the file is valid, <c>null</c> is returned.
        /// If the file exceeds the maximum allowable size, an error message is returned.
        /// </returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            
            if (file != null) 
            {
                if(file.Length > (1000000 * _maxFileSize))
                {
                    return new ValidationResult("File Size is too large");
                }
            }

            return base.IsValid(value, validationContext);
        }
    }
}
