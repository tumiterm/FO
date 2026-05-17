// <copyright file="CourseModuleViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the CourseModuleViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for a course module.
    /// </summary>
    public class CourseModuleViewModel : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the module.
        /// </summary>
        [Key]
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        public string? ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the National Qualifications Framework (NQF) level of the module.
        /// </summary>
        public eNQF? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the N-type classification of the module.
        /// </summary>
        public eNType? NType { get; set; }

        /// <summary>
        /// Gets or sets the NQF level of the associated course.
        /// </summary>
        public eNQF? CourseNQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the credit value of the module.
        /// </summary>
        public double? Credit { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the total credit value of the course.
        /// </summary>
        public double? CourseCredit { get; set; }

        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        public eCourseType Type { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the course type.
        /// </summary>
        public Guid CourseTypeFK { get; set; }

        /// <summary>
        /// Gets or sets the list of modules associated with the course.
        /// </summary>
        public List<ForekOnline.Domain.Entities.Module>? Module { get; set; }

        /// <summary>
        /// Gets or sets the number of modules
        /// </summary>
        public int ModuleCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course is eligible for online applications.
        /// </summary>
        [ValidateNever]
        public bool IsEligibleForOnlineApplications { get; set; } = false;
        /// <summary>
        /// Gets or sets the minimum requirements for enrolling in the course.
        /// </summary>
        public eMinimumRequirement MinimumRequirement { get; set; }
    }

}
