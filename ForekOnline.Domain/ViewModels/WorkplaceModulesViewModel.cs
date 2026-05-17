
// <copyright file="WorkplaceModulesViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the WorkplaceModulesViewModel class

using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the workplace module details associated with a learner.
    /// </summary>
    public record WorkplaceModulesViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the learner's workplace module.
        /// </summary>
        public Guid LearnerWorkplaceModulesId { get; set; }

        /// <summary>
        /// Gets or sets the name of the course associated with the module.
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the module.
        /// </summary>
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets the name of the student assigned to the module.
        /// </summary>
        public string? Student { get; set; }

        /// <summary>
        /// Gets or sets the number of days allocated for the module.
        /// </summary>
        public int? Days { get; set; }

        /// <summary>
        /// Gets or sets the progress status of the module.
        /// </summary>
        public eStatus? Progress { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who registered the module.
        /// </summary>
        public string? RegisteredBy { get; set; }

        /// <summary>
        /// Gets or sets the start date of the module.
        /// </summary>
        public string? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the module.
        /// </summary>
        public string? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who last modified the module.
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the placement associated with the module.
        /// </summary>
        public Guid PlacementId { get; set; }

        /// <summary>
        /// Gets or sets the date when the module was last modified.
        /// </summary>
        public string? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the module is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the name of the company associated with the module.
        /// </summary>
        public string? Company { get; set; }
    }

}
