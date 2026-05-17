// <copyright file="IAssessmentAttemptAnswer.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:14 PM
// Purpose:         Defines the IAssessmentAttemptAnswer interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a contract for managing assessment attempt answers within a repository.
    /// </summary>
    /// <remarks>This interface extends the <see cref="IRepository{T}"/> interface, providing additional
    /// functionality specific to handling <see cref="AssessmentAttemptAnswer"/> entities.</remarks>
    public interface IAssessmentAttemptAnswer : IRepository<AssessmentAttemptAnswer>
    {
        /// <summary>
        /// Updates an existing assessment attempt answer asynchronously.
        /// </summary>
        /// <param name="assessmentAttemptAnswer">The assessment attempt answer to update. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="AssessmentAttemptAnswer"/>.</returns>
        Task<AssessmentAttemptAnswer> UpdateAssessmentAttemptAnswerAsync(AssessmentAttemptAnswer assessmentAttemptAnswer);
    }
}
