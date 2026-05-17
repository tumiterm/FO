// <copyright file="CourseSearchResult.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/03/2026 11:00 AM
// Purpose:         Defines the CourseSearchResult class

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents the result of a course search, containing summary information about a course.
    /// </summary>
    public class CourseSearchResult
    {
        /// <summary>
        /// Gets or sets the unique identifier of the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        public eCourseType Type { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the item.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the National Qualifications Framework (NQF) level associated with this entity.
        /// </summary>
        public eNQF? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is fully funded.
        /// </summary>
        public bool IsFunded { get; set; }

        /// <summary>
        /// Gets or sets the credit amount associated with the account.
        /// </summary>
        public double? Credit { get; set; }

        /// <summary>
        /// Gets or sets the number of modules included in the collection.
        /// </summary>
        public int ModuleCount { get; set; }
    }
}
