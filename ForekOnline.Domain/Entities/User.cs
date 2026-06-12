// <copyright file="User.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/08/2023 13:18 PM
// Purpose:         Defines the User class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a user entity with personal and authentication details.
    /// </summary>
    [SkipAuditInterceptor]
    public partial class User : Base, ForekOnline.Domain.Shared.ITenantOwned
    {
        public Guid TenantId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        [Display(Name = "Full Name")]
        [MaxLength(25)]
        [MinLength(3)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [Display(Name = "Last Name")]
        [MaxLength(25)]
        [MinLength(3)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user, used as the username.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user's email is verified.
        /// </summary>
        public bool IsEmailVerified { get; set; }

        /// <summary>
        /// Gets or sets the department to which the user belongs.
        /// </summary>
        public eDepartment? Department { get; set; }

        /// <summary>
        /// Gets or sets the activation code for account verification.
        /// </summary>
        public Guid? ActivationCode { get; set; }

        /// <summary>
        /// Gets or sets the reset password code for password recovery.
        /// </summary>
        public string? ResetPasswordCode { get; set; }

        /// <summary>
        /// Gets or sets the last login date of the user.
        /// </summary>
        [Display(Name = "Last Login Date")]
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets the student number of the user, if applicable.
        /// </summary>
        [Display(Name = "StudentId Number")]
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the user.
        /// Must be exactly 10 characters long.
        /// </summary>
        [MaxLength(10)]
        [MinLength(10)]
        public string? Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the identification number or passport number of the user.
        /// Must be between 8 and 13 characters long.
        /// </summary>
        [MaxLength(13)]
        [MinLength(8)]
        [Display(Name = "ID Number / Passport")]
        public string? IDPass { get; set; }

        /// <summary>
        /// Gets or sets the role of the user within the system.
        /// </summary>
        [Display(Name = "Register As")]
        public eSysRole? Role { get; set; }

        /// <summary>
        /// Gets or sets the last activity date of the user.
        /// </summary>
        public DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the URL or path to the user's profile image.
        /// </summary>
        public string? ProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the link to the email signature.
        /// </summary>
        public string? EmailSignatureLink { get; set; } 

        /// <summary>
        /// Gets or sets a value indicating whether the individual is the head of the department.
        /// </summary>
        public bool IsHeadOfDepartment { get; set; } = false;

        /// <summary>
        /// Gets or sets the ProfileImageFile.
        /// </summary>
        [NotMapped]
        [ValidateNever]
        public IFormFile? ProfileImageFile { get; set; }
    }
}
