//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    24/Feb/2025 22:41 PM
// Purpose:         Defines the LessonPlanRepository.

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
    /// Represents a repository specifically for performing operations on the Lesson-Plan Repository.
    /// </summary>
    public class LessonPlanRepository : Repository<LessonPlan>, ILessonPlans
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;


        /// <summary>
        /// Initializes a new instance of the <see cref="LessonPlanRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public LessonPlanRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        /// <summary>
        /// Updates an existing Application model in the repository.
        /// </summary>
        /// <param name="lessonPlan">The lessonPlan model to be updated.</param>
        public async Task<LessonPlan> UpdateLessonPlanAsync(LessonPlan lessonPlan)
        {
            _context.LessonPlan.Update(lessonPlan);

            await _context.SaveChangesAsync();

            return lessonPlan;
        }
    }
}
