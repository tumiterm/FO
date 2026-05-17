// <copyright file="AssessmentService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    09/11/2025 16:13:27 PM
// Purpose:         Defines the AssessmentService 

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides services for managing assessments, including creation, updating, and handling attempts.
    /// </summary>
    /// <remarks>This service interacts with the data layer to perform operations related to assessments and
    /// their attempts. It includes methods for creating and updating assessments, starting and submitting attempts, and
    /// retrieving results.</remarks>
    public class AssessmentService : IAssessmentService
    {
        #region Fields
        private readonly IUnitOfWork _context;
        private readonly ILogger<AssessmentService> _logger;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentService"/> class.
        /// </summary>
        /// <param name="context">The unit of work context used for database operations. Cannot be null.</param>
        /// <param name="logger">The logger instance used for logging operations. Cannot be null.</param>
        public AssessmentService(IUnitOfWork context, ILogger<AssessmentService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Automatically submits attempts that have expired based on their timer.
        /// </summary>
        /// <remarks>This method is intended to be used as a background task to identify and submit
        /// in-progress attempts whose timers have expired. The specific scheduling and execution context depend on the
        /// hosting environment, such as a hosted service.</remarks>
        /// <returns></returns>
        public Task AutoSubmitExpiredAttemptsAsync()
        {

            // Background task: find in-progress attempts where timer expired and auto-submit them.
            // Implementation depends on scheduling (e.g., hosted service).
            return Task.CompletedTask;
        }

        /// <summary>
        /// Determines whether a new assessment attempt can be started for a given learner.
        /// </summary>
        /// <remarks>This method checks the status and retry policy of the specified assessment to
        /// determine if a new  attempt is allowed. If the assessment does not allow retries, it ensures no previous
        /// attempts  exist that are not aborted. If retries are allowed, it checks that the number of submitted 
        /// attempts does not exceed the maximum allowed retries.</remarks>
        /// <param name="assessmentId">The unique identifier of the assessment to be attempted.</param>
        /// <param name="learnerIdPass">The identifier for the learner attempting the assessment.</param>
        /// <returns><see langword="true"/> if the learner can start a new attempt for the specified assessment;  otherwise, <see
        /// langword="false"/>.</returns>
        public async Task<bool> CanStartAttemptAsync(Guid assessmentId, string learnerIdPass)
        {
            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == assessmentId);
            if (assessment == null || !assessment.IsActive) return false;

            if (!assessment.AllowRetries)
            {
                var existing = await _context.AssessmentAttempts.GetAllAsync(x => x.AssessmentId == assessmentId && x.LearnerIdPass == learnerIdPass && x.Status != eAssessmentAttemptStatus.Aborted);
                return !existing.Any();
            }
            else
            {
                var attempts = await _context.AssessmentAttempts.GetAllAsync(x => x.AssessmentId == assessmentId && x.LearnerIdPass == learnerIdPass && x.Status == eAssessmentAttemptStatus.Submitted);
                if (assessment.MaxRetryAttempts.HasValue && attempts.Count() >= assessment.MaxRetryAttempts.Value) return false;
            }
            return true;
        }

        /// <summary>
        /// Asynchronously creates a new assessment based on the provided configuration.
        /// </summary>
        /// <remarks>The method validates the provided configuration to ensure it contains a title and at
        /// least one question. If the assessment is password-protected, the password is hashed. The method also ensures
        /// that multiple-choice questions have at least one correct option.</remarks>
        /// <param name="config">The configuration details for the assessment, including title, questions, and settings.</param>
        /// <param name="createdBy">The identifier of the user who is creating the assessment.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the success or failure of the operation. Returns a validation
        /// error if the configuration is invalid.</returns>
        public async Task<ValidationResponse> CreateAssessmentAsync(AssessmentConfigViewModel config, string createdBy)
        {
            if (config == null) return new ValidationResponse("Config is null");
            if (string.IsNullOrWhiteSpace(config.Title)) return new ValidationResponse("Title required");
            if (config.Questions.Count == 0) return new ValidationResponse("At least one question required");

            if (config.MaxScore < 0) return new ValidationResponse("MaxScore must be 0 or greater.");

            var totalMarks = config.Questions.Sum(q => q.Marks <= 0 ? 0 : q.Marks);
            if (config.MaxScore > 0 && totalMarks != config.MaxScore)
                return new ValidationResponse($"Total question marks ({totalMarks}) must equal Max Score ({config.MaxScore}).");

            var assessment = new Assessment
            {
                Id = Guid.NewGuid(),
                Title = config.Title.Trim(),
                TimerMinutes = config.TimerMinutes,
                IsPasswordProtected = config.IsPasswordProtected,
                PasswordHash = config.IsPasswordProtected && !string.IsNullOrWhiteSpace(config.AssessmentPassword)
                    ? HashPassword(config.AssessmentPassword.Trim())
                    : null,
                AllowRetries = config.AllowRetries,
                MaxRetryAttempts = config.AllowRetries ? config.MaxRetryAttempts : null,
                ShuffleQuestions = config.ShuffleQuestions,
                ShuffleAnswers = config.ShuffleAnswers,
                ShowReviewAfter = config.ShowReviewAfter,
                EnforceFullscreen = config.EnforceFullscreen,
                MaxFocusLossAllowed = config.MaxFocusLossAllowed,
                CreatedBy = createdBy,
                CreatedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                TotalQuestions = config.Questions.Count,
                MaxScore = config.MaxScore,
                AssessmentType = config.AssessmentType,
                EnableMathRendering = config.EnableMathRendering,
                IsModerated = false,
                ScheduledDateUtc = config.ScheduledDate
            };

            int order = 1;
            foreach (var q in config.Questions)
            {
                var qEntity = new AssessmentQuestion
                {
                    Id = Guid.NewGuid(),
                    AssessmentId = assessment.Id,
                    QuestionType = q.QuestionType,
                    DisplayOrder = order++,
                    Prompt = q.Prompt.Trim(),
                    Explanation = q.Explanation,
                    ImagePath = q.ImagePath,
                    EnableAnnotation = q.EnableAnnotation && !string.IsNullOrWhiteSpace(q.ImagePath),
                    IsActive = true,
                    Marks = q.Marks <= 0 ? 1 : q.Marks
                };

                if (q.QuestionType == eAssessmentQuestionType.MultipleChoice)
                {
                    int optOrder = 0;
                    foreach (var opt in q.Options)
                    {
                        qEntity.Options.Add(new AssessmentQuestionOption
                        {
                            Id = Guid.NewGuid(),
                            AssessmentQuestionId = qEntity.Id,
                            Text = opt.Text.Trim(),
                            IsCorrect = opt.IsCorrect,
                            OrderIndex = optOrder++
                        });
                    }

                    if (!qEntity.Options.Any(o => o.IsCorrect))
                        return new ValidationResponse($"Question '{q.Prompt}' has no correct option.");
                }
                assessment.Questions.Add(qEntity);
            }

            await _context.EmbeddedAssessment.AddAsync(assessment);
            var saved = await _context.SaveAsync();
            return saved > 0 ? new ValidationResponse() : new ValidationResponse("Failed to save assessment");
        }

        /// <summary>
        /// Asynchronously retrieves the configuration for a specified assessment.
        /// </summary>
        /// <remarks>The configuration includes details such as the assessment's title, timer settings,
        /// question order, and options for each question. This method fetches the assessment and its related questions
        /// and options from the data context.</remarks>
        /// <param name="assessmentId">The unique identifier of the assessment to retrieve the configuration for.</param>
        /// <returns>An <see cref="AssessmentConfigViewModel"/> containing the configuration details of the assessment, or <see
        /// langword="null"/> if the assessment does not exist.</returns>
        public async Task<AssessmentConfigViewModel?> GetAssessmentConfigAsync(Guid assessmentId)
        {
            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == assessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });
            if (assessment == null) return null;

            var questionIds = assessment.Questions.Select(q => q.Id).ToList();
            var allOptions = await _context.AssessmentQuestionOptions.GetAllAsync(o => questionIds.Contains(o.AssessmentQuestionId));

            return new AssessmentConfigViewModel
            {
                AssessmentId = assessment.Id,
                Title = assessment.Title,
                TimerMinutes = assessment.TimerMinutes,
                IsPasswordProtected = assessment.IsPasswordProtected,
                AllowRetries = assessment.AllowRetries,
                MaxRetryAttempts = assessment.MaxRetryAttempts,
                ShuffleQuestions = assessment.ShuffleQuestions,
                ShuffleAnswers = assessment.ShuffleAnswers,
                ShowReviewAfter = assessment.ShowReviewAfter,
                EnforceFullscreen = assessment.EnforceFullscreen,
                MaxFocusLossAllowed = assessment.MaxFocusLossAllowed,
                AssessmentType = assessment.AssessmentType,

                MaxScore = assessment.MaxScore,

                Questions = assessment.Questions
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new QuestionConfigItem
                    {
                        QuestionId = q.Id,
                        QuestionType = q.QuestionType,
                        Prompt = q.Prompt,
                        Explanation = q.Explanation,
                        ImagePath = q.ImagePath,

                        Marks = q.Marks,

                        Options = allOptions.Where(o => o.AssessmentQuestionId == q.Id)
                            .OrderBy(o => o.OrderIndex)
                            .Select(o => new QuestionOptionItem
                            {
                                OptionId = o.Id,
                                Text = o.Text,
                                IsCorrect = o.IsCorrect
                            }).ToList()
                    }).ToList()
            };
        }

        /// <summary>
        /// Retrieves the result of an assessment attempt, including the score, percentage, time used, and optionally a
        /// review.
        /// </summary>
        /// <remarks>The method fetches the assessment attempt and its associated assessment details from
        /// the database.  If the attempt is still in progress or not found, it returns default values.  If <paramref
        /// name="includeReview"/> is <see langword="true"/> and the assessment is configured to show a review,  the
        /// method includes a detailed review of the attempt.</remarks>
        /// <param name="attemptId">The unique identifier of the assessment attempt to retrieve results for.</param>
        /// <param name="includeReview">A boolean value indicating whether to include a review of the attempt.  If <see langword="true"/>, and the
        /// assessment allows review, a detailed review is included in the result.</param>
        /// <returns>A tuple containing the score as an integer, the percentage as a double, the time used as a <see
        /// cref="TimeSpan"/>,  and an optional <see cref="AssessmentAttemptViewModel"/> for the review.  If the attempt
        /// is in progress or not found, returns default values with a null review.</returns>
        public async Task<(int score, double percent, TimeSpan timeUsed, AssessmentAttemptViewModel? review)> GetAttemptResultAsync(Guid attemptId, bool includeReview)
        {
            var attempt = await _context.AssessmentAttempts.GetAsync(a => a.Id == attemptId,
               includeProperties: new[] { nameof(AssessmentAttempt.Answers) });
            if (attempt == null || attempt.Status is eAssessmentAttemptStatus.InProgress)
                return (0, 0, TimeSpan.Zero, null);

            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == attempt.AssessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });
            if (assessment == null) return (0, 0, TimeSpan.Zero, null);

            TimeSpan used = (attempt.SubmittedUtc ?? DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime) - attempt.StartedUtc;

            AssessmentAttemptViewModel? reviewVm = null;

            if (includeReview && assessment.ShowReviewAfter)
            {
                var allOptions = await _context.AssessmentQuestionOptions.GetAllAsync(o => o.AssessmentQuestionId != Guid.Empty);
                reviewVm = new AssessmentAttemptViewModel
                {
                    AssessmentId = assessment.Id,
                    AttemptId = attempt.Id,
                    Title = assessment.Title,
                    TimerMinutes = assessment.TimerMinutes,
                    StartedUtc = attempt.StartedUtc,
                    Questions = assessment.Questions
                        .OrderBy(q => q.DisplayOrder)
                        .Select(q => new AttemptQuestionItem
                        {
                            QuestionId = q.Id,
                            Prompt = q.Prompt,
                            ImagePath = q.ImagePath,
                            EnableAnnotation = q.EnableAnnotation,
                            IsMultipleChoice = q.QuestionType == eAssessmentQuestionType.MultipleChoice,
                            Options = allOptions.Where(o => o.AssessmentQuestionId == q.Id)
                            .Select(o => new AttemptOptionItem
                            {
                                OptionId = o.Id,
                                Text = o.Text
                            }).ToList()
                        }).ToList()
                };
            }
            return (attempt.FinalScore ?? 0, attempt.Percentage ?? 0, used, reviewVm);
        }

        /// <summary>
        /// Asynchronously retrieves the number of remaining attempts for a specified assessment and learner.
        /// </summary>
        /// <remarks>This method calculates the remaining attempts by subtracting the number of submitted
        /// attempts from the maximum allowed retries. If the assessment does not exist or does not allow retries, the
        /// method returns 0.</remarks>
        /// <param name="assessmentId">The unique identifier of the assessment.</param>
        /// <param name="learnerIdPass">The identifier for the learner, used to track attempts.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of remaining
        /// attempts. Returns <see langword="int.MaxValue"/> if the assessment allows unlimited retries, or 0 if retries
        /// are not allowed.</returns>
        public async Task<int> GetRemainingAttemptsAsync(Guid assessmentId, string learnerIdPass)
        {
            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == assessmentId);
            if (assessment == null || !assessment.AllowRetries) return 0;
            var attempts = await _context.AssessmentAttempts.GetAllAsync(a =>
                a.AssessmentId == assessmentId &&
                a.LearnerIdPass == learnerIdPass &&
                a.Status == eAssessmentAttemptStatus.Submitted);
            var used = attempts.Count();
            if (!assessment.MaxRetryAttempts.HasValue) return int.MaxValue;
            return Math.Max(assessment.MaxRetryAttempts.Value - used, 0);
        }

        /// <summary>
        /// Initiates a new assessment attempt for a learner.
        /// </summary>
        /// <remarks>The method checks if the assessment is active and, if password-protected, verifies
        /// the provided password. It also ensures that the learner is eligible to start the attempt. If successful, it
        /// creates a new attempt and returns a view model containing the assessment details and questions, with options
        /// shuffled if specified.</remarks>
        /// <param name="assessmentId">The unique identifier of the assessment to be attempted.</param>
        /// <param name="learnerIdPass">The identifier for the learner attempting the assessment, which may include a passcode.</param>
        /// <param name="passwordPlain">The plain text password for the assessment, if it is password-protected. Can be <see langword="null"/> if no
        /// password is required.</param>
        /// <returns>An <see cref="AssessmentAttemptViewModel"/> representing the started attempt, or <see langword="null"/> if
        /// the attempt could not be started.</returns>
        public async Task<AssessmentAttemptViewModel?> StartAttemptAsync(Guid assessmentId, string learnerIdPass, string? passwordPlain)
        {
            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == assessmentId,
               includeProperties: new[] { nameof(Assessment.Questions) });
            if (assessment == null || !assessment.IsActive) return null;

            if (!assessment.IsModerated || assessment.ModerationApproved != true) return null;

            if (assessment.ScheduledDateUtc.HasValue && assessment.ScheduledDateUtc.Value > DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime)
                return null;

            if (assessment.IsPasswordProtected)
            {
                if (string.IsNullOrWhiteSpace(passwordPlain) || !VerifyHash(passwordPlain.Trim(), assessment.PasswordHash!))
                    return null;
            }

            if (!await CanStartAttemptAsync(assessmentId, learnerIdPass))
                return null;

            var qIds = assessment.Questions.Select(q => q.Id).ToList();
            var options = await _context.AssessmentQuestionOptions.GetAllAsync(o => qIds.Contains(o.AssessmentQuestionId));

            var attempt = new AssessmentAttempt
            {
                Id = Guid.NewGuid(),
                AssessmentId = assessment.Id,
                LearnerIdPass = learnerIdPass,
                StartedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                Status = eAssessmentAttemptStatus.InProgress
            };
            await _context.AssessmentAttempts.AddAsync(attempt);
            var saved = await _context.SaveAsync();
            if (saved <= 0) return null;

            var questionItems = assessment.Questions.Where(q => q.IsActive).Select(q => new AttemptQuestionItem
            {
                QuestionId = q.Id,
                Prompt = q.Prompt,
                EnableAnnotation = q.EnableAnnotation,
                ImagePath = q.ImagePath,
                IsMultipleChoice = (q.QuestionType == eAssessmentQuestionType.MultipleChoice),
                Options = options.Where(o => o.AssessmentQuestionId == q.Id)
                   .Select(o => new AttemptOptionItem
                   {
                       OptionId = o.Id,
                       Text = o.Text
                   }).ToList()
            }).ToList();

            if (assessment.ShuffleQuestions)
                questionItems = questionItems.OrderBy(_ => Guid.NewGuid()).ToList();
            if (assessment.ShuffleAnswers)
            {
                foreach (var q in questionItems.Where(q => q.IsMultipleChoice))
                {
                    q.Options = q.Options.OrderBy(_ => Guid.NewGuid()).ToList();
                }
            }

            return new AssessmentAttemptViewModel
            {
                AssessmentId = assessment.Id,
                AttemptId = attempt.Id,
                Title = assessment.Title,
                TimerMinutes = assessment.TimerMinutes,
                StartedUtc = attempt.StartedUtc,
                ShuffleQuestions = assessment.ShuffleQuestions,
                ShuffleAnswers = assessment.ShuffleAnswers,
                EnforceFullscreen = assessment.EnforceFullscreen,
                MaxFocusLossAllowed = assessment.MaxFocusLossAllowed,
                ShowReviewAfter = assessment.ShowReviewAfter,
                Questions = questionItems,
                EnableMathRendering = assessment.EnableMathRendering
            };
        }
       
        /// <summary>
        /// Submits an assessment attempt asynchronously, evaluating the answers and updating the attempt status.
        /// </summary>
        /// <remarks>This method processes the answers provided in the <paramref name="request"/>,
        /// calculates the score, and updates the attempt status to either submitted or auto-submitted based on the
        /// request parameters. It ensures that the attempt is valid and in progress, and that the learner identity
        /// matches before processing.</remarks>
        /// <param name="request">The request containing the attempt details and answers to be submitted.</param>
        /// <param name="learnerIdPass">The identifier used to verify the learner's identity for the attempt.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the success or failure of the submission process.</returns>
        public async Task<ValidationResponse> SubmitAttemptAsync(SubmitAssessmentRequest request, string learnerIdPass)
        {
            var attempt = await _context.AssessmentAttempts.GetAsync(
                a => a.Id == request.AttemptId,
                includeProperties: new[] { nameof(AssessmentAttempt.Answers) });

            if (attempt == null)
                return new ValidationResponse("Attempt invalid");

            if (attempt.LearnerIdPass != learnerIdPass)
                return new ValidationResponse("Learner mismatch");

            if (attempt.Status is eAssessmentAttemptStatus.Submitted or eAssessmentAttemptStatus.AutoSubmitted)
                return new ValidationResponse();

            if (attempt.Status != eAssessmentAttemptStatus.InProgress)
                return new ValidationResponse("Attempt invalid");

            var assessment = await _context.EmbeddedAssessment.GetAsync(
                a => a.Id == attempt.AssessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });

            if (assessment == null)
                return new ValidationResponse("Assessment missing");

            var allOptions = await _context.AssessmentQuestionOptions.GetAllAsync(o => o.AssessmentQuestionId != Guid.Empty);
            var questionsById = assessment.Questions.ToDictionary(q => q.Id, q => q);

            int correctCount = 0;

            foreach (var submitted in request.Answers)
            {
                if (!questionsById.TryGetValue(submitted.QuestionId, out var question))
                    continue;

                bool? isCorrect = null;
                Guid? selectedOptionId = submitted.SelectedOptionId;

                if (question.QuestionType == eAssessmentQuestionType.MultipleChoice && selectedOptionId.HasValue)
                {
                    var opt = allOptions.FirstOrDefault(o => o.Id == selectedOptionId.Value && o.AssessmentQuestionId == question.Id);
                    if (opt != null)
                    {
                        isCorrect = opt.IsCorrect;
                        if (opt.IsCorrect) correctCount++;
                    }
                }

                var shortAnswerValue = question.QuestionType == eAssessmentQuestionType.ShortAnswer
                    ? submitted.ShortAnswerValue
                    : null;

                var annotationJson = question.EnableAnnotation
                     ? submitted.DiagramAnnotationJson
                     : null;

                var annotationSnapshotFileId = question.EnableAnnotation
                    ? submitted.DiagramAnnotationSnapshotFileId
                    : null;

                var existing = attempt.Answers.FirstOrDefault(x => x.AssessmentQuestionId == question.Id);

                if (existing == null)
                {
                    var newAnswer = new AssessmentAttemptAnswer
                    {
                        Id = Guid.NewGuid(),
                        AssessmentAttemptId = attempt.Id,
                        AssessmentQuestionId = question.Id,
                        SelectedOptionId = selectedOptionId,
                        ShortAnswerValue = shortAnswerValue,
                        IsCorrect = isCorrect,

                        DiagramAnnotationJson = annotationJson,
                        DiagramAnnotationSnapshotFileId = annotationSnapshotFileId
                    };

                    await _context.AssessmentAttemptAnswer.AddAsync(newAnswer);

                    attempt.Answers.Add(newAnswer);
                }
                else
                {
                    existing.SelectedOptionId = selectedOptionId;
                    existing.ShortAnswerValue = shortAnswerValue;
                    existing.IsCorrect = isCorrect;

                    existing.DiagramAnnotationJson = annotationJson;
                    existing.DiagramAnnotationSnapshotFileId = annotationSnapshotFileId;
                }
            }

            attempt.SubmittedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            attempt.Status = request.ForcedAutoSubmit ? eAssessmentAttemptStatus.AutoSubmitted : eAssessmentAttemptStatus.Submitted;
            attempt.FinalScore = correctCount;
            attempt.Percentage = assessment.TotalQuestions > 0
                ? Math.Round((double)correctCount / assessment.TotalQuestions * 100, 2)
                : 0;

            var saved = await _context.SaveAsync();
            return saved > 0 ? new ValidationResponse() : new ValidationResponse("Failed to submit");
        }

        /// <summary>
        /// Approves or rejects an assessment during moderation.
        /// </summary>
        public async Task<ValidationResponse> ModerateAssessmentAsync(Guid assessmentId, bool approved, string moderatedBy, string? rejectionReason)
        {
            var assessment = await _context.EmbeddedAssessment.GetAsync(
                a => a.Id == assessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });

            if (assessment == null)
                return new ValidationResponse("Assessment not found.");

            if (assessment.IsModerated && assessment.ModerationApproved == true)
                return new ValidationResponse("Assessment is already approved.");

            if (!approved && string.IsNullOrWhiteSpace(rejectionReason))
                return new ValidationResponse("A rejection reason is required.");

            if (approved)
            {
                if (assessment.TotalQuestions == 0 || !assessment.Questions.Any(q => q.IsActive))
                    return new ValidationResponse("Cannot approve an assessment with no active questions.");
            }

            assessment.IsModerated = true;
            assessment.ModerationApproved = approved;
            assessment.ModeratedBy = moderatedBy;
            assessment.ModeratedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            assessment.ModerationRejectionReason = approved ? null : rejectionReason?.Trim();

            var saved = await _context.SaveAsync();
            return saved > 0
                ? new ValidationResponse()
                : new ValidationResponse("Failed to save moderation status.");
        }

        /// <summary>
        /// Updates an existing assessment with the specified configuration and records the user who modified it.
        /// </summary>
        /// <remarks>This method updates the assessment's properties such as title, timer, password
        /// protection, and question details. It also handles the removal and recreation of associated questions and
        /// their options.</remarks>
        /// <param name="config">The configuration details for the assessment, including its ID and updated properties.</param>
        /// <param name="modifiedBy">The identifier of the user who is making the modifications.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the success or failure of the update operation. Returns a
        /// response with an error message if the assessment ID is missing or the assessment is not found.</returns>
        public async Task<ValidationResponse> UpdateAssessmentAsync(AssessmentConfigViewModel config, string modifiedBy)
        {
            if (config.AssessmentId == null) return new ValidationResponse("AssessmentId required");
            if (string.IsNullOrWhiteSpace(config.Title)) return new ValidationResponse("Title required");
            if (config.Questions == null || config.Questions.Count == 0) return new ValidationResponse("At least one question required");

            if (config.MaxScore < 0) return new ValidationResponse("MaxScore must be 0 or greater.");

            var totalMarks = config.Questions.Sum(q => q.Marks <= 0 ? 0 : q.Marks);
            if (config.MaxScore > 0 && totalMarks != config.MaxScore)
                return new ValidationResponse($"Total question marks ({totalMarks}) must equal Max Score ({config.MaxScore}).");

            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == config.AssessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });
            if (assessment == null) return new ValidationResponse("Not found");

            assessment.Title = config.Title.Trim();
            assessment.TimerMinutes = config.TimerMinutes;
            assessment.IsPasswordProtected = config.IsPasswordProtected;
            assessment.PasswordHash = assessment.IsPasswordProtected && !string.IsNullOrWhiteSpace(config.AssessmentPassword)
                ? HashPassword(config.AssessmentPassword.Trim())
                : assessment.PasswordHash;
            assessment.AllowRetries = config.AllowRetries;
            assessment.MaxRetryAttempts = config.AllowRetries ? config.MaxRetryAttempts : null;
            assessment.ShuffleQuestions = config.ShuffleQuestions;
            assessment.ShuffleAnswers = config.ShuffleAnswers;
            assessment.ShowReviewAfter = config.ShowReviewAfter;
            assessment.EnforceFullscreen = config.EnforceFullscreen;
            assessment.MaxFocusLossAllowed = config.MaxFocusLossAllowed;
            assessment.AssessmentType  = config.AssessmentType;

            assessment.MaxScore = config.MaxScore;

            assessment.ModifiedBy = modifiedBy;
            assessment.ModifiedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            var existingQuestions = await _context.AssessmentQuestions.GetAllAsync(q => q.AssessmentId == assessment.Id,
                includeProperties: new[] { nameof(AssessmentQuestion.Options) });

            foreach (var eq in existingQuestions)
            {
                await _context.AssessmentQuestions.RemoveAsync(eq);
            }

            int order = 1;

            foreach (var q in config.Questions)
            {
                var qEntity = new AssessmentQuestion
                {
                    Id = Guid.NewGuid(),
                    AssessmentId = assessment.Id,
                    QuestionType = q.QuestionType,
                    DisplayOrder = order++,
                    Prompt = q.Prompt.Trim(),
                    Explanation = q.Explanation,
                    ImagePath = q.ImagePath,
                    IsActive = true,
                    EnableAnnotation = q.EnableAnnotation && !string.IsNullOrWhiteSpace(q.ImagePath),
                    Marks = q.Marks <= 0 ? 1 : q.Marks
                };

                if (q.QuestionType == eAssessmentQuestionType.MultipleChoice)
                {
                    int optOrder = 0;
                    foreach (var opt in q.Options)
                    {
                        qEntity.Options.Add(new AssessmentQuestionOption
                        {
                            Id = Guid.NewGuid(),
                            AssessmentQuestionId = qEntity.Id,
                            Text = opt.Text.Trim(),
                            IsCorrect = opt.IsCorrect,
                            OrderIndex = optOrder++
                        });
                    }

                    if (!qEntity.Options.Any(o => o.IsCorrect))
                        return new ValidationResponse($"Question '{q.Prompt}' has no correct option.");
                }

                await _context.AssessmentQuestions.AddAsync(qEntity);
            }

            assessment.TotalQuestions = config.Questions.Count;

            await _context.EmbeddedAssessment.UpdateAssessmentAsync(assessment);
            var saved = await _context.SaveAsync();
            return saved > 0 ? new ValidationResponse() : new ValidationResponse("Update failed");
        }

        /// <summary>
        /// Moderates an assessment, marking it as reviewed and approved for learner access.
        /// </summary>
        /// <param name="assessmentId">The unique identifier of the assessment to moderate.</param>
        /// <param name="moderatedBy">The name or identifier of the moderator.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the moderation.</returns>
        public async Task<ValidationResponse> ModerateAssessmentAsync(Guid assessmentId, string moderatedBy)
        {
            var assessment = await _context.EmbeddedAssessment.GetAsync(
                a => a.Id == assessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });

            if (assessment == null)
                return new ValidationResponse("Assessment not found.");

            if (assessment.IsModerated)
                return new ValidationResponse("Assessment is already moderated.");

            if (assessment.TotalQuestions == 0 || !assessment.Questions.Any(q => q.IsActive))
                return new ValidationResponse("Cannot moderate an assessment with no active questions.");

            assessment.IsModerated = true;
            assessment.ModeratedBy = moderatedBy;
            assessment.ModeratedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            var saved = await _context.SaveAsync();
            return saved > 0
                ? new ValidationResponse()
                : new ValidationResponse("Failed to save moderation status.");
        }

        #region Private Methods

        /// <summary>
        /// Computes a SHA-256 hash for the specified plain text password and returns the hash as a Base64-encoded
        /// string.
        /// </summary>
        /// <param name="plain">The plain text password to hash. Cannot be null or empty.</param>
        /// <returns>A Base64-encoded string representing the SHA-256 hash of the input password.</returns>
        private static string HashPassword(string plain)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(plain)));
        }

        /// <summary>
        /// Verifies whether the specified plain text matches the given hash.
        /// </summary>
        /// <param name="plain">The plain text to verify against the hash.</param>
        /// <param name="hash">The hash to compare with the plain text.</param>
        /// <returns><see langword="true"/> if the plain text matches the hash; otherwise, <see langword="false"/>.</returns>
        private static bool VerifyHash(string plain, string hash) => HashPassword(plain) == hash;

        #endregion
    }
}
