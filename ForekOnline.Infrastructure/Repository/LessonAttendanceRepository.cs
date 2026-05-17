// <copyright file="LessonAttendanceRepository.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 16:05 PM
// Purpose:         Defines the LessonAttendanceRepository repository

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides data access and management operations for lesson attendance records.
    /// </summary>
    /// <remarks>This repository implements functionality for updating and managing lesson attendance entities
    /// within the application's data store. It is intended to be used as part of the application's data access layer
    /// and is typically registered for dependency injection. All operations are performed using the provided
    /// application database context.</remarks>
    public class LessonAttendanceRepository : Repository<LessonAttendance>, ILessonAttendance
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LessonAttendanceRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public LessonAttendanceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing lessonAttendance model in the repository.
        /// </summary>
        /// <param name="lessonAttendance">The lessonAttendance model to be updated.</param>
        public async Task<LessonAttendance> UpdateLessonAttendanceAsync(LessonAttendance lessonAttendance)
        {
            _context.LessonAttendances.Update(lessonAttendance);

            await _context.SaveChangesAsync();

            return lessonAttendance;
        }
    }
}
