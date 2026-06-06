// <copyright file="ApplicationRating.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Stores an applicant's feedback about the online application experience.
    /// </summary>
    [Table(nameof(ApplicationRating))]
    public class ApplicationRating
    {
        /// <summary>
        /// Gets or sets the unique rating identifier.
        /// </summary>
        [Key]
        public Guid ApplicationRatingId { get; set; }

        /// <summary>
        /// Gets or sets the application associated with the feedback.
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the rating from one to five stars.
        /// </summary>
        [Range(1, 5)]
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets optional written feedback from the applicant.
        /// </summary>
        [MaxLength(1000)]
        public string? Comment { get; set; }

        /// <summary>
        /// Gets or sets when the feedback was submitted, in UTC.
        /// </summary>
        public DateTime SubmittedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the related application.
        /// </summary>
        [ForeignKey(nameof(ApplicationId))]
        public Application Application { get; set; } = null!;
    }
}
