// <copyright file="ForekBaseModel.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2024 21:00 PM
// Purpose:         Defines the main class.

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a news article on the website.
    /// </summary>
    public class ForekBaseModel
    {
        [Key]

        /// <summary>
        /// Gets or sets the unique identifier for the news article.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the news article.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        /// 
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; }


        /// <summary>
        /// Gets or sets the content of the news article.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        /// 
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the publication date of the news article.
        /// </summary>
        /// <value>
        /// The publication date.
        /// </value>

        [Display(Name = "Publication Date")]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }


        /// <summary>
        /// Gets or sets the author of the news article.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        /// 
        [Display(Name = "Author")]
        public string Author { get; set; }
    }
}
