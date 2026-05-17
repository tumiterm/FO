// <copyright file="AssessmentController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    02/01/2023 15:09:27 PM
// Purpose:         Defines the AssessmentAttachment class

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
    /// Represents an attachment related to an assessment.
    /// </summary
    [SkipAuditInterceptor]
    public class AssessmentAttachment : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the attachment.
        /// </summary>
        [Key]
        public Guid AttachmentId { get; set; }

        /// <summary>
        /// Gets or sets the document associated with the assessment attachment.
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// Gets or sets the student number.
        /// </summary>
        [Display(Name = "StudentId Number")]
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student.
        /// </summary>
        public Guid? StudentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course.
        /// </summary>
        public Guid? CourseId { get; set; }

        /// <summary>
        /// Gets or sets the module associated with the assessment attachment.
        /// </summary>
        public eModule? Module { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the module.
        /// </summary>
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the percentage score of the assessment.
        /// </summary>
        public double? Percentage { get; set; }

        /// <summary>
        /// Gets or sets the number of attempts made for the assessment.
        /// </summary>
        public eAttempts Attempts { get; set; }

        /// <summary>
        /// Gets or sets the type of assessment administration.
        /// </summary>
        public eAssessmentAdministration Type { get; set; }

        /// <summary>
        /// Gets or sets the file (Assessment) url.
        /// </summary>
        public string? FileURL { get; set; }

        /// <summary>
        /// Gets or sets the file uploaded as an attachment.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        [Display(Name = "File")]
        public IFormFile? AttachmentFile { get; set; }
    }
}
