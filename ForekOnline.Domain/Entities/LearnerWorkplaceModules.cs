// <copyright file="LearnerWorkplaceModules.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the LearnerWorkplaceModules class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a learner's module in a workplace setting, including details on course, module, placement, and progress.
    /// </summary>
    [SkipAuditInterceptor]
    public class LearnerWorkplaceModules : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the learner's workplace module record.
        /// </summary>
        [Key]
        public Guid LearnerWorkplaceModulesId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the associated course.
        /// </summary>
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the associated module.
        /// </summary>
        [ForeignKey("Module")]
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the student name associated with the workplace module.
        /// </summary>
        public string? Student { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the associated placement.
        /// </summary>
        [ForeignKey("Placement")]
        public Guid PlacementId { get; set; }

        /// <summary>
        /// Gets or sets the number of days the learner spent on the workplace module.
        /// </summary>
        public int? Days { get; set; }

        /// <summary>
        /// Gets or sets the progress status of the learner in the workplace module.
        /// </summary>
        public eStatus? Progress { get; set; }

        /// <summary>
        /// Gets or sets the start date of the learner's workplace module.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the learner's workplace module.
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

}
