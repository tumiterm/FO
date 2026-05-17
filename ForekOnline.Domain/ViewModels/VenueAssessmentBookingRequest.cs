using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Request model for creating a venue assessment booking (Stage 2 – FR-04).
    /// </summary>
    public class VenueAssessmentBookingRequest
    {
        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid ModuleId { get; set; }

        [Required]
        [StringLength(200)]
        public string AssessmentName { get; set; } = string.Empty;

        public eAssessmentType? AssessmentType { get; set; }

        [StringLength(2000)]
        public string? Instructions { get; set; }

        public int? DurationMinutes { get; set; }

        /// <summary>
        /// List of student ID/Passport numbers to send the assessment to.
        /// </summary>
        [Required]
        public List<string> StudentIdentifiers { get; set; } = new();
    }
}