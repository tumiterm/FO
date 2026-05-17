// <copyright file="User.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/08/2023 13:18 PM
// Purpose:         Defines the User class

using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a user entity with metadata annotations.
    /// </summary>
  
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
        /// <summary>
        /// Gets or sets the confirmation password, used for validation purposes.
        /// </summary>
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Contains metadata for the <see cref="User"/> class, defining validation attributes.
    /// </summary>
    public class UserMetaData
    {
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name required")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [Display(Name = "Phone")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number is required")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        [MinLength(10)]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation password, ensuring it matches the original password.
        /// </summary>
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password and password do not match!")]
        public string ConfirmPassword { get; set; }
    }
}
