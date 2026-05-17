// <copyright file="CourseStatsResult.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/03/2026 11:00 AM
// Purpose:         Defines the CourseStatsResult class

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents statistical information and key attributes for a specific course.
    /// </summary>
    /// <remarks>This class is typically used to convey summary data about a course, such as its identifier,
    /// credit value, number of active modules, NQF level, funding status, and the last time the statistics were
    /// updated. All properties are intended for data transfer and reporting purposes.</remarks>
    public class CourseStatsResult
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the total number of credits associated with the entity.
        /// </summary>
        public double? TotalCredits { get; set; }

        /// <summary>
        /// Gets or sets the number of modules that are currently active.
        /// </summary>
        public int ActiveModules { get; set; }

        /// <summary>
        /// Gets or sets the National Qualifications Framework (NQF) level associated with this entity.
        /// </summary>
        public eNQF? NqfLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been funded.
        /// </summary>
        public bool IsFunded { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the entity was last updated.
        /// </summary>
        public DateTime? LastUpdatedUtc { get; set; }
    }
}
