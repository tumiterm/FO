// <copyright file="IAssessmentQuestion.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:06 PM
// Purpose:         Defines the IAssessmentQuestion interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a contract for managing assessment questions within a repository.
    /// </summary>
    /// <remarks>This interface extends the <see cref="IRepository{T}"/> interface, providing additional
    /// functionality specific to assessment questions.</remarks>
    public interface IAssessmentQuestion : IRepository<AssessmentQuestion>
    {
        /// <summary>
        /// Updates the specified assessment question category asynchronously.
        /// </summary>
        /// <param name="assessmentQuestion">The assessment question containing the updated category information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="AssessmentQuestion"/> object.</returns>
        Task<AssessmentQuestion> UpdateAssessmentQuestionAsync(AssessmentQuestion assessmentQuestion);
    }
}
