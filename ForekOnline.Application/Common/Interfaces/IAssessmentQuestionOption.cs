// <copyright file="IAssessmentQuestionOption.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-11-2025 15:07 PM
// Purpose:         Defines the IAssessmentQuestionOption interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents an interface for managing assessment question options, providing methods for updating and retrieving
    /// options.
    /// </summary>
    public interface IAssessmentQuestionOption : IRepository<AssessmentQuestionOption>
    {
        /// <summary>
        /// Updates the specified assessment question option asynchronously.
        /// </summary>
        /// <param name="assessmentQuestionOption">The assessment question option to update. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="AssessmentQuestionOption"/>.</returns>
        Task<AssessmentQuestionOption> UpdateAssessmentQuestionOptionAsync(AssessmentQuestionOption assessmentQuestionOption);

    }
}
