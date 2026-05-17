// <copyright file="AssessmentAttemptAnswerRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:44 PM
// Purpose:         Defines the AssessmentAttemptAnswer Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides methods for managing and updating <see cref="AssessmentAttemptAnswer"/> entities in the data store.
    /// </summary>
    /// <remarks>This repository is responsible for handling data operations related to <see
    /// cref="AssessmentAttemptAnswer"/> objects, including updating existing records in the database. It utilizes the
    /// <see cref="ApplicationDbContext"/> to perform these operations.</remarks>
    public class AssessmentAttemptAnswerRepository : Repository<AssessmentAttemptAnswer>, IAssessmentAttemptAnswer
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentAttemptAnswerRepository"/> class with the specified
        /// database context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> used to access the database. Cannot be null.</param>
        public AssessmentAttemptAnswerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing assessment attempt answer in the database.
        /// </summary>
        /// <param name="assessmentAttemptAnswer">The assessment attempt answer to update. Cannot be null.</param>
        /// <returns>The updated <see cref="AssessmentAttemptAnswer"/> object.</returns>
        public Task<AssessmentAttemptAnswer> UpdateAssessmentAttemptAnswerAsync(AssessmentAttemptAnswer assessmentAttemptAnswer)
        {
            _context.AssessmentAttemptAnswers.Update(assessmentAttemptAnswer);

            //await _context.SaveChangesAsync();

            return Task.FromResult(assessmentAttemptAnswer);
        }
    }
}
