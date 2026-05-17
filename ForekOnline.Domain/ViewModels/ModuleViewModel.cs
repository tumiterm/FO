// <copyright file="ModuleViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 19:09:27 PM
// Purpose:         Defines the ModuleViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Data Transfer Object for Module details.
    /// </summary>
    public record ModuleViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the module.
        /// </summary>
        [Key]
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the module name.
        /// </summary>
        public string? ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the associated course.
        /// </summary>
        public Guid CourseIdFK { get; set; }

        /// <summary>
        /// Gets or sets the NQF Level.
        /// </summary>
        public eNQF? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the Credits.
        /// </summary>
        public double? Credit { get; set; }

        /// <summary>
        /// Gets or sets the Active flag.
        /// </summary>
        public bool IsActive { get; set; }


        /// <summary>
        /// The navigation property to load associated Course.
        /// </summary>

        [ValidateNever]
        public Course Course { get; set; }
    }
}
