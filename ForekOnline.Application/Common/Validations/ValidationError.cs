using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Validations
{
    /// <summary>
    /// Defines the ValidationError class
    /// Remarks: This class is used to return validation errors
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Gets or sets the code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the parent path
        /// </summary>
        public string ParentPath { get; set; }

        /// <summary>
        /// Initializes a new instance of ValidationError class
        /// </summary>
        public ValidationError()
        {
            Code = 0;
        }

        /// <summary>
        /// Initializes a new instance of ValidationError class
        /// </summary>
        /// <param name="message">The message</param>
        public ValidationError(string message)
        {
            Message = message;
        }
    }
}
