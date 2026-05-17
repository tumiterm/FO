//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2025 22:36 PM
// Purpose:         Defines the AssessmentsRepository.

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
    /// Represents a repository specifically for performing operations on the Assessment Repository.
    /// </summary>
    public class AssessmentsRepository : Repository<AssessmentAttachment>, IAssessments
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentsRepository"/> class with the specified database
        /// context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> used to access the database. Cannot be <see langword="null"/>.</param>
        public AssessmentsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Assessment model in the repository.
        /// </summary>
        /// <param name="assessment">The Assessment model to be updated.</param>
        public async Task<AssessmentAttachment> UpdateAssessmentAsync(AssessmentAttachment assessment)
        {
            _context.AssessmentAttachments.Update(assessment);

            await _context.SaveChangesAsync();

            return assessment;
        }
    }
}
