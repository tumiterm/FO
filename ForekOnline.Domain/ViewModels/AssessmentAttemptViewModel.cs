// <copyright file="AssessmentAttemptViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:41 PM
// Purpose:         Defines the AssessmentAttemptViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a user's attempt at an assessment, including configuration, timing, and question
    /// details.
    /// </summary>
    /// <remarks>This class is typically used to transfer assessment attempt data between the application and
    /// the user interface. It contains information about the assessment, attempt identifiers, timing constraints,
    /// question shuffling options, fullscreen enforcement, focus loss limits, review settings, and the list of
    /// questions presented in the attempt. All properties are intended to be set and read as part of the assessment
    /// workflow.</remarks>
    public class AssessmentAttemptViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the assessment.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the current attempt.
        /// </summary>
        public Guid AttemptId { get; set; }

        /// <summary>
        /// Gets or sets the title associated with the object.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the duration of the timer, in minutes.
        /// </summary>
        public int TimerMinutes { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the operation started.
        /// </summary>
        public DateTime StartedUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether questions should be presented in a random order.
        /// </summary>
        public bool ShuffleQuestions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order of answers should be randomized.
        /// </summary>
        public bool ShuffleAnswers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fullscreen mode is enforced for the application.
        /// </summary>
        public bool EnforceFullscreen { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of consecutive focus losses allowed before triggering corrective action.
        /// </summary>
        public int MaxFocusLossAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a review should be shown after the operation completes.
        /// </summary>
        public bool ShowReviewAfter { get; set; }

        /// <summary>
        /// Gets or sets the collection of questions included in the attempt.
        /// </summary>
        public List<AttemptQuestionItem> Questions { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether LaTeX math rendering is enabled for this assessment attempt.
        /// </summary>
        public bool EnableMathRendering { get; set; }
    }
}
