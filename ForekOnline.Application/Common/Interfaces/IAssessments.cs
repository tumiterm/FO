// <copyright file="IAssessments.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    26-06-2025 22:07 PM
// Purpose:         Defines the IAssessments interface.


#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines operations for managing and updating assessment attachments.
    /// </summary>
    /// <remarks>This interface extends the <see cref="IRepository{T}"/> interface, providing additional
    /// functionality specific to  <see cref="AssessmentAttachment"/> entities.</remarks>
    public interface IAssessments : IRepository<AssessmentAttachment>
    {
        /// <summary>
        /// Updates the specified assessment attachment and returns the updated version.
        /// </summary>
        /// <param name="assessment">The assessment attachment to update. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="AssessmentAttachment"/>.</returns>
        Task<AssessmentAttachment> UpdateAssessmentAsync(AssessmentAttachment assessment);

    }
}
