// <copyright file="UserDetailsViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/08/2023 13:18 PM
// Purpose:         Defines the UserDetailsViewModel class


#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{


    /// <summary>
    /// Represents the details of a user.
    /// </summary>
    public class UserDetailsViewModel : Base
    {
        /// <summary>
        /// Gets or sets StudentNumber of the User.
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets LastLoginDate of the User.
        /// </summary>
        [ValidateNever]
        public DateTime? LastLoginDate { get; set; }


        /// <summary>
        /// Gets or sets LastActivityDate of the User.
        /// </summary>
        [ValidateNever]
        public DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets Id of the User Record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the user.
        /// </summary>
        public string? Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the old password of the user, used for password updates.
        /// </summary>
        [ValidateNever]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password of the user, if applicable.
        /// </summary>
        public string? NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the department of the user.
        /// </summary>
        public eDepartment? Department { get; set; }

        /// <summary>
        /// Gets or sets the role of the user within the system.
        /// </summary>
        public eSysRole? Role { get; set; }

        /// <summary>
        /// Gets or sets the department of the user.
        /// </summary>
        public string? IDPass {  get; set; }

        /// <summary>
        /// Gets or sets the URL or path to the user's profile image.
        /// </summary>
        public string? ProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the ProfileImageFile.
        /// </summary>
        [NotMapped]
        [ValidateNever]
        public IFormFile? ProfileImageFile { get; set; }

        /// <summary>
        /// Gets or sets the link to the email signature.
        /// </summary>
        public string? EmailSignatureLink { get; set; }

        [ValidateNever]
        /// <summary>
        /// Gets or sets the confirmation password, used for validation purposes.
        /// </summary>
        public string ConfirmPassword { get; set; }

        [ValidateNever]
        public bool IsViewingSelf { get; set; }

        [ValidateNever]
        public bool CanDirectDowngrade { get; set; }

        [ValidateNever]
        public bool HasPendingRoleRequest { get; set; }

        [ValidateNever]
        public IReadOnlyList<UserRoleHistory> RoleHistory { get; set; } = [];

        [ValidateNever]
        public IReadOnlyList<RoleAccessRequest> RoleRequests { get; set; } = [];
    }
}
