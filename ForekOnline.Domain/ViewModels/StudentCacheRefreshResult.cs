// <copyright file="StudentCacheRefreshResult.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Result of an explicit legacy API to SQLite cache refresh.
    /// </summary>
    public sealed class StudentCacheRefreshResult
    {
        public int StudentCount { get; set; }
        public int EnrollmentHistoryCount { get; set; }
        public int DetailRecordsLoaded { get; set; }
        public DateTime CompletedUtc { get; set; }
    }
}
