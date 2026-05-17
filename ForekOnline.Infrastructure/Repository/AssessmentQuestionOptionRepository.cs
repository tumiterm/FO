// <copyright file="AssessmentQuestionOptionRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:33 PM
// Purpose:         Defines the AssessmentQuestionOption Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides data access methods for managing <see cref="AssessmentQuestionOption"/> entities in the database.
    /// </summary>
    /// <remarks>This repository offers functionality to update and manage assessment question options within
    /// the application's data context.</remarks>
    public class AssessmentQuestionOptionRepository : Repository<AssessmentQuestionOption>, IAssessmentQuestionOption
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentQuestionOptionRepository"/> class with the specified
        /// database context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> used to access the database. Cannot be null.</param>
        public AssessmentQuestionOptionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing assessment question option in the database.
        /// </summary>
        /// <remarks>This method updates the specified assessment question option in the database and
        /// saves the changes asynchronously.</remarks>
        /// <param name="assessmentQuestionOption">The assessment question option to update. Must not be null.</param>
        /// <returns>The updated <see cref="AssessmentQuestionOption"/> object.</returns>
        public async Task<AssessmentQuestionOption> UpdateAssessmentQuestionOptionAsync(AssessmentQuestionOption assessmentQuestionOption)
        {
            _context.AssessmentQuestionOptions.Update(assessmentQuestionOption);

            await _context.SaveChangesAsync();

            return assessmentQuestionOption;
        }
    }
}
