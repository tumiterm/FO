//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    28/Oct/2025 22:26 PM
// Purpose:         Defines the EmbeddedAssessmentsRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Assessment Repository.
    /// </summary>
    public class EmbeddedAssessmentsRepository : Repository<Assessment>, IEmbeddedAssessment
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedAssessmentsRepository"/> class with the specified
        /// database context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> used to access the database. Cannot be null.</param>
        public EmbeddedAssessmentsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Assessment model in the repository.
        /// </summary>
        /// <param name="assessment">The Assessment model to be updated.</param>
        public async Task<Assessment> UpdateAssessmentAsync(Assessment assessment)
        {
            _context.Assessments.Update(assessment);

            await _context.SaveChangesAsync();

            return assessment;
        }
    }
}
