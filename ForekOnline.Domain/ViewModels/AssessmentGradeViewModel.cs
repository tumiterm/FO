// <copyright file="AssessmentGradeViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:52 PM
// Purpose:         Defines the AssessmentGradeViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the grading details and summary information for a learner's assessment attempt, including scores,
    /// question counts, and short answer grading data.
    /// </summary>
    /// <remarks>This view model is typically used to display or process the results of an assessment attempt
    /// for a learner. It includes both objective grading metrics, such as multiple-choice question counts and scores,
    /// as well as subjective grading data for short answer questions. All properties are intended to be populated with
    /// the results of a single assessment attempt.</remarks>
    public class AssessmentGradeViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the assessment.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for this attempt.
        /// </summary>
        public Guid AttemptId { get; set; }

        /// <summary>
        /// Gets or sets the title of the assessment.
        /// </summary>
        public string AssessmentTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the learner's identification passport.
        /// </summary>
        public string LearnerIdPass { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the student.
        /// </summary>
        public string StudentDisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of questions in the assessment.
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// Gets or sets the number of multiple-choice questions answered correctly.
        /// </summary>
        public int McqCorrectCount { get; set; }

        /// <summary>
        /// Gets or sets the current final score of the assessment attempt.
        /// </summary>
        public int CurrentFinalScore { get; set; }

        /// <summary>
        /// Gets or sets the current percentage score of the assessment attempt.
        /// </summary>
        public double CurrentPercentage { get; set; }

        /// <summary>
        /// Gets or sets the list of short answer questions and their grading details.
        /// </summary>
        public List<GradeShortAnswerRowViewModel> ShortAnswers { get; set; } = new();
    }
}
