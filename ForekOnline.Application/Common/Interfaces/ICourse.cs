// <copyright file="ICourse.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:03 PM
// Purpose:         Defines the ICourse interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a contract for managing and updating course entities in the repository.
    /// </summary>
    /// <remarks>This interface extends the <see cref="IRepository{T}"/> interface, providing additional
    /// functionality specific to courses.</remarks>
    public interface ICourse : IRepository<Course>
    {
        /// <summary>
        /// Updates the details of an existing course asynchronously.
        /// </summary>
        /// <param name="course">The <see cref="Course"/> object containing the updated course details. The course must have a valid
        /// identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Course"/>
        /// object.</returns>
        Task<Course> UpdateCourseAsync(Course course);

    }
}
