// <copyright file="StudentCacheStatusViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Describes the contents and most recent refresh of the SQLite student cache.
    /// </summary>
    public sealed class StudentCacheStatusViewModel
    {
        public int StudentCount { get; set; }
        public int EnrollmentHistoryCount { get; set; }
        public DateTime? LastSyncUtc { get; set; }
        public bool? LastSyncWasSuccessful { get; set; }
        public int? LastSyncStudentCount { get; set; }
        public string DatabasePath { get; set; } = string.Empty;
    }
}
