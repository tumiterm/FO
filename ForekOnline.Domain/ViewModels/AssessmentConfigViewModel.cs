// <copyright file="AssessmentConfigViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:45 PM
// Purpose:         Defines the AssessmentConfigViewModel class

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the configuration settings for an assessment, including timing, security, retry options, question
    /// ordering, and review behavior.
    /// </summary>
    /// <remarks>This view model is used to define and transfer assessment parameters between application
    /// layers, such as when creating or editing an assessment. It includes options for password protection, question
    /// and answer shuffling, retry limits, fullscreen enforcement, and review display. The collection of questions
    /// specifies the content and structure of the assessment. All properties should be set according to the
    /// requirements of the assessment being configured.</remarks>
    public class AssessmentConfigViewModel
    {
        public Guid? AssessmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TimerMinutes { get; set; }
        public bool IsPasswordProtected { get; set; }
        public string? AssessmentPassword { get; set; }
        public bool AllowRetries { get; set; }
        public int? MaxRetryAttempts { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleAnswers { get; set; }
        public bool ShowReviewAfter { get; set; } = true;
        public bool EnforceFullscreen { get; set; }
        public int MaxFocusLossAllowed { get; set; } = 3;
        public int MaxScore { get; set; }
        public eAssessmentType AssessmentType { get; set; } = eAssessmentType.Informal;
        public List<QuestionConfigItem> Questions { get; set; } = new();
        public bool EnableMathRendering { get; set; }
        public DateTime? ScheduledDate { get; set; }

    }
}
