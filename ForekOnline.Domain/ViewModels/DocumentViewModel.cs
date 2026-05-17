// <copyright file="DocumentViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/02/2025 19:54 PM
// Purpose:         Defines the DocumentViewModel class.


#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Http;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a document data transfer object (DTO) for handling document-related requests.
    /// </summary>
    public class DocumentViewModel : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the document.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the reference associated with the document.
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
        /// Gets or sets the return date of the document, if applicable.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student associated with the document, if applicable.
        /// </summary>
        public Guid? StudentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the person who requested the document.
        /// </summary>
        public Guid RequestedBy { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of selected student IDs.
        /// </summary>
        public string? SelectedStudentIDs { get; set; }

        /// <summary>
        /// Gets or sets an array of selected student IDs.
        /// </summary>
        public string[] SelectedIDArray { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the module associated with the document, if applicable.
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course associated with the document, if applicable.
        /// </summary>
        public Guid? CourseId { get; set; }

        /// <summary>
        /// Gets or sets the department requesting or handling the document.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the designation associated with the document request.
        /// </summary>
        public eSysRole Designation { get; set; }

        /// <summary>
        /// Gets or sets the quantity of documents requested.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the document request.
        /// </summary>
        public string RequestPurpose { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the person who approved the document request, if applicable.
        /// </summary>
        public Guid? ApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document has been issued via email.
        /// </summary>
        public bool IsEmailIssued { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document has been issued as a hard copy.
        /// </summary>
        public bool IsHardCopyIssued { get; set; }

        /// <summary>
        /// Gets or sets the path or reference to the uploaded document.
        /// </summary>
        public string? DocumentUpload { get; set; }

        /// <summary>
        /// Indicates whether the document has been returned.
        /// </summary>
        public bool IsReturned = false;

        /// <summary>
        /// Gets or sets the uploaded document file.
        /// </summary>
        public IFormFile? DocumentFile { get; set; }
    }

}
