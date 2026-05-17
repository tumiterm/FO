using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForekOnline.Domain.Entities
{
    [Table("BackgroundJobQueueItem", Schema = "FO")]
    [SkipAuditInterceptor]
    public class BackgroundJobQueueItem 
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Logical queue name (e.g. "onlineapps").
        /// Lets you have separate drainers per subsystem.
        /// </summary>
        public string Queue { get; set; } = "default";

        /// <summary>
        /// Task type discriminator (e.g. "OnlineApplicationSubmission").
        /// Future-proof for additional workflows.
        /// </summary>
        public string JobType { get; set; } = string.Empty;

        /// <summary>
        /// JSON payload for the handler.
        /// </summary>
        public string PayloadJson { get; set; } = "{}";

        public string Status { get; set; } = "Pending"; // Pending | Processing | Completed | Failed

        public int Attempts { get; set; }

        public DateTimeOffset? LockedUntilUtc { get; set; }

        public string? LockedBy { get; set; }

        public DateTimeOffset? ProcessedUtc { get; set; }

        public string? LastError { get; set; }

        #region IEntity
        public string? Code { get; set; }
        public string? Name { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public string? UserCreated { get; set; }
        public string? UserModified { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DateDeleted { get; set; }
        #endregion
    }
}