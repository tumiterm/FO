// <copyright file="StudentImportPayload.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         Hangfire job payload for student import

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Payload for the Hangfire student import job.
    /// </summary>
    public class StudentImportPayload
    {
        /// <summary>
        /// Which data source to read from: "API", "SQLite", or "Direct".
        /// </summary>
        public string Source { get; set; } = "SQLite";

        /// <summary>
        /// When Source = "Direct", this holds the student data submitted from the form.
        /// </summary>
        public DirectStudentData? DirectData { get; set; }

        /// <summary>
        /// If not null, only import this specific student (by ID/Passport).
        /// If null, do a full bulk import.
        /// </summary>
        public string? SingleIdentity { get; set; }

        /// <summary>
        /// The Application.ApplicationId if the student came from an approved application.
        /// </summary>
        public Guid? OriginalApplicationId { get; set; }

        /// <summary>
        /// Course to enroll the student into.
        /// </summary>
        public Guid? CourseId { get; set; }
    }   
}