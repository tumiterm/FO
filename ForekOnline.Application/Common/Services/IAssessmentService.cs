// <copyright file="IAssessmentService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    23/11/2025 23:17:27 PM
// Purpose:         Defines the IAssessmentService

#region Using Directives
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides methods for managing and interacting with assessments, including creation, updates, and attempts.
    /// </summary>
    public interface IAssessmentService
    {
        Task<ValidationResponse> CreateAssessmentAsync(AssessmentConfigViewModel config, string createdBy);

        Task<ValidationResponse> UpdateAssessmentAsync(AssessmentConfigViewModel config, string modifiedBy);

        Task<AssessmentConfigViewModel?> GetAssessmentConfigAsync(Guid assessmentId);

        Task<bool> CanStartAttemptAsync(Guid assessmentId, string learnerIdPass);

        Task<AssessmentAttemptViewModel?> StartAttemptAsync(Guid assessmentId, string learnerIdPass, string? passwordPlain);

        Task<ValidationResponse> SubmitAttemptAsync(SubmitAssessmentRequest request, string learnerIdPass);

        Task<int> GetRemainingAttemptsAsync(Guid assessmentId, string learnerIdPass);

        Task<(int score, double percent, TimeSpan timeUsed, AssessmentAttemptViewModel? review)> GetAttemptResultAsync(Guid attemptId, bool includeReview);

        Task AutoSubmitExpiredAttemptsAsync();

        /// <summary>
        /// Approves or rejects an assessment during moderation.
        /// </summary>
        /// <param name="assessmentId">The assessment to moderate.</param>
        /// <param name="approved">True to approve, false to reject.</param>
        /// <param name="moderatedBy">The moderator's name.</param>
        /// <param name="rejectionReason">Required when <paramref name="approved"/> is false.</param>
        Task<ValidationResponse> ModerateAssessmentAsync(Guid assessmentId, bool approved, string moderatedBy, string? rejectionReason);
    }
}
