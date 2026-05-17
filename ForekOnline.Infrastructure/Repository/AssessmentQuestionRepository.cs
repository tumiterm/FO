// <copyright file="AssessmentQuestionRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:33 PM
// Purpose:         Defines the AssessmentQuestion Repository.

#region Usings  
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides methods for managing and updating assessment questions in the database.
    /// </summary>
    /// <remarks>This repository is responsible for handling operations related to <see
    /// cref="AssessmentQuestion"/> entities, including updating existing questions. It utilizes the <see
    /// cref="ApplicationDbContext"/> to interact with the database.</remarks>
    public class AssessmentQuestionRepository : Repository<AssessmentQuestion>, IAssessmentQuestion
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentQuestionRepository"/> class with the specified
        /// database context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> used to access the database. Cannot be null.</param>
        public AssessmentQuestionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing assessment question in the database.
        /// </summary>
        /// <remarks>This method saves changes to the database asynchronously. Ensure that the provided
        /// assessment question has been previously retrieved from the database and contains a valid
        /// identifier.</remarks>
        /// <param name="assessmentQuestion">The assessment question to update. Must not be null and should have a valid identifier.</param>
        /// <returns>The updated <see cref="AssessmentQuestion"/> object.</returns>
        public async Task<AssessmentQuestion> UpdateAssessmentQuestionAsync(AssessmentQuestion assessmentQuestion)
        {
            _context.AssessmentQuestions.Update(assessmentQuestion);

            await _context.SaveChangesAsync();

            return assessmentQuestion;
        }
    }
}
