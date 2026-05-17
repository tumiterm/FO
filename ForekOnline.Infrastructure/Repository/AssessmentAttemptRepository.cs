// <copyright file="AssessmentAttemptRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:38 PM
// Purpose:         Defines the AssessmentAttempt Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides methods for managing and updating assessment attempts in the database.
    /// </summary>
    /// <remarks>This repository is responsible for handling operations related to <see
    /// cref="AssessmentAttempt"/> entities, including updating existing attempts. It interacts with the underlying
    /// database context to perform these operations.</remarks>
    public class AssessmentAttemptRepository : Repository<AssessmentAttempt>, IAssessmentAttempt
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentAttemptRepository"/> class with the specified
        /// database context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> used to access the database. Cannot be null.</param>
        public AssessmentAttemptRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing assessment attempt in the database.
        /// </summary>
        /// <remarks>This method saves changes to the database asynchronously. Ensure that the provided
        /// <paramref name="assessmentAttempt"/> object is already tracked by the context.</remarks>
        /// <param name="assessmentAttempt">The assessment attempt to update. Cannot be null.</param>
        /// <returns>The updated <see cref="AssessmentAttempt"/> object.</returns>
        public Task<AssessmentAttempt> UpdateAssessmentAttemptAsync(AssessmentAttempt assessmentAttempt)
        {
            if (assessmentAttempt is null)
                throw new ArgumentNullException(nameof(assessmentAttempt));

            _context.AssessmentAttempts.Update(assessmentAttempt);

            return Task.FromResult(assessmentAttempt);
        }
    }
}
