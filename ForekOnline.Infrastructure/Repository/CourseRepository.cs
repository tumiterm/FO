//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    26/Feb/2025 18:53 PM
// Purpose:         Defines the CourseRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Infrastructure.Repository
{

    /// <summary>
    /// Represents a repository specifically for performing operations on the Course Repository.
    /// </summary>
    public class CourseRepository : Repository<Course>, ICourse
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseRepository"/> class.
        /// </summary>
        /// <param name="context">The Course database context.</param>
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Course model in the repository.
        /// </summary>
        /// <param name="course">The Course model to be updated.</param>
        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _context.Course.Update(course);

            await _context.SaveChangesAsync();

            return course;
        }
    }
}
