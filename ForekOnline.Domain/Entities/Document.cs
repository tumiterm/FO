// <copyright file="Document.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the Document class

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
    /// Represents a document entity with details about its request, approval, and issuance.
    /// </summary>
    [SkipAuditInterceptor]
    public class Document : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the document.
        /// </summary>
        [Key]
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier for the document.
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the type of document.
        /// </summary>
        public eDocumentType DocumentType { get; set; }

        /// <summary>
        /// Gets or sets the date when the document was requested.
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the date when the document was returned, if applicable.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student associated with the document request.
        /// </summary>
        [ForeignKey(nameof(Student))]
        public Guid? Student { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who requested the document.
        /// </summary>
        [ForeignKey(nameof(User))]
        public Guid RequestedBy { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the module associated with the document request.
        /// </summary>
        [ForeignKey(nameof(Module))]
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course associated with the document request.
        /// </summary>
        [ForeignKey(nameof(Course))]
        public Guid? CourseId { get; set; }

        /// <summary>
        /// Gets or sets the department related to the document request.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the designation of the requester in the system.
        /// </summary>
        public eSysRole Designation { get; set; }

        /// <summary>
        /// Gets or sets the quantity of documents requested.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the purpose for requesting the document.
        /// </summary>
        public string RequestPurpose { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who approved the request.
        /// </summary>
        public Guid? ApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document was issued via email.
        /// </summary>
        public bool IsEmailIssued { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a hard copy of the document was issued.
        /// </summary>
        public bool IsHardCopyIssued { get; set; }

        /// <summary>
        /// Gets or sets the uploaded document file path, if applicable.
        /// </summary>
        public string? DocumentUpload { get; set; }

        /// <summary>
        /// Indicates whether the document has been returned.
        /// </summary>
        public bool IsReturned = false;

        /// <summary>
        /// Gets or sets the selected student IDs associated with the document request.
        /// </summary>
        public string? SelectedStudentIDs { get; set; }

        /// <summary>
        /// Gets or sets an array of selected student IDs, not mapped to the database.
        /// </summary>
        [NotMapped]
        public string[] SelectedIDArray { get; set; }

        /// <summary>
        /// Gets or sets the uploaded document file, not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }
    }

}
