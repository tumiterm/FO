using System;

using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.Entities
{
    public class StudentDocument
    {
        #region Identity
        public Guid StudentDocumentId { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        #endregion

        #region Document Type
        public eStudentDocumentType DocumentType { get; set; }
        #endregion

        #region File Info 
        public string FileName { get; set; } = string.Empty;       // original file name
        public string StoredFileName { get; set; } = string.Empty; // GUID-based name on disk/blob
        public string FilePath { get; set; } = string.Empty;       // relative or blob URL
        public string ContentType { get; set; } = string.Empty;    // e.g. "application/pdf"
        public long FileSizeBytes { get; set; }

        #endregion

        #region Validity 
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? ExpiryDate { get; set; }                  // passport/permit expiry
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
        #endregion
    }
}
