// <copyright file="LessonPlan.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the LessonPlan class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a lesson plan, including details about the course, module, funding, approval, and associated documents.
    /// </summary>
    [SkipAuditInterceptor]
    public class LessonPlan : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lesson plan record.
        /// </summary>
        [Key]
        public Guid LessonPlanId { get; set; }

        /// <summary>
        /// Gets or sets the identifier or passport associated with the lesson plan.
        /// </summary>
        public string IdPass { get; set; }

        /// <summary>
        /// Gets or sets the reference number or code for the lesson plan.
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the course associated with the lesson plan.
        /// </summary>
        public Guid Course { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the module associated with the lesson plan.
        /// </summary>
        public Guid Module { get; set; }

        /// <summary>
        /// Gets or sets the phase of the lesson plan (e.g., planning, implementation, etc.).
        /// </summary>
        public ePhase? Phase { get; set; }

        /// <summary>
        /// Gets or sets the funding source for the lesson plan (e.g., government, private, etc.).
        /// </summary>
        public eFunder Funder { get; set; }

        /// <summary>
        /// Gets or sets the approval status of the lesson plan (e.g., approved, pending, etc.).
        /// </summary>
        public eSelection Approval { get; set; }

        /// <summary>
        /// Gets or sets the person who approved the lesson plan, if applicable.
        /// </summary>
        public string? IsApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets the reason for approval or rejection of the lesson plan.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Gets or sets the file path or URL of the lesson plan document.
        /// </summary>
        public string? Document { get; set; }
        public string? UploadUrl { get; set; }

        /// <summary>
        /// Gets or sets the uploaded lesson plan document file, not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }
    }
}
