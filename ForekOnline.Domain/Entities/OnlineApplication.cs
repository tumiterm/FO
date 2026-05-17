// <copyright file="OnlineApplication.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/02/2026 22:17 PM
// Purpose:         Defines the OnlineApplication class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an online application submitted by a student for a specific academic cycle, including course
    /// preferences, status, and turnaround tracking information.
    /// </summary>
    /// <remarks>This entity contains details about the applicant, selected courses, application status, and
    /// timing information relevant to processing and turnaround time evaluation. It is mapped to the
    /// 'OnlineApplication' table in the 'FO' schema. The class implements IEntity<Guid> to provide standard entity
    /// metadata such as creation and modification timestamps, user tracking, and soft deletion support.</remarks>
    [Table("OnlineApplication", Schema = "FO")]
    [SkipAuditInterceptor]
    public class OnlineApplication /*: EntityBase<Guid>*/
    {
        /// <summary>
        /// Gets or sets the unique identifier for the applicant.
        /// </summary>
        [Required]
        public Guid ApplicantId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the cycle.
        /// </summary>
        [Required]
        public Guid CycleId { get; set; }

        /// <summary>
        /// Gets or sets the student number assigned to the applicant for this application. 
        /// </summary>
        [MaxLength(20)]
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the academic year associated with the entity.
        /// </summary>
        public int AcademicYear { get; set; }

        /// <summary>
        /// Gets or sets the code representing the type of course.
        /// </summary>
        /// <remarks>Valid values are "SS", "OC", or "NQ". The code must be a string with a maximum length
        /// of 2 characters.</remarks>
        [MaxLength(2)]
        public string CourseTypeCode { get; set; } = string.Empty; // SS|OC|NQ

        /// <summary>
        /// Gets or sets the code that indicates the funding type for the transaction.
        /// </summary>
        /// <remarks>Valid values are 'P' for private funding and 'F' for funded. The property
        /// does not enforce value constraints; callers should ensure that only supported codes are used.</remarks>
        public char FundingTypeCode { get; set; } // P|F

        /// <summary>
        /// Gets or sets the unique identifier of the applicant's first-choice course.
        /// </summary>
        public Guid? FirstChoiceCourseId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the applicant's second-choice course selection.
        /// </summary>
        public Guid? SecondChoiceCourseId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the third-choice course selected by the user.
        /// </summary>
        public Guid? ThirdChoiceCourseId { get; set; }

        /// <summary>
        /// Gets or sets the current status of the item.
        /// </summary>
        /// <remarks>The status is limited to a maximum of 30 characters. Typical values may include
        /// "Draft", "Published", or other custom states as defined by the application.</remarks>
        [MaxLength(30)]
        public string Status { get; set; } = "Draft";

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the item was submitted.
        /// </summary>
        public DateTime? SubmittedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the entity was last saved.
        /// </summary>
        public DateTime LastSavedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the turnaround time, in days, for the associated process or item.
        /// </summary>
        public int? TatDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the turnaround time has been exceeded.
        /// </summary>
        public bool HasExceededTurnaroundTime { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the turnaround time was last evaluated.
        /// </summary>
        public DateTime? LastTatEvaluatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when an overdue notification was sent.
        /// </summary>
        public DateTime? OverdueNotifiedOnUtc { get; set; }

        #region IEntity

        /// <summary>
        /// Gets or sets the unique code that identifies the entity.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the database row version used for concurrency control.
        /// </summary>
        /// <remarks>The row version is typically a timestamp or binary value generated by the database to
        /// detect changes to a record. Use this property when implementing optimistic concurrency to ensure updates are
        /// not overwritten by other operations.</remarks>
       // public byte[] RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the item was last modified.
        /// </summary>
        public DateTimeOffset DateModified { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who created the entity.
        /// </summary>
        public string? UserCreated { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who last modified the entity.
        /// </summary>
        public string? UserModified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been marked as deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was deleted.
        /// </summary>
        public DateTimeOffset? DateDeleted { get; set; }
        #endregion
    }
}
