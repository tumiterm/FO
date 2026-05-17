
#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Request model for creating a new venue reservation (Stage 1).
    /// </summary>
    public class VenueReservationCreateRequest
    {
        [Required]
        public Guid VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string Campus { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int ExpectedStudents { get; set; }

        [Required]
        public DateTime ReservedDate { get; set; }

        [Required]
        public DateTime StartTimeUtc { get; set; }

        [Required]
        public DateTime EndTimeUtc { get; set; }
    }
}