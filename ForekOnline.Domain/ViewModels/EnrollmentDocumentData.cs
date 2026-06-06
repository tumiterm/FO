using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Serializable metadata for a document uploaded before an enrollment job is queued.
    /// </summary>
    public class EnrollmentDocumentData
    {
        public eStudentDocumentType DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
