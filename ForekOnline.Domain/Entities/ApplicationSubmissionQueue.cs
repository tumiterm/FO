// <copyright file="ApplicationSubmissionQueue.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/03/2026 12:09 PM
// Purpose:         Defines the ApplicationSubmissionQueue class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a queue entry for tracking the submission and processing status of an online application.
    /// </summary>
    /// <remarks>This entity is used to manage the lifecycle of application submissions, including their
    /// processing state, retry attempts, and error tracking. It is mapped to the 'ApplicationSubmissionQueue' table in
    /// the 'FO' schema. Typical usage involves monitoring and updating the status of application submissions as they
    /// move through various processing stages.</remarks>
    [Table("ApplicationSubmissionQueue", Schema = "FO")]
    [SkipAuditInterceptor]
    public class ApplicationSubmissionQueue /*: EntityBase<Guid>*/
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated online application user.
        /// </summary>
        public Guid OnlineApplicationUserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the cycle.
        /// </summary>
        public Guid CycleId { get; set; }

        /// <summary>
        /// Gets or sets the current status of the operation.
        /// </summary>
        /// <remarks>Possible values include "Pending", "Processing", "Completed", and "Failed". The
        /// default value is "Pending".</remarks>
        public string Status { get; set; } = "Pending"; // Pending | Processing | Completed | Failed

        /// <summary>
        /// Gets or sets the number of attempts made for the associated operation.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, until which the resource remains locked.
        /// </summary>
        public DateTimeOffset? LockedUntilUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who locked the resource.
        /// </summary>
        public string? LockedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the entity was created.
        /// </summary>
        public DateTimeOffset DateCreatedUtc { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        ///  Gets or sets the date and time, in Coordinated Universal Time (UTC), when the entity was last modified.
        /// </summary>
        public DateTimeOffset? ProcessedUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the acknowledgement was sent.
        /// </summary>
        public DateTimeOffset? AcknowledgementSentUtc { get; set; }

        /// <summary>
        /// Gets or sets the message describing the most recent error that occurred during an operation.
        /// </summary>
        public string? LastError { get; set; }

        #region IEntity
        public string? Code { get; set; }

        public string? Name { get; set; }

        //public byte[] RowVersion { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public DateTimeOffset DateModified { get; set; }

        public string? UserCreated { get; set; }

        public string? UserModified { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset? DateDeleted { get; set; }
        #endregion
    }
}