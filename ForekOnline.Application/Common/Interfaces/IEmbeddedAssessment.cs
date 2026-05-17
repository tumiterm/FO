// <copyright file="IEmbeddedAssessment.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-10-2025 22:21 PM
// Purpose:         Defines the IEmbeddedAssessment interface.


#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines operations for managing and updating in-app assessment.
    /// </summary>
    /// <remarks>This interface extends the <see cref="IRepository{T}"/> interface, providing additional
    /// functionality specific to  <see cref="Assessment"/> entities.</remarks>
    public interface IEmbeddedAssessment : IRepository<Assessment>
    {
        /// <summary>
        /// Updates the specified assessment and returns the updated version.
        /// </summary>
        /// <param name="assessment">The assessment to update. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="Assessment"/>.</returns>
        Task<Assessment> UpdateAssessmentAsync(Assessment assessment);

    }
}
