// <copyright file="ApplicationRatingViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Captures feedback about the online application submission experience.
    /// </summary>
    public class ApplicationRatingViewModel
    {
        /// <summary>
        /// Gets or sets the application being rated.
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the selected star rating.
        /// </summary>
        [Range(1, 5, ErrorMessage = "Please select a rating from one to five stars.")]
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets optional written feedback.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Feedback cannot exceed 1000 characters.")]
        public string? Comment { get; set; }
    }
}
