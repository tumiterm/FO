// <copyright file="LessonRepository.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 16:05 PM
// Purpose:         Defines the LessonRepository repository

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository for managing lesson entities within the application data store.
    /// </summary>
    /// <remarks>The LessonRepository provides data access and persistence operations for Lesson objects,
    /// implementing the ILessons interface. It extends the generic Repository base class to offer additional
    /// functionality specific to lessons. This class is intended to be used within the application's data access layer
    /// and should be instantiated with a valid ApplicationDbContext.</remarks>
    public class LessonRepository : Repository<Lesson>, ILessons
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LessonRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public LessonRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing lesson model in the repository.
        /// </summary>
        /// <param name="lesson">The lesson model to be updated.</param>
        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Update(lesson);

            await _context.SaveChangesAsync();

            return lesson;
        }
    }
}
