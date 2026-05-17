// <copyright file="Module.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 19:09:27 PM
// Purpose:         Defines the Module class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents the module entity containing details about the modules from a related course.
    /// </summary>
    [SkipAuditInterceptor]
    public class Module
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
        public Course Course { get; set; }
    }
}
