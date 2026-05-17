// <copyright file="IAssessmentAttempt.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:12 PM
// Purpose:         Defines the IAssessmentAttempt interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents an attempt to complete an assessment, providing methods to update the attempt's details.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{AssessmentAttempt}"/> to include functionality
    /// specific to updating assessment attempts.</remarks>
    public interface IAssessmentAttempt : IRepository<AssessmentAttempt>
    {
        /// <summary>
        /// Updates the specified assessment attempt asynchronously.
        /// </summary>
        /// <param name="assessmentAttempt">The assessment attempt to update. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="AssessmentAttempt"/>.</returns>
        Task<AssessmentAttempt> UpdateAssessmentAttemptAsync(AssessmentAttempt assessmentAttempt);
    }
}
