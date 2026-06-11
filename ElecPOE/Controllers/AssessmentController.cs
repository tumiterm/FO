// <copyright file="AssessmentController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2023 14:09:27 PM
// Purpose:         Defines the AssessmentController class

#region Usings
using ElecPOE.Common;
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Provides actions for managing assessments, including creating, starting, submitting, and listing assessments.
    /// </summary>
    /// <remarks>This controller handles both HTTP GET and POST requests to manage assessments and their
    /// attempts.  It requires authorization and interacts with various services to perform operations such as creating 
    /// assessments, starting attempts, and generating progress reports. The controller also manages file  attachments
    /// related to assessments.</remarks>
    [Authorize]
    public class AssessmentController : Controller
    {
        #region Private Fields
        private readonly IUnitOfWork _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<AssessmentController> _logger;
        private readonly IHelperService _helperService;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IBlobFileService _blobFileService;
        private readonly string _containerName;
        private readonly IAssessmentService _service;
        private readonly IFileUploadService _fileUploadService;
        private readonly IInAppNotificationService _inAppNotificationService;
        private readonly IPdfReportService _pdfReportService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentController"/> class with required dependencies.
        /// </summary>
        /// <param name="context">The unit of work instance for data access.</param>
        /// <param name="hostEnvironment">The web host environment for accessing server paths and files.</param>
        /// <param name="logger">The logger instance for logging application events.</param>
        /// <param name="helperService">The helper service providing utility functions like time handling.</param>
        /// <param name="userService">The user service for retrieving current user information.</param>
        /// <param name="pdfReportService">The service used to render progress reports as PDF documents.</param>
        public AssessmentController(IUnitOfWork context,IWebHostEnvironment hostEnvironment,ILogger<AssessmentController> logger,IHelperService helperService, IUserService userService, IStudentService studentService, IBlobFileService blobFileService, IAssessmentService service, IFileUploadService fileUploadService, IInAppNotificationService inAppNotificationService, IPdfReportService pdfReportService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger;
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService)); ;
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _blobFileService = blobFileService ?? throw new ArgumentNullException(nameof(blobFileService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _containerName = _helperService.GetConfigurationValue("AzureStorage:Containers:Assessments", string.Empty);
            _inAppNotificationService = inAppNotificationService ?? throw new ArgumentNullException(nameof(inAppNotificationService));
            _pdfReportService = pdfReportService ?? throw new ArgumentNullException(nameof(pdfReportService));
        }

        /// <summary>
        /// Handles HTTP GET requests to render the default view for the current action.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the view associated with the current action.</returns>
        [HttpGet]
        public IActionResult Attempt()
        {
            return View();
        }

        /// <summary>
        /// Displays a view for creating a new assessment.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the view for creating an assessment with a new <see
        /// cref="AssessmentConfigViewModel"/>.</returns>
        [HttpGet]
        public IActionResult CreateAssessment()
        {
            return View(new AssessmentConfigViewModel());
        }

        /// <summary>
        /// Initiates the creation of a new assessment based on the provided configuration model.
        /// </summary>
        /// <remarks>This method requires a valid <paramref name="model"/> and uses the current user's
        /// information to create the assessment. If the creation is unsuccessful, an error message is stored in <see
        /// cref="TempData"/>.</remarks>
        /// <param name="model">The configuration model containing the details required to create the assessment.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the view with the model if the model state is invalid or the
        /// creation fails; otherwise, redirects to the assessment list view upon successful creation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssessment(AssessmentConfigViewModel model)
        {
            var user = _userService.OnGetCurrentUser();

            if (!ModelState.IsValid) return View(model);
            var resp = await _service.CreateAssessmentAsync(model, $"{user?.Name} {user?.LastName}");

            if (resp.IsError)
            {
                TempData["error"] = resp.Message;
                return View(model);
            }

            TempData["success"] = "Assessment created successfully.";

            await _inAppNotificationService.SendToRoleAsync(eSysRole.Admin, $"A new assessment has been created and scheduled ({model.AssessmentType.GetDisplayName()} - {model.Title})",
                   actionUrl: Url.Action("List", "Assessment"), iconCss: "fa fa-file-alt", createdBy: $"{user?.Name} {user?.LastName}");

            return RedirectToAction(nameof(List));
        }

        /// <summary>
        /// Initiates a new attempt for the specified assessment.
        /// </summary>
        /// <remarks>This method retrieves the current user and attempts to start a new assessment
        /// attempt.  If the attempt cannot be started due to the assessment being locked or invalid,  an error message
        /// is stored in <see cref="TempData"/> and the user is redirected to the list view.</remarks>
        /// <param name="assessmentId">The unique identifier of the assessment to start an attempt for.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the attempt view if successful;  otherwise, redirects to the
        /// list view with an error message if the attempt cannot be started.</returns>
        [HttpGet]
        public async Task<IActionResult> Start(Guid assessmentId)
        {
            var user = _userService.OnGetCurrentUser() ?? new User();
            var student = await _studentService.GetStudentAsync(user.StudentNumber);

            var attemptVm = await _service.StartAttemptAsync(assessmentId, student.IDNumber ?? student.PassportNumber, null);
            if (attemptVm == null)
            {
                TempData["error"] = "Unable to start attempt (locked or invalid).";
                return RedirectToAction(nameof(List));
            }
            return View(nameof(Attempt), attemptVm);
        }

        /// <summary>
        /// Submits an assessment attempt for the current user and returns the result.
        /// </summary>
        /// <param name="request">The request containing the assessment attempt details to be submitted.</param>
        /// <returns>An <see cref="IActionResult"/> containing the assessment result, including score, percentage, time used, and
        /// review details. Returns a <see cref="BadRequestResult"/> if the submission is invalid.</returns>
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] SubmitAssessmentRequest request)
        {
            var user = _userService.OnGetCurrentUser() ?? new User();
            var student = await _studentService.GetStudentAsync(user.StudentNumber);

            var attempt = await _context.AssessmentAttempts.GetAsync(a =>
                a.Id == request.AttemptId &&
                a.LearnerIdPass == (student.IDNumber ?? student.PassportNumber));

            if (attempt is null)
            {
                return BadRequest("Attempt not found.");
            }

            if (attempt.Status is eAssessmentAttemptStatus.Submitted or eAssessmentAttemptStatus.AutoSubmitted)
            {
                var existing = await _service.GetAttemptResultAsync(request.AttemptId, includeReview: true);
                return Ok(new
                {
                    score = existing.score,
                    percentage = existing.percent,
                    timeUsed = existing.timeUsed.TotalSeconds,
                    review = existing.review
                });
            }

            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == attempt.AssessmentId);
            var timerMinutes = assessment?.TimerMinutes ?? 0;

            if (timerMinutes > 0)
            {
                var deadline = attempt.StartedUtc.AddMinutes(timerMinutes);
                if (DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime > deadline)
                {
                    request.ForcedAutoSubmit = true;
                }
            }

            var resp = await _service.SubmitAttemptAsync(request, user.IDPass);
            if (resp.IsError) return BadRequest(resp.Message);

            var result = await _service.GetAttemptResultAsync(request.AttemptId, includeReview: true);
            return Ok(new
            {
                score = result.score,
                percentage = result.percent,
                timeUsed = result.timeUsed.TotalSeconds,
                review = result.review
            });
        }

        /// <summary>
        /// Retrieves a list of assessments available to the current user, including details about each assessment and
        /// the user's attempt status.
        /// </summary>
        /// <remarks>The list includes information such as the number of completed and remaining attempts
        /// for each assessment, and whether the user can start a new attempt. The method also sets view data to
        /// indicate if the current user is a facilitator and to display the user's name.</remarks>
        /// <returns>An <see cref="IActionResult"/> that renders a view displaying the list of assessments and their details.</returns>
        public async Task<IActionResult> List()
        {
            var currentUser = _userService.OnGetCurrentUser() ?? new User();
            var isFacilitator = User.IsInRole("Facilitator") || User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var assessments = await _context.EmbeddedAssessment.GetAllAsync();

            var learnerAttempts = await _context.AssessmentAttempts
                .GetAllAsync(a => a.LearnerIdPass == currentUser.IDPass);

            var list = assessments
                .OrderByDescending(a => a.CreatedOnUtc)
                .Select(a =>
                {
                    var attemptsForAssessment = learnerAttempts.Where(l => l.AssessmentId == a.Id
                        && l.Status == eAssessmentAttemptStatus.Submitted).ToList();

                    int completed = attemptsForAssessment.Count;
                    int remaining = a.AllowRetries && a.MaxRetryAttempts.HasValue
                        ? Math.Max(a.MaxRetryAttempts.Value - completed, 0)
                        : (a.AllowRetries && !a.MaxRetryAttempts.HasValue ? int.MaxValue : 0);

                    bool scheduleOpen = !a.ScheduledDateUtc.HasValue || a.ScheduledDateUtc.Value <= DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

                    bool canStart = a.IsActive
                                    && a.IsModerated
                                    && a.ModerationApproved == true
                                    && scheduleOpen
                                    && (a.AllowRetries
                                      ? (a.MaxRetryAttempts.HasValue ? remaining > 0 : true)
                                      : completed == 0);

                    return new AssessmentListItemViewModel
                    {
                        AssessmentId = a.Id,
                        Title = a.Title,
                        TotalQuestions = a.TotalQuestions,
                        TimerMinutes = a.TimerMinutes,
                        IsPasswordProtected = a.IsPasswordProtected,
                        AllowRetries = a.AllowRetries,
                        MaxRetryAttempts = a.MaxRetryAttempts,
                        IsActive = a.IsActive,
                        ShuffleQuestions = a.ShuffleQuestions,
                        ShuffleAnswers = a.ShuffleAnswers,
                        ShowReviewAfter = a.ShowReviewAfter,
                        EnforceFullscreen = a.EnforceFullscreen,
                        MaxFocusLossAllowed = a.MaxFocusLossAllowed,
                        CompletedAttempts = completed,
                        RemainingAttempts = remaining == int.MaxValue ? -1 : remaining,
                        CanStart = canStart,
                        CreatedOnUtc = a.CreatedOnUtc,
                        CreatedBy = a.CreatedBy,
                        IsModerated = a.IsModerated,
                        ModerationApproved = a.ModerationApproved,
                        ModerationRejectionReason = a.ModerationRejectionReason,
                        ScheduledDateUtc = a.ScheduledDateUtc
                    };
                })
                .ToList();

            ViewData["isFacilitator"] = isFacilitator;
            ViewData["CurrentUserName"] = $"{currentUser?.Name} {currentUser?.LastName}";
            return View(list);
        }

        /// <summary>
        /// Displays the moderation review page showing the full assessment with all questions.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> Moderate(Guid assessmentId, CancellationToken cancellationToken = default)
        {
            if (assessmentId == Guid.Empty)
                return BadRequest("assessmentId is required.");

            var isHeadOfDepartment = await _userService.IsHeadOfDepartmentAsync(cancellationToken);

            if (!isHeadOfDepartment)
            {
                TempData["error"] = "Error: Only Heads of Department can moderate assessments.";
                return RedirectToAction(nameof(List));
            }

            var assessment = await _context.EmbeddedAssessment.GetAsync(
                a => a.Id == assessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });

            if (assessment == null)
            {
                TempData["error"] = "Error: No Assessment(s) found";
                return RedirectToAction(nameof(List));
            }

            var questionIds = assessment.Questions.Select(q => q.Id).ToList();
            var allOptions = await _context.AssessmentQuestionOptions.GetAllAsync(
                o => questionIds.Contains(o.AssessmentQuestionId));

            var vm = new AssessmentModerationViewModel
            {
                AssessmentId = assessment.Id,
                Title = assessment.Title,
                AssessmentType = assessment.AssessmentType,
                TotalQuestions = assessment.TotalQuestions,
                TimerMinutes = assessment.TimerMinutes,
                MaxScore = assessment.MaxScore,
                ShuffleQuestions = assessment.ShuffleQuestions,
                ShuffleAnswers = assessment.ShuffleAnswers,
                IsPasswordProtected = assessment.IsPasswordProtected,
                AllowRetries = assessment.AllowRetries,
                MaxRetryAttempts = assessment.MaxRetryAttempts,
                EnforceFullscreen = assessment.EnforceFullscreen,
                ShowReviewAfter = assessment.ShowReviewAfter,
                CreatedBy = assessment.CreatedBy,
                CreatedOnUtc = assessment.CreatedOnUtc,
                ScheduledDateUtc = assessment.ScheduledDateUtc,
                Questions = assessment.Questions
                    .Where(q => q.IsActive)
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new ModerationQuestionItem
                    {
                        QuestionId = q.Id,
                        DisplayOrder = q.DisplayOrder,
                        QuestionType = q.QuestionType,
                        Prompt = q.Prompt,
                        Explanation = q.Explanation,
                        ImagePath = q.ImagePath,
                        EnableAnnotation = q.EnableAnnotation,
                        Marks = q.Marks,
                        Options = allOptions
                            .Where(o => o.AssessmentQuestionId == q.Id)
                            .OrderBy(o => o.OrderIndex)
                            .Select(o => new ModerationOptionItem
                            {
                                Text = o.Text,
                                IsCorrect = o.IsCorrect
                            }).ToList()
                    }).ToList()
            };

            return View(vm);
        }

        /// <summary>
        /// Processes the moderator's approve/reject decision.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> Moderate(AssessmentModerationViewModel model)
        {
            var user = _userService.OnGetCurrentUser();
            var resp = await _service.ModerateAssessmentAsync(
                model.AssessmentId,
                model.Approved,
                $"{user?.Name} {user?.LastName}",
                model.RejectionReason);

            if (resp.IsError)
            {
                TempData["error"] = resp.Message;
                return RedirectToAction(nameof(Moderate), new { assessmentId = model.AssessmentId });
            }

            TempData["success"] = model.Approved
                ? "Assessment approved and ready for learners."
                : "Assessment rejected. The creator can revise and resubmit.";

            return RedirectToAction(nameof(List));
        }

        /// <summary>
        /// Displays the Index Page.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the Attach Assessment page for a student.
        /// </summary>
        /// <param name="studentNumber">The student number.</param>
        /// <returns>The Attach Assessment view.</returns>
        /// <summary>
        /// Retrieves the assessment attachments for a student and prepares the necessary data for the view.
        /// </summary>
        /// <param name="studentNumber">The student number to identify the student.</param>
        /// <returns>A view with the prepared assessment data or appropriate error responses.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AttachAssessment(string studentNumber)   
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                return BadRequest("Invalid student number.");
            }

            try
            {
                var student = await GetStudentDetailsAsync(studentNumber);

                if (student == null)
                {
                    _logger.LogWarning("Student with student number {StudentNumber} not found.", studentNumber);
                    return NotFound("Student not found.");
                }

                var activeEnrollment = GetActiveEnrollment(student);

                if (activeEnrollment == null)
                {
                    _logger.LogWarning("No active enrollment found for student number {StudentNumber}.", studentNumber);
                    return NotFound("No active enrollment found for the student.");
                }

                var attachments = await GetAssessmentAttachmentsAsync(studentNumber);
                var modules = await GetAvailableModulesAsync();
                var viewModel = PrepareViewModel(student, activeEnrollment, attachments);
                PrepareViewData(student, activeEnrollment, studentNumber, modules);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attaching assessment for student number {StudentNumber}.", studentNumber);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Handles the submission of an assessment attachment for a learner.
        /// </summary>
        /// <param name="attachment">The assessment attachment to save.</param>
        /// <returns>A redirect to the AttachAssessment view or an error view if saving fails.</returns>
        [HttpPost]
        public async Task<IActionResult> AttachAssessment(AssessmentAttachment attachment)
        {
            try
            {
                InitializeAttachment(attachment);

                if (!ModelState.IsValid)
                {
                    TempData["error"] = "Error: Please fill in all the fields!";
                    return View(attachment);
                }

                await UploadAssessmentAsync(attachment);

                var result = await SaveAttachmentAsync(attachment);

                if (!result.IsError)
                {
                    TempData["success"] = "Learner attachment successfully saved";
                    return RedirectToAction("AttachAssessment", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });
                }

                TempData["error"] = "Error: Unable to save assessment!";
            }

            catch (Exception ex)
            {
               // await _fileUploadService.DeleteAsync(attachment.FileURL ?? string.Empty, HttpContext.RequestAborted);
                TempData["error"] = ex.Message;
                _logger.LogError(ex.Message);
            }

            return View(attachment);
        }

        /// <summary>
        /// Retrieves a list of student assessments and prepares them for display in the view.
        /// </summary>
        /// <remarks>This method fetches all assessments from the database, enriches them with additional
        /// student information, and returns the data as a view model for rendering in the associated view.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the view populated with a list of student assessments.</returns>
        [HttpGet]
        public async Task<IActionResult> StudAssessments()
        {
            var list1 = await _context.Assessments.GetAllAsync();
            List<LearnerAssessmentViewModel> list2 = new();

            foreach (var item in list1)
            {
                var assessmentDTO = new LearnerAssessmentViewModel
                {
                    Assessment = new AssessmentViewModel
                    {
                        Module = item.Module,
                        Student = _studentService.GetStudentAsync(item.StudentNumber).GetAwaiter().GetResult().FirstName,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = item.CreatedOn,
                        Type = item.Type,
                    }
                };
                list2.Add(assessmentDTO);
            }

            return View(list2);
        }

        /// <summary>
        /// Generates a progress report for a specified student.
        /// </summary>
        /// <remarks>If no assessments are found for the specified student, the view will display a
        /// message indicating this.</remarks>
        /// <param name="studentNumber">The unique identifier of the student for whom the progress report is generated. Defaults to "FIT20221862" if
        /// not provided. Cannot be null, empty, or whitespace.</param>
        /// <returns>An <see cref="IActionResult"/> containing the progress report view for the specified student. Returns a <see
        /// cref="BadRequestResult"/> if the <paramref name="studentNumber"/> is null, empty, or whitespace.</returns>
        [HttpGet]
        public async Task<IActionResult> ProgressReport(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                return BadRequest("Student number required.");

            var report = await BuildProgressReport(studentNumber);

            if (report.TotalAssessments == 0)
            {
                ViewData["Message"] = "No assessments found for this learner.";
            }

            return View(report);
        }

        /// <summary>
        /// Generates a downloadable PDF progress report for a specified learner.
        /// </summary>
        /// <param name="studentNumber">The unique student number for the learner.</param>
        /// <returns>The learner progress report as a PDF file.</returns>
        [HttpGet]
        public async Task<IActionResult> PrintProgressReportPdf(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                return BadRequest("Student number required.");
            }

            var report = await BuildProgressReport(studentNumber);
            var pdfBytes = await _pdfReportService.RenderViewToPdfAsync(
                viewPath: "~/Views/Assessment/PrintProgressReportPdf.cshtml",
                model: report,
                options: new PdfDocumentOptions
                {
                    Title = $"Progress Report - {studentNumber}",
                    DisplayHeader = false,
                    DisplayFooter = false
                });

            return File(pdfBytes, "application/pdf", $"progress-report-{studentNumber}.pdf");
        }

        /// <summary>
        /// Updates an assessment's module and percentage for a specific student.
        /// </summary>
        /// <param name="AttachmentId">The unique identifier of the assessment attachment.</param>
        /// <param name="ModuleId">The unique identifier of the module to associate with the assessment.</param>
        /// <param name="Percentage">The percentage score to assign to the assessment.</param>
        /// <param name="StudentNumber">The student's unique number.</param>
        /// <param name="StudentId">The unique identifier of the student.</param>
        /// <returns>Returns a redirection result to the AttachAssessment view with success or error feedback.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAssessment(Guid AttachmentId, Guid ModuleId, double Percentage, string StudentNumber, Guid StudentId)
        {
            if (AttachmentId == Guid.Empty || StudentId == Guid.Empty || ModuleId == Guid.Empty)
            {
                TempData["error"] = "Invalid input parameters.";
                return RedirectToAction("AttachAssessment", new { StudentNumber, StudentId });
            }

            if (Percentage < 0 || Percentage > 100)
            {
                TempData["error"] = "Percentage must be between 0 and 100.";
                return RedirectToAction("AttachAssessment", new { StudentNumber, StudentId });
            }

            try
            {
                var attachment = await _context.Assessments.GetAsync(filter: a => a.AttachmentId == AttachmentId && a.StudentId == StudentId);
                if (attachment == null)
                {
                    TempData["error"] = "Assessment not found or you do not have permission to update it.";
                    return RedirectToAction("AttachAssessment", new { StudentNumber, StudentId });
                }

                attachment.ModuleId = ModuleId;
                attachment.Percentage = Percentage;

                var currentUser = _userService.OnGetCurrentUser();
                attachment.ModifiedBy = currentUser != null ? $"{currentUser.Name} {currentUser.LastName}" : "Unknown";
                attachment.ModifiedOn = _helperService.GetCurrentTime().ToShortDateString();

                await _context.SaveAsync();
                TempData["success"] = "Assessment updated successfully.";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating assessment for AttachmentId: {AttachmentId}", AttachmentId);
                TempData["error"] = "The assessment was modified by another user. Please refresh and try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating assessment for AttachmentId: {AttachmentId}", AttachmentId);
                TempData["error"] = "An unexpected error occurred while updating the assessment.";
            }

            return RedirectToAction("AttachAssessment", new { StudentNumber, StudentId });
        }

        /// <summary>
        /// Downloads the assessment file with the specified file ID.
        /// </summary>
        /// <param name="fileId">The ID of the file to download.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>An IActionResult representing the file download or a NotFound result if the file does not exist.</returns>
        public async Task<IActionResult> DownloadAssessment(string fileId, CancellationToken ct)
        {
            var download = await _fileUploadService.DownloadIfPresentAsync(fileId, ct);
            if (download is null) return NotFound();
            return File(download.Value.FileStream, download.Value.ContentType ?? "application/octet-stream");
        }

        /// <summary>
        /// Retrieves the results for a specified assessment, including all submitted or auto-submitted attempts.
        /// </summary>
        /// <remarks>This action is accessible only to users in the Facilitator, Admin, or SuperAdmin
        /// roles. The results include all attempts with a status of Submitted or AutoSubmitted for the specified
        /// assessment.</remarks>
        /// <param name="assessmentId">The unique identifier of the assessment for which to retrieve results. Must not be an empty GUID.</param>
        /// <returns>An IActionResult that renders the assessment results view if the assessment exists; otherwise, a BadRequest
        /// result if the assessmentId is invalid or a NotFound result if the assessment does not exist.</returns>
        [HttpGet]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> Results(Guid assessmentId)
        {
            if (assessmentId == Guid.Empty)
            {
                return BadRequest("assessmentId is required.");
            }

            var assessment = await _context.EmbeddedAssessment.GetAsync(a => a.Id == assessmentId);
            if (assessment == null)
            {
                return NotFound("Assessment not found.");
            }

            var attempts = await _context.AssessmentAttempts.GetAllAsync(a =>
                a.AssessmentId == assessmentId &&
                (a.Status == eAssessmentAttemptStatus.Submitted || a.Status == eAssessmentAttemptStatus.AutoSubmitted));

            var students = await _studentService.GetStudentListAsync();
            var studentLookup = BuildLearnerIdPassToStudentLookup(students);

            var vm = new AssessmentResultsViewModel
            {
                AssessmentId = assessment.Id,
                Title = assessment.Title,
                Rows = attempts
                    .OrderByDescending(a => a.SubmittedUtc ?? a.StartedUtc)
                    .Select(a => new AssessmentResultRowViewModel
                    {
                        AttemptId = a.Id,
                        LearnerIdPass = a.LearnerIdPass,
                        StudentDisplayName = studentLookup.TryGetValue(a.LearnerIdPass, out var display)
                            ? display
                            : a.LearnerIdPass,
                        StartedUtc = a.StartedUtc,
                        SubmittedUtc = a.SubmittedUtc,
                        Status = a.Status.ToString(),
                        FinalScore = a.FinalScore,
                        Percentage = a.Percentage
                    })
                    .ToList()
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> Grade(Guid attemptId)
        {
            if (attemptId == Guid.Empty)
            {
                return BadRequest("attemptId is required.");
            }

            var attempt = await _context.AssessmentAttempts.GetAsync(
                a => a.Id == attemptId,
                includeProperties: new[] { nameof(AssessmentAttempt.Answers) });

            if (attempt == null)
            {
                return NotFound("Attempt not found.");
            }

            var assessment = await _context.EmbeddedAssessment.GetAsync(
                a => a.Id == attempt.AssessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });

            if (assessment == null)
            {
                return NotFound("Assessment not found.");
            }

            var students = await _studentService.GetStudentListAsync();
            var studentLookup = BuildLearnerIdPassToStudentLookup(students);

            var shortAnswerQuestions = assessment.Questions
                .Where(q => q.IsActive && q.QuestionType == eAssessmentQuestionType.ShortAnswer)
                .OrderBy(q => q.DisplayOrder)
                .ToList();

            var answersByQuestionId = attempt.Answers.ToDictionary(a => a.AssessmentQuestionId, a => a);

            var mcqCorrect = attempt.Answers.Count(a => a.IsCorrect == true);

            var vm = new AssessmentGradeViewModel
            {
                AssessmentId = assessment.Id,
                AttemptId = attempt.Id,
                AssessmentTitle = assessment.Title,
                LearnerIdPass = attempt.LearnerIdPass,
                StudentDisplayName = studentLookup.TryGetValue(attempt.LearnerIdPass, out var display) ? display : attempt.LearnerIdPass,
                TotalQuestions = assessment.TotalQuestions,
                McqCorrectCount = mcqCorrect,
                CurrentFinalScore = attempt.FinalScore ?? 0,
                CurrentPercentage = attempt.Percentage ?? 0,
                ShortAnswers = shortAnswerQuestions.Select(q =>
                {
                    answersByQuestionId.TryGetValue(q.Id, out var ans);

                    var maxMarks = q.Marks <= 0 ? 1 : q.Marks;

                    return new GradeShortAnswerRowViewModel
                    {
                        QuestionId = q.Id,
                        Prompt = q.Prompt,
                        ImagePath = q.ImagePath,
                        StudentAnswer = ans?.ShortAnswerValue,
                        MaxMarks = maxMarks,
                        DiagramAnnotationJson = ans?.DiagramAnnotationJson,
                        DiagramAnnotationSnapshotFileId = ans?.DiagramAnnotationSnapshotFileId,

                        MarksAwarded = ans?.MarksAwarded is int awarded ? Math.Clamp(awarded, 0, maxMarks) : 0
                    };
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> Grade(AssessmentGradeViewModel model)
        {
            if (model == null || model.AttemptId == Guid.Empty)
            {
                return BadRequest("Invalid grade payload.");
            }

            var attempt = await _context.AssessmentAttempts.GetAsync(
                a => a.Id == model.AttemptId,
                includeProperties: new[] { nameof(AssessmentAttempt.Answers) });

            if (attempt == null)
            {
                return NotFound("Attempt not found.");
            }

            var assessment = await _context.EmbeddedAssessment.GetAsync(
                a => a.Id == attempt.AssessmentId,
                includeProperties: new[] { nameof(Assessment.Questions) });

            if (assessment == null)
            {
                return NotFound("Assessment not found.");
            }

            var questionsById = assessment.Questions.ToDictionary(q => q.Id, q => q);

            if (model.ShortAnswers != null)
            {
                foreach (var row in model.ShortAnswers)
                {
                    if (!questionsById.TryGetValue(row.QuestionId, out var q))
                    {
                        continue;
                    }

                    if (q.QuestionType != eAssessmentQuestionType.ShortAnswer)
                    {
                        continue;
                    }

                    var max = q.Marks <= 0 ? 1 : q.Marks;
                    var awarded = Math.Clamp(row.MarksAwarded, 0, max);

                    var ans = attempt.Answers.FirstOrDefault(a => a.AssessmentQuestionId == row.QuestionId);
                    if (ans != null)
                    {
                        ans.MarksAwarded = awarded;
                    }
                }
            }

            var mcqScore = attempt.Answers
                .Where(a => a.IsCorrect == true)
                .Sum(a =>
                {
                    if (questionsById.TryGetValue(a.AssessmentQuestionId, out var q))
                    {
                        return q.Marks <= 0 ? 1 : q.Marks;
                    }

                    return 0;
                });

            var shortAnswerScore = attempt.Answers
                .Where(a =>
                    questionsById.TryGetValue(a.AssessmentQuestionId, out var q) &&
                    q.QuestionType == eAssessmentQuestionType.ShortAnswer)
                .Sum(a => Math.Max(0, a.MarksAwarded ?? 0));

            var finalScore = mcqScore + shortAnswerScore;

            attempt.FinalScore = finalScore;

            var denom = assessment.MaxScore > 0 ? assessment.MaxScore : assessment.TotalQuestions;

            attempt.Percentage = denom > 0
                ? Math.Round((double)finalScore / denom * 100, 2)
                : 0;

            await _context.SaveAsync();

            TempData["success"] = "Grade saved.";
            return RedirectToAction(nameof(Results), new { assessmentId = attempt.AssessmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> UploadQuestionImage(IFormFile file, CancellationToken ct)
        {
            if (file is null || file.Length <= 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only image uploads are allowed.");
            }

            await using var stream = file.OpenReadStream();

            var upload = await _fileUploadService.UploadAsync(new UploadFileRequest(
                FileStream: stream,
                FileName: file.FileName,
                ContentType: file.ContentType,
                Metadata: new Dictionary<string, string>
                {
                    ["Entity"] = "AssessmentQuestion",
                    ["Purpose"] = "QuestionImage"
                },
                ProviderHint: null,
                ExpiryDate: null,
                TenantId: null,
                DocumentType: "AssessmentQuestionImage"
            ), ct).ConfigureAwait(false);

            return Ok(new { fileId = upload.FileId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> QuestionImage(string fileId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return BadRequest("fileId is required.");
            }

            DownloadFileResponse download;
            try
            {
                download = await _fileUploadService.DownloadAsync(fileId, ct).ConfigureAwait(false);
            }
            catch
            {
                return NotFound();
            }

            var contentType = string.IsNullOrWhiteSpace(download.ContentType) ? "application/octet-stream" : download.ContentType;

            return File(download.FileStream, contentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UploadAnnotationSnapshot(IFormFile file, CancellationToken ct)
        {
            if (file is null || file.Length <= 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only image uploads are allowed.");
            }

            await using var stream = file.OpenReadStream();

            var upload = await _fileUploadService.UploadAsync(new UploadFileRequest(
                FileStream: stream,
                FileName: file.FileName,
                ContentType: file.ContentType,
                Metadata: new Dictionary<string, string>
                {
                    ["Entity"] = "AssessmentAttemptAnswer",
                    ["Purpose"] = "DiagramAnnotationSnapshot"
                },
                ProviderHint: null,
                ExpiryDate: null,
                TenantId: null,
                DocumentType: "AssessmentDiagramAnnotationSnapshot"
            ), ct).ConfigureAwait(false);

            return Ok(new { fileId = upload.FileId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AnnotationSnapshot(string fileId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return BadRequest("fileId is required.");
            }

            DownloadFileResponse download;
            try
            {
                download = await _fileUploadService.DownloadAsync(fileId, ct).ConfigureAwait(false);
            }
            catch
            {
                return NotFound();
            }

            var contentType = string.IsNullOrWhiteSpace(download.ContentType)
                ? "application/octet-stream"
                : download.ContentType;

            return File(download.FileStream, contentType);
        }

        #region Private

        /// <summary>
        /// 
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        private static Dictionary<string, string> BuildLearnerIdPassToStudentLookup(IEnumerable<Student> students)
        {
            var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var s in students)
            {
                var display = BuildStudentDisplayName(s);

                void add(string? key)
                {
                    if (string.IsNullOrWhiteSpace(key)) return;
                    if (!lookup.ContainsKey(key)) lookup[key] = display;
                }

                add(s.IDNumber);
                add(s.PassportNumber);
            }

            return lookup;
        }

        private static string BuildStudentDisplayName(Student s)
        {
            var full = $"{s.FirstName} {s.LastName}".Trim();
            if (!string.IsNullOrWhiteSpace(full))
            {
                return full;
            }

            if (!string.IsNullOrWhiteSpace(s.StudentNumber))
            {
                return s.StudentNumber;
            }

            return "Unknown Student";
        }

        /// <summary>
        /// Retrieves student details by student number.
        /// </summary>
        /// <param name="studentNumber">The student number.</param>
        /// <returns>A <see cref="Student"/> object or null if not found.</returns>
        private async Task<Student> GetStudentDetailsAsync(string studentNumber)
        {
            if (studentNumber.Contains("FIT-DUMMY"))
            {
               return await _studentService.GetDummyStudentAsync(studentNumber);
            }

            return await _studentService.GetStudentAsync(studentNumber); 
        }

        /// <summary>
        /// Retrieves the active enrollment for the given student.
        /// </summary>
        /// <param name="student">The student object.</param>
        /// <returns>The active enrollment or null if not found.</returns>
        private EnrollmentHistory? GetActiveEnrollment(Student student)
        {
            return student.EnrollmentHistory?.FirstOrDefault(e => e.IsActive);
        }

        /// <summary>
        /// Retrieves assessment attachments for a given student number.
        /// </summary>
        /// <param name="studentNumber">The student number.</param>
        /// <returns>A list of <see cref="AssessmentAttachment"/> objects.</returns>
        private async Task<List<AssessmentAttachment>> GetAssessmentAttachmentsAsync(string studentNumber)
        {
            var attachments = await _context.Assessments.GetAllAsync();

            return attachments.Where(a => a.StudentNumber == studentNumber).ToList();
        }

        /// <summary>
        /// Retrieves a list of available modules for selection.
        /// </summary>
        /// <returns>A list of select items representing modules.</returns>
        private async Task<IEnumerable<SelectListItem>> GetAvailableModulesAsync()
        {
            var moduleList = await _context.Modules.GetAllAsync();

            return moduleList
                .Where(m => !string.IsNullOrEmpty(m.ModuleName))
                .OrderBy(m => m.ModuleName)
                .Select(m => new SelectListItem
                {
                    Value = m.ModuleId.ToString(),
                    Text = m.ModuleName
                });
        }

        /// <summary>
        /// Prepares the dynamic view model for the view.
        /// </summary>
        /// <param name="student">The student object.</param>
        /// <param name="activeEnrollment">The active enrollment.</param>
        /// <param name="attachments">The list of assessment attachments.</param>
        /// <returns>A dynamic view model object.</returns>
        private dynamic PrepareViewModel(Student student, EnrollmentHistory activeEnrollment, List<AssessmentAttachment> attachments)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.FileModel = attachments;

            return viewModel;
        }

        /// <summary>
        /// Prepares the ViewData for the view.
        /// </summary>
        /// <param name="student">The student object.</param>
        /// <param name="activeEnrollment">The active enrollment.</param>
        /// <param name="studentNumber">The student number.</param>
        /// <param name="modules">The list of available modules.</param>
        private void PrepareViewData(Student student, EnrollmentHistory activeEnrollment, string studentNumber, IEnumerable<SelectListItem> modules)
        {
            ViewData["StudentNumber"] = studentNumber;
            ViewData["name"] = $"{student.FirstName} {student.LastName}";
            ViewData["StudentId"] = student.StudentId.ToString();
            ViewData["course"] = activeEnrollment.CourseTitle;
            ViewBag.ModuleId = new SelectList(modules, "Value", "Text");
        }

        /// <summary>
        /// Initializes the properties of the attachment.
        /// </summary>
        /// <param name="attachment">The attachment object to initialize.</param>
        private void InitializeAttachment(AssessmentAttachment attachment)
        {
            attachment.AttachmentId = Helper.GenerateGuid();
            attachment.IsActive = true;
            attachment.CreatedBy = $"{_userService.OnGetCurrentUser()?.Name} {_userService.OnGetCurrentUser()?.LastName}";
            attachment.CreatedOn = _helperService.GetCurrentTime().ToString();
        }

        #region New File Upload Service Methods
        /// <summary>
        /// Uploads the report document file to Azure Blob Storage.
        /// </summary>
        /// <param name="report">The report containing the file.</param>
        /// <returns>Returns the uploaded file URL if successful, otherwise an empty string.</returns>
        private async Task<string> UploadAssessmentAsync(AssessmentAttachment attachment)
        {
            var fileId = await _fileUploadService.UploadIfPresentAsync(
                 file: attachment.AttachmentFile,
                 documentType: "Assessment",
                 metadata: new Dictionary<string, string>
                 {
                     ["Entity"] = "AssessmentAttachment",
                     ["AttachmentId"] = attachment.AttachmentId.ToString("D"),
                     ["StudentNumber"] = attachment.StudentNumber ?? string.Empty,
                     ["StudentId"] = attachment.StudentId.ToString() ?? string.Empty,
                 },
                 ct: HttpContext.RequestAborted).ConfigureAwait(false);

            attachment.Document = fileId;
            attachment.FileURL = fileId;

            return fileId ?? string.Empty;
        }
        #endregion

        /// <summary>
        /// Saves the attachment to the database.
        /// </summary>
        /// <param name="attachment">The attachment object to save.</param>
        /// <returns>A boolean indicating whether the save was successful.</returns>
        private async Task<ValidationResponse> SaveAttachmentAsync(AssessmentAttachment attachment)
        {
            if (attachment == null)
            {
                _logger.LogWarning("Attempted to save a null attachment.");
                return _helperService.ErrorResponse("Attachment cannot be null.");
            }

            if (string.IsNullOrEmpty(attachment.StudentNumber))
            {
                _logger.LogWarning("Attachment missing student number.");
                return _helperService.ErrorResponse("Student number is required.");
            }

            try
            {
                await _context.Assessments.AddAsync(attachment);

                int rowsAffected = await _context.SaveAsync();

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Successfully saved attachment for student {StudentNumber}.", attachment.StudentNumber);
                    return _helperService.SuccessResponse("Success: Assessment successfully uploaded and saved.");
                }

                _logger.LogWarning("No rows affected when saving attachment for student {StudentNumber}.", attachment.StudentNumber);
                return _helperService.ErrorResponse($"Failed to save attachment for student {attachment.StudentNumber}.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving attachment for student {StudentNumber}.", attachment.StudentNumber);
                return _helperService.ErrorResponse($"Failed to save attachment for student {attachment.StudentNumber} due to a database error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving attachment for student {StudentNumber}.", attachment.StudentNumber);
                return _helperService.ErrorResponse("An unexpected error occurred while saving the attachment.");
            }
        }

        /// <summary>
        /// Builds a progress report for a learner based on their assessments.
        /// </summary>
        /// <remarks>This method retrieves all assessments associated with the specified student,
        /// organizes them in descending order of creation date, and calculates relevant details such as scores and
        /// maximum scores for each assessment. The generated report includes metadata such as the student's name,
        /// course title, and the date the report was generated.</remarks>
        /// <param name="studentNumber">The unique identifier of the student for whom the progress report is being generated.</param>
        /// <returns>A <see cref="LearnerProgressReportViewModel"/> containing the learner's progress report, including their
        /// name, course title, and a list of their assessments.</returns>
        private async Task<LearnerProgressReportViewModel> BuildProgressReport(string studentNumber)
        {
            var all = await _context.Assessments.GetAllAsync();
            var learnerAssessments = all.Where(a => a.StudentNumber == studentNumber)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .ToList();

            string studentDisplay = _studentService.GetStudentAsync(studentNumber).GetAwaiter().GetResult().FirstName; 
            var (studentName, courseTitle) = SplitStudentDisplay(studentDisplay);

            var vm = new LearnerProgressReportViewModel
            {
                StudentNumber = studentNumber,
                StudentName = studentName,
                CourseTitle = courseTitle,
                GeneratedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime
            };

            foreach (var a in learnerAssessments)
            {
                decimal? score = TryGetDecimal(a, "Percentage");
                decimal? max = TryGetDecimal(a, "MaxScore") ?? 100; 

                vm.Assessments.Add(new LearnerAssessmentResultViewModel
                {
                    Id = a.AttachmentId,
                    Module = await ConvertModuleToString(a.ModuleId),
                    AssessmentType = a.Type,
                    CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                    Score = score,
                    MaxScore = max
                });
            }

            return vm;
        }

        /// <summary>
        /// Splits a student display string into the student's name and course.
        /// </summary>
        /// <remarks>This method assumes that the square brackets, if present, enclose the course
        /// information. Any leading or trailing whitespace around the name or course is trimmed.</remarks>
        /// <param name="display">A string representing the student's display information, where the name is optionally followed by the course
        /// enclosed in square brackets (e.g., "John Doe [Math 101]").</param>
        /// <returns>A tuple containing the student's name and course. The name is extracted from the portion of the string
        /// before the square brackets, and the course is extracted from within the square brackets. If no square
        /// brackets are present, the course will be an empty string.</returns>
        private static (string name, string course) SplitStudentDisplay(string display)
        {
            int i1 = display.IndexOf('[');
            int i2 = display.IndexOf(']');
            string name = i1 > 0 ? display[..i1].Trim() : display;
            string course = (i1 >= 0 && i2 > i1) ? display.Substring(i1 + 1, i2 - i1 - 1) : string.Empty;
            return (name, course);
        }

        /// <summary>
        /// Attempts to retrieve the value of a specified property from an object and convert it to a <see
        /// cref="decimal"/>.
        /// </summary>
        /// <param name="obj">The object containing the property to retrieve. Cannot be <see langword="null"/>.</param>
        /// <param name="propName">The name of the property to retrieve. Cannot be <see langword="null"/> or empty.</param>
        /// <returns>The <see cref="decimal"/> value of the specified property if it exists, is not <see langword="null"/>,  and
        /// can be successfully converted to a <see cref="decimal"/>; otherwise, <see langword="null"/>.</returns>
        private static decimal? TryGetDecimal(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop == null) return null;
            var raw = prop.GetValue(obj);
            if (raw == null) return null;
            return decimal.TryParse(raw.ToString(), out var d) ? d : null;
        }

        /// <summary>
        /// Converts the specified module identifier to its corresponding module name.
        /// </summary>
        /// <remarks>This method retrieves the module from the data context using the provided identifier.
        /// If no module is found with the specified identifier, the method returns "Unknown Module".</remarks>
        /// <param name="ModuleId">The unique identifier of the module to retrieve.</param>
        /// <returns>A string representing the name of the module if found; otherwise, "Unknown Module".</returns>
        private async Task<string> ConvertModuleToString(Guid ModuleId)
        {
            var module = await _context.Modules.GetAsync(filter: m => m.ModuleId == ModuleId);
            return module != null ? module.ModuleName : "Unknown Module";
        }

        #endregion
    }
}


