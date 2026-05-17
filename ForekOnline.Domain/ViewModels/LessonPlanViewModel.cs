// <copyright file="LessonPlanViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the LessonPlanViewModel class


#region Usings
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
using Microsoft.AspNetCore.Mvc.Rendering;
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a lesson plan within a course module.
    /// </summary>
    public class LessonPlanViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lesson plan.
        /// </summary>
        public Guid LessonPlanId { get; set; }

        /// <summary>
        /// Gets or sets the reference for the lesson plan.
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the identifier or password associated with the lesson plan.
        /// </summary>
        public string IdPass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the lesson plan is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated course.
        /// </summary>
        public Guid Course { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated module.
        /// </summary>
        public Guid Module { get; set; }

        /// <summary>
        /// Gets or sets the phase associated with the lesson plan.
        /// </summary>
        public ePhase? Phase { get; set; }

        /// <summary>
        /// Gets or sets the funder for the lesson plan.
        /// </summary>
        public eFunder Funder { get; set; }

        /// <summary>
        /// Gets or sets the approval status of the lesson plan.
        /// </summary>
        [Display(Name = "IsApproved")]
        public eSelection Approval { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who approved the lesson plan.
        /// </summary>
        public string? IsApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets the reason for approval or rejection.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Gets or sets the document associated with the lesson plan.
        /// </summary>
        public string? Document { get; set; }
        public string? UploadUrl { get; set; }

        /// <summary>
        /// Gets or sets the uploaded document file for the lesson plan (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile DocumentFile { get; set; }

        #region NEW*

        [ValidateNever]
        public SelectList Courses { get; set; }

        [ValidateNever]
        public SelectList Modules { get; set; }

        [ValidateNever]
        public IReadOnlyList<LessonPlan> ExistingPlans { get; set; }

        [ValidateNever]
        public string UserDetail { get; set; }
        #endregion
    }

}
