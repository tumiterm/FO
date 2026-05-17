// <copyright file="UserViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/08/2023 13:18 PM
// Purpose:         Defines the UserViewModel class


using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a user, including login details and preferences.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the student number (used as the email for authentication).
        /// </summary>
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID required!")]
        public string StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the password for the user's login.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the "Remember Me" option is selected.
        /// </summary>
        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the URL to redirect to after successful login.
        /// </summary>
        public string? ReturnUrl { get; set; }
    }

}
