using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Validations
{
    /// <summary>
    /// Defines the ValidationResponse class
    /// Remarks: This class can be used to return potential errors and validation results
    /// </summary>
    public sealed class ValidationResponse
    {
        /// <summary>
        /// Gets or sets the Validity flag
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the error code
        /// Value: the error code.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the Error Description
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// Value: The error description
        /// </summary>
        public ICollection<ValidationError> Errors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an error occured
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message title
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// Gets or sets the return code
        /// </summary>
        public string ReturnCode { get; set; }

        /// <summary>
        /// Initializes a new instances of the ValidationResponse class
        /// </summary>
        public ValidationResponse()
        {
            ErrorCode = 0;
            Errors = new Collection<ValidationError>();
            IsError = false;
        }


        /// <summary>
        /// Initializes a new instance of the ValidationResponse class
        /// </summary>
        /// <param name="errorDescription"> The error description</param>
        public ValidationResponse(string errorDescription) : this()
        {
            ErrorDescription = errorDescription;
            IsError = true;
        }
    }
}
