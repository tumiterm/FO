// <copyright file="Company.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    27/10/2025 11:00 AM
// Purpose:         Defines the CourseCreateRequest class

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a request to create a new course with specified details.
    /// </summary>
    /// <remarks>This class is used to encapsulate all necessary information required to create a course,
    /// including its name, type, optional category, and associated modules. It also tracks metadata such as the creator
    /// and any modifications.</remarks>
    public class CourseCreateRequest
    {
        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        public eCourseType Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity, represented as an <see cref="eNType"/> enumeration.
        /// </summary>
        public eNType? NType { get; set; }

        /// <summary>
        /// Gets or sets the National Qualifications Framework (NQF) level.
        /// </summary>
        public eNQF? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the item.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the project is funded.
        /// </summary>
        public bool IsFunded { get; set; }

        /// <summary>
        /// Gets or sets the collection of module upsert requests.
        /// </summary>
        public IEnumerable<ModuleUpsertRequest>? Modules { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the credit amount associated with the account.
        /// </summary>
        public int Credit { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified the entity.
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the entity.
        /// </summary>
        public string? LastUpdatedBy { get; set; }
    }
}
