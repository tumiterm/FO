// <copyright file="ApplicationsViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the ApplicationsViewModel class


using ForekOnline.Domain.Entities;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for an application, containing details about the applicant and their submission.
    /// </summary>
    public class ApplicationsViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the application.
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the course for which the application is submitted.
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// Gets or sets the current status of the application.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the full name of the applicant.
        /// </summary>
        public string Names { get; set; }

        /// <summary>
        /// Gets or sets the email address of the applicant.
        /// This property is optional.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the reference number associated with the application.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the identification number of the applicant.
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the applicant.
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the file path or reference to the applicant's identification document (ID, passport, or other relevant document).
        /// </summary>
        public string IDPassDoc { get; set; }

        /// <summary>
        /// Gets or sets the file path or reference to the applicant's qualification document.
        /// </summary>
        public string QualificationDoc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the applicant should receive SMS notifications.
        /// </summary>
        public bool IsSMS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the applicant should receive email notifications.
        /// </summary>
        public bool IsEmail { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the application was submitted.
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Gets or sets the address of the applicant.
        /// </summary>
        public Address ApplicantAddress { get; set; }

        /// <summary>
        /// Gets or sets the guardian details of the applicant.
        /// </summary>
        public Guardian ApplicantGuardian { get; set; }
    }

}
