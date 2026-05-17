// <copyright file="CourseUpdateRequest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/03/2026 11:00 AM
// Purpose:         Defines the CourseUpdateRequest class

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a request to update the details of an existing course, including its metadata, type, funding status,
    /// and associated modules.
    /// </summary>
    /// <remarks>Use this type to supply updated information for a course in scenarios such as administrative
    /// course management or batch updates. All required fields must be provided to ensure the update is processed
    /// correctly. Optional properties may be omitted if no change is needed for those values.</remarks>
    public class CourseUpdateRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        public eCourseType Type { get; set; }

        /// <summary>
        /// Gets or sets the node type for the current entity.
        /// </summary>
        public eNType? NType { get; set; }

        /// <summary>
        /// Gets or sets the National Qualifications Framework (NQF) level associated with this entity.
        /// </summary>
        public eNQF? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the item.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course is funded.
        /// </summary>
        public bool? IsFunded { get; set; }

        /// <summary>
        /// Gets or sets the collection of modules associated with the course.
        /// </summary>
        public IEnumerable<ModuleUpsertRequest>? Modules { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the course.
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the concurrency stamp for the course.
        /// </summary>
        public string? ConcurrencyStamp { get; set; }
    }
}
