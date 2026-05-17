// <copyright file="ReportController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the ReportController class

#region Using Directives
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
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Controller responsible for managing reports.
    /// </summary>
    public class ReportController : Controller
    {
        #region Private Variables
        private readonly IUnitOfWork _context;
        private readonly ILogger<ReportController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment _hostEnvironment;
        private readonly IHelperService _helperService;
        private readonly string _containerName;
        private readonly IBlobFileService _blobFileService;
        private readonly string _emailAccounts;
        private readonly IDocumentService _documentService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IInAppNotificationService _inAppNotificationService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IReportComplianceService _complianceService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController"/> class.
        /// </summary>
        /// <param name="context">Unit of work for report operations.</param>
        /// <param name="hostEnvironment">Provides information about the web hosting environment.</param>
        /// <param name="logger">Logger instance for logging.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
        public ReportController(IUnitOfWork context, IReportComplianceService complianceService,
                                IWebHostEnvironment hostEnvironment,
                                ILogger<ReportController> logger,
                                IHttpContextAccessor httpContextAccessor,
                                IHelperService helperService,
                                IBlobFileService blobFileService,
                                IDocumentService documentService,
                                IFileUploadService fileUploadService,
                                IInAppNotificationService inAppNotificationService,
                                IUserService userService,
                                IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _helperService = helperService ?? throw new ArgumentException(nameof(helperService));
            _documentService = documentService ?? throw new ArgumentException(nameof(helperService));
            _blobFileService = blobFileService ?? throw new ArgumentException(nameof(documentService));
            _containerName = _helperService.GetConfigurationValue("AzureStorage:Containers:Reports", string.Empty);
            _emailAccounts = helperService.GetConfigurationValue("EmailAccounts:GeneralManager", "EmailAccounts:ICTManager");
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _inAppNotificationService = inAppNotificationService ?? throw new ArgumentNullException(nameof(inAppNotificationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _complianceService = complianceService ?? throw new ArgumentNullException(nameof(complianceService));
        }

        /// <summary>
        /// Displays the report creation view.
        /// </summary>
        /// <param name="idPass">The ID or passport of the user.</param>
        /// <returns>A view for creating a report.</returns>
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        [HttpGet]
        public async Task<IActionResult> CreateReport(string idPass)
        {
            var decrypedId = EncryptionHelper.Decrypt(idPass);
            var users = await _context.Users.GetAllAsync();

            var facilitatorAdmins = users
                                .Where(u => u.Role == eSysRole.Facilitator || u.Role == eSysRole.Admin)
                                .Select(u => new SelectListItem
                                {
                                    Value = u.Id.ToString(),
                                    Text = $"({u.LastName.ToUpper()} {u.Name.ToUpper()} | {u.StudentNumber})"
                                }).ToList();

            var user = users.FirstOrDefault(u => u.IDPass == decrypedId);

            if (user == null)
            {
                return RedirectToAction(nameof(IncorrectID));
            }

            var compliance = await _complianceService.CalculateComplianceAsync(decrypedId, $"{user.Name} {user.LastName}");

            ViewBag.UserId = new SelectList(facilitatorAdmins, "Value", "Text");
            ViewBag.Compliance = compliance;
            ViewData["IName"] = $"{user.Name.ToUpper()} {user.LastName.ToUpper()}";
            ViewData["IDPass"] = idPass;
            ViewData["userData"] = OnGetCurrentUser()?.Id;

            return View();
        }

        /// <summary>
        /// Handles the POST request for creating a new report.
        /// Now accepts an optional "acknowledgeLate" flag and intended period from the form.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(Report report, bool acknowledgeLate = false, string? intendedPeriodStart = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error: All fields are required!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(report.IdPass))
            {
                _logger.LogError("CreateReport POST: IdPass is null or empty. Unable to process submission.");
                TempData["error"] = "Session expired or invalid request. Please try again.";
                return RedirectToAction(nameof(CreateReport), new { idPass = Request.Form["IdPass"].ToString() });
            }

            User? currentloggedInUser = OnGetCurrentUser() ?? new User();
            PrepareReportForSave(report, currentloggedInUser);

            if (!IsReportSubmissionValid(report))
            {
                TempData["error"] = "Error: Please upload a valid report document.";
                return RedirectToAction(nameof(UserReports), new { idPass = report.IdPass });
            }

            var decryptedId = EncryptionHelper.Decrypt(report.IdPass);

            var checkResult = await _complianceService.CheckSubmissionAsync(decryptedId, report.ReportType);

            if (checkResult.WouldBeLate && !acknowledgeLate)
            {
                TempData["lateCheck"] = System.Text.Json.JsonSerializer.Serialize(new
                {
                    wouldBeLate = true,
                    intendedPeriodLabel = checkResult.IntendedPeriodLabel,
                    intendedPeriodStart = checkResult.IntendedPeriodStart.ToString("o"),
                    message = checkResult.Message,
                    existingLateCount = checkResult.ExistingLateCount
                });
                TempData["error"] = checkResult.Message;
                return RedirectToAction(nameof(CreateReport), new { idPass = report.IdPass });
            }

            _complianceService.ApplyLateSubmissionFields(report, checkResult);

            if (!string.IsNullOrWhiteSpace(intendedPeriodStart) &&
                DateTime.TryParse(intendedPeriodStart, out var parsedPeriod))
            {
                var selectedMissed = checkResult.MissedPeriods
                    .FirstOrDefault(m => m.PeriodStart.Date == parsedPeriod.Date);

                if (selectedMissed is not null)
                {
                    report.IntendedPeriodStart = selectedMissed.PeriodStart;
                    report.IntendedPeriodEnd = selectedMissed.PeriodEnd;
                    report.IntendedPeriodLabel = selectedMissed.Label;
                }
            }

            report.IdPass = decryptedId;

            if (report.DocumentFile is not null && report.DocumentFile.Length > 0)
            {
                await using var stream = report.DocumentFile.OpenReadStream();

                var uploadResponse = await _fileUploadService.UploadAsync(
                    new UploadFileRequest(
                        FileStream: stream,
                        FileName: report.DocumentFile.FileName,
                        ContentType: report.DocumentFile.ContentType,
                        Metadata: new Dictionary<string, string>
                        {
                            ["Entity"] = "Report",
                            ["ReportId"] = report.ReportId.ToString("D")
                        },
                        ProviderHint: null,
                        ExpiryDate: null,
                        TenantId: null,
                        DocumentType: "Report"
                    ),
                    HttpContext.RequestAborted);

                report.ReportURL = uploadResponse.FileId;
            }

            report.ExpiryDate = _helperService.GetCurrentTime().AddDays(20);

            return await SaveReportAsync(report, currentloggedInUser);
        }
        /// <summary>
        /// AJAX endpoint: checks submission eligibility before the user submits.
        /// The view calls this when the user selects a report type, returning the check result as JSON.
        /// If the submission would be late, the view shows the acknowledgment popup.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> CheckSubmission(string idPass, ReportType reportType)
        {
            var decryptedId = EncryptionHelper.Decrypt(idPass);
            var result = await _complianceService.CheckSubmissionAsync(decryptedId, reportType);

            return Json(new
            {
                isAllowed = result.IsAllowed,
                wouldBeLate = result.WouldBeLate,
                hasMissedPeriods = result.HasMissedPeriods,
                currentPeriodAlreadyCovered = result.CurrentPeriodAlreadyCovered,
                intendedPeriodLabel = result.IntendedPeriodLabel,
                intendedPeriodStart = result.IntendedPeriodStart.ToString("o"),
                intendedPeriodEnd = result.IntendedPeriodEnd.ToString("o"),
                message = result.Message,
                existingLateCount = result.ExistingLateCount,
                missedPeriods = result.MissedPeriods.Select(m => new
                {
                    label = m.Label,
                    periodStart = m.PeriodStart.ToString("o"),
                    periodEnd = m.PeriodEnd.ToString("o")
                })
            });
        }

        /// <summary>
        /// Handles the creation of a new report along with its associated subreports.
        /// Now supports late submission acknowledgment.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReportWithSubReports(ReportSubmissionViewModel model, bool acknowledgeLate = false, string? intendedPeriodStart = null)
        {
            const int MaxSubReports = 5;

            if (model is null)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(model.Report?.IdPass))
            {
                _logger.LogError("CreateReportWithSubReports POST: Report.IdPass is null or empty.");
                TempData["error"] = "Session expired or invalid request. Please try again.";
                return RedirectToAction(nameof(CreateReport), new { idPass = Request.Form["Report.IdPass"].ToString() });
            }

            if (model.SubReports.Count > MaxSubReports)
            {
                ModelState.AddModelError(string.Empty, $"Max {MaxSubReports} subreports reached.");
            }

            model.SubReports = model.SubReports
                .Where(r => r is not null)
                .Where(r => r.DocumentFile is not null || !string.IsNullOrWhiteSpace(r.FileName) || !string.IsNullOrWhiteSpace(r.Description))
                .ToList();

            foreach (var row in model.SubReports)
            {
                if (row.DocumentFile is null || row.DocumentFile.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, $"Subreport '{row.FileName}' must include a file.");
                }
                if (string.IsNullOrWhiteSpace(row.FileName))
                {
                    ModelState.AddModelError(string.Empty, "Each subreport must have a file name.");
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error: Please fix validation errors and try again.";
                return View("CreateReport", model.Report);
            }

            var report = model.Report;

            User? currentloggedInUser = OnGetCurrentUser() ?? new User();
            var now = DateTimeHelper.GetCurrentSastDateTimeOffset();
            PrepareReportForSave(report, currentloggedInUser);

            var decryptedId = EncryptionHelper.Decrypt(report.IdPass);

            var checkResult = await _complianceService.CheckSubmissionAsync(decryptedId, report.ReportType);

            if (checkResult.WouldBeLate && !acknowledgeLate)
            {
                TempData["lateCheck"] = System.Text.Json.JsonSerializer.Serialize(new
                {
                    wouldBeLate = true,
                    intendedPeriodLabel = checkResult.IntendedPeriodLabel,
                    intendedPeriodStart = checkResult.IntendedPeriodStart.ToString("o"),
                    message = checkResult.Message,
                    existingLateCount = checkResult.ExistingLateCount
                });
                TempData["error"] = checkResult.Message;
                return RedirectToAction(nameof(CreateReport), new { idPass = report.IdPass });
            }

            _complianceService.ApplyLateSubmissionFields(report, checkResult);

            if (!string.IsNullOrWhiteSpace(intendedPeriodStart) &&
                DateTime.TryParse(intendedPeriodStart, out var parsedPeriod))
            {
                var selectedMissed = checkResult.MissedPeriods
                    .FirstOrDefault(m => m.PeriodStart.Date == parsedPeriod.Date);

                if (selectedMissed is not null)
                {
                    report.IntendedPeriodStart = selectedMissed.PeriodStart;
                    report.IntendedPeriodEnd = selectedMissed.PeriodEnd;
                    report.IntendedPeriodLabel = selectedMissed.Label;
                }
            }

            report.IdPass = decryptedId;

            if (report.DocumentFile is not null && report.DocumentFile.Length > 0)
            {
                await using var stream = report.DocumentFile.OpenReadStream();

                var uploadResponse = await _fileUploadService.UploadAsync(
                    new UploadFileRequest(
                        FileStream: stream,
                        FileName: report.DocumentFile.FileName,
                        ContentType: report.DocumentFile.ContentType,
                        Metadata: new Dictionary<string, string>
                        {
                            ["Entity"] = "Report",
                            ["ReportId"] = report.ReportId.ToString("D")
                        },
                        ProviderHint: null,
                        ExpiryDate: null,
                        TenantId: null,
                        DocumentType: "Report"
                    ),
                    HttpContext.RequestAborted);

                report.ReportURL = uploadResponse.FileId;
            }

            report.ExpiryDate = _helperService.GetCurrentTime().AddDays(20);

            var savedReport = await _context.Reports.AddAsync(report);
            if (savedReport is null || await _context.SaveAsync() <= 0)
            {
                TempData["error"] = "Failed to save report.";
                return View("CreateReport", report);
            }

            foreach (var row in model.SubReports.Take(MaxSubReports))
            {
                if (row.DocumentFile is null || row.DocumentFile.Length == 0)
                {
                    continue;
                }
                await using var subStream = row.DocumentFile.OpenReadStream();

                var subUpload = await _fileUploadService.UploadAsync(
                    new UploadFileRequest(
                        FileStream: subStream,
                        FileName: row.DocumentFile.FileName,
                        ContentType: row.DocumentFile.ContentType,
                        Metadata: new Dictionary<string, string>
                        {
                            ["Entity"] = "ReportSubReport",
                            ["ReportId"] = report.ReportId.ToString("D"),
                        },
                        ProviderHint: null,
                        ExpiryDate: null,
                        TenantId: null,
                        DocumentType: "ReportSubReport"
                    ),
                    HttpContext.RequestAborted);

                var sub = new ReportSubReport
                {
                    Id = Helper.GenerateGuid(),
                    ReportId = report.ReportId,
                    FileName = row.FileName.Trim(),
                    Description = string.IsNullOrWhiteSpace(row.Description) ? null : row.Description.Trim(),
                    FileId = subUpload.FileId,
                    IsDeleted = false,
                    Code = "SUB-REP",
                    Name = $"SubReport for {report.Reference}",
                    RowVersion = Array.Empty<byte>(),
                    DateCreated = now,
                    DateModified = now,
                    UserCreated = $"{currentloggedInUser.Name} {currentloggedInUser.LastName}",
                    UserModified = $"{currentloggedInUser.Name} {currentloggedInUser.LastName}",
                };

                await _context.ReportSubReport.AddAsync(sub);
            }

            await _context.SaveAsync();

            TempData["success"] = report.IsLateSubmission ? $"Report saved — flagged as late submission." : "Report(s) successfully saved";

            NotifyUser(currentloggedInUser, report);

            var userId = await _userService.GetUserIdByEmailAsync(_configuration["EmailAccounts:GeneralManager"]);

            var lateTag = report.IsLateSubmission ? " [LATE]" : ""; await _inAppNotificationService.SendAsync(userId.Value,
                                                                    $"{currentloggedInUser.Name} {currentloggedInUser.LastName} uploaded a {report.ReportType} report{lateTag} (Ref: {report.Reference})",
                                                                    actionUrl: Url.Action("ReportDetails", "Report", new { reportId = report.ReportId, facilitatorId = report.FacilitatorId }),
                                                                    iconCss: report.IsLateSubmission ? "fa fa-exclamation-triangle" : "fa fa-file-alt",
                                                                    createdBy: $"{currentloggedInUser.Name} {currentloggedInUser.LastName}");

            return RedirectToAction(nameof(UserReports), new { idPass = EncryptionHelper.Encrypt(report.IdPass) });
        }

        /// <summary>
        /// Handles the downloading of a report.
        /// </summary>
        /// <param name="fileId">The name of the file to be downloaded.</param>
        /// <returns>A file download response or an error message if the file is not found.</returns>
        public async Task<IActionResult> ReportDownload(string fileId, CancellationToken ct = default)
        {
            var download = await _fileUploadService.DownloadIfPresentAsync(fileId, ct).ConfigureAwait(false);

            if (download is null)
            {
                return NotFound();
            }

            return File(download.Value.FileStream, download.Value.ContentType ?? "application/octet-stream");
        }

        /// <summary>
        /// Displays a list of reports associated with a specific user.
        /// Now includes compliance data.
        /// </summary>
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> UserReports(string idPass)
        {
            var decryptedId = EncryptionHelper.Decrypt(idPass);
            var user = await OnGetUser(decryptedId);

            if (user == null)
            {
                return RedirectToAction(nameof(IncorrectID));
            }

            var userReports = await GetUserReportsAsync(decryptedId);

            var reportIds = userReports.Select(r => r.ReportId).ToList();
            var allSubReports = await _context.ReportSubReport.GetAllAsync();

            var subReportsByReport = allSubReports
                .Where(s => !s.IsDeleted && reportIds.Contains(s.ReportId))
                .GroupBy(s => s.ReportId)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.FileName).ToList());

            ViewData["SubReportsByReport"] = subReportsByReport;

            var compliance = await _complianceService.CalculateComplianceAsync(
                decryptedId,
                $"{user.Name} {user.LastName}");

            ViewBag.Compliance = compliance;
            SetUserViewData(user, idPass);

            return View(userReports);
        }

        /// <summary>
        /// Previews the report details for a given report ID.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report to preview.</param>
        /// <returns>An <see cref="IActionResult"/> representing the view with the report details.</returns>
        public async Task<IActionResult> OnPreviewReport(Guid reportId)
        {
            var report = await _context.Reports.GetAsync(filter: r => r.ReportId == reportId);
            return View(report);
        }

        /// <summary>
        /// Previews the report details for a given report ID.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report to preview.</param>
        /// <returns>An <see cref="IActionResult"/> representing the view with the report details.</returns>
        public async Task<IActionResult> OnPreviewReportFromUrl(Guid reportId)
        {
            var report = await _context.Reports.GetAsync(filter: r => r.ReportId == reportId);
            if (report == null || string.IsNullOrEmpty(report.ReportURL))
            {
                return Content("<p>No report available to preview.</p>", "text/html");
            }

            string iframeHtml = $"<iframe src=\"https://docs.google.com/gview?embedded=true&url={Uri.EscapeDataString(report.ReportURL)}\" style=\"width:100%; height:500px; border:none;\"></iframe>";
            return Content(iframeHtml, "text/html");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="facilitatorId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ReportDetails(Guid reportId, Guid facilitatorId)
        {
            _logger.LogInformation($"getting facilitator with Id = {facilitatorId}");

            try
            {
                var report = await _documentService.OpenDocumentAsync(reportId, facilitatorId);
                _logger.LogInformation($"getting report with Id = {report.Report.FacilitatorId}");

                var allSubReports = await _context.ReportSubReport.GetAllAsync();
                report.SubReports = allSubReports
                    .Where(s => !s.IsDeleted && s.ReportId == reportId)
                    .OrderBy(s => s.FileName)
                    .ToList();

                return View(report);

            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Error:Report with Id {reportId} at {MethodBase.GetCurrentMethod()?.Name} not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error:{ex.Message}");
                return StatusCode(500, $"An unexpected error occured while opening the report. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves and displays a report for modification.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report to modify.</param>
        /// <returns>An <see cref="IActionResult"/> with the report details or a redirection to a not found page.</returns>
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        [HttpGet]
        public async Task<IActionResult> ModifyReport(Guid reportId)
        {
            if (reportId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var report = await _context.Reports.GetAsync(filter: r => r.ReportId == reportId);
            if (report == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            await PopulateUserSelectListAsync();
            return View(report);
        }

        /// <summary>
        /// Modifies the details of an existing report.
        /// </summary>
        /// <param name="report">The report object with updated details.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success or failure of the operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyReport(Report report)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill in all required fields.";

                return View();
            }

            report.ModifiedBy = Helper.loggedInUser;
            report.ModifiedOn = Helper.OnGetCurrentDateTime();
            report.IsActive = false;

            var updatedReport = await _context.Reports.UpdateReportAsync(report);
            if (updatedReport != null && await _context.SaveAsync() > 0)
            {
                TempData["success"] = "Report modified successfully";
                return RedirectToAction(nameof(UserReports));
            }

            TempData["error"] = "Failed to modify report";
            return View();
        }

        /// <summary>
        /// Removes a report from the system.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report to remove.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success or failure of the operation.</returns>
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> RemoveReport(Guid reportId)
        {
            if (reportId == Guid.Empty)
            {
                _logger.LogWarning("Invalid ReportId: {ReportId}", reportId);
                TempData["error"] = "Invalid report ID.";
                return RedirectToAction("RouteNotFound", "Global");
            }

            var report = await _context.Reports.GetAsync(filter: r => r.ReportId == reportId);

            if (report == null)
            {
                _logger.LogWarning("Report with ID {ReportId} not found.", reportId);
                TempData["error"] = "Report not found.";
                return RedirectToAction("RouteNotFound", "Global");
            }

            try
            {
                var resultSuccess = await _context.Reports.RemoveAsync(report);

                if (resultSuccess)
                {
                    _helperService.DeleteLocalFile(report.Document, "Reports");
                    _logger.LogInformation("Report with ID {ReportId} removed successfully.", reportId);
                    TempData["success"] = "Report successfully removed!";

                    return RedirectToAction(nameof(UserReports), new { IdPass = EncryptionHelper.Encrypt(report.IdPass) });
                }

                _logger.LogError("Failed to remove report with ID {ReportId}. Database returned 0 rows affected.", reportId);
                TempData["error"] = "Error: Could not remove report!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to remove report with ID {ReportId}.", reportId);
                TempData["error"] = "An unexpected error occurred while attempting to remove the report.";
            }

            return RedirectToAction(nameof(UserReports));
        }

        /// <summary>
        /// Displays the view for an incorrect ID error.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the error view.</returns>
        public IActionResult IncorrectID()
        {
            return View();
        }

        /// <summary>
        /// Retrieves a report that has been liked by a user.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report.</param>
        /// <returns>A task that represents the asynchronous operation, containing the liked report.</returns>
        public async Task<IActionResult> Liked(Guid reportId)
        {
            try
            {
                var report = await _documentService.LikedReportAsync(reportId);

                return RedirectToAction(nameof(ReportDetails), new { reportId = reportId, facilitatorId = report.FacilitatorId });

            }
            catch (InvalidOperationException ex)
            {
                TempData["error"] = ex.Message;

                return RedirectToAction(nameof(ReportDetails), new { reportId });

            }
        }

        #region Private

        // <summary>
        /// Retrieves a user by their ID or Passport number.
        /// </summary>
        /// <param name="IdPass">The ID or Passport number of the user.</param>
        /// <returns>A <see cref="User"/> object matching the criteria, or a new default user if no match is found.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="IdPass"/> is null or empty.</exception>
        private async Task<User> OnGetUser(string IdPass)
        {
            if (string.IsNullOrEmpty(IdPass))
            {
                throw new ArgumentException("ID / Passport cannot be null or empty", nameof(IdPass));
            }

            var users = await _context.Users.GetAllAsync();

            return users.FirstOrDefault(u =>

                u.IsActive &&
                (u.Role == eSysRole.Facilitator || u.Role == eSysRole.Admin) &&
                u.IDPass == IdPass

            ) ?? new User();
        }

        /// <summary>
        /// Populates the user selection list with facilitators.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task PopulateUserSelectListAsync()
        {
            var users = await _context.Users.GetAllAsync();

            ViewBag.UserId = new SelectList(

                users.Where(u => u.Role == eSysRole.Facilitator).Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),

                    Text = $"{u.LastName} {u.Name} | {u.StudentNumber}"

                }), "Value", "Text");
        }

        /// <summary>
        /// Sends a notification to a user regarding a specific report.
        /// </summary>
        /// <param name="user">The user to notify.</param>
        /// <param name="report">The report associated with the notification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> or <paramref name="report"/> is null.</exception>
        private void NotifyUser(User user, Report report)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");

                throw new ArgumentNullException(nameof(user));
            }

            if (report == null)
            {
                _logger.LogError("Report cannot be null.");

                throw new ArgumentNullException(nameof(report));
            }

            var currentUser = OnGetCurrentUser();
            var userToNotify = $"{currentUser?.Name} {currentUser?.LastName}";

            try
            {
                if (!string.IsNullOrWhiteSpace(user.Cellphone) && user.Cellphone.Length == 10)
                {
                    var message = new StringBuilder()
                        .AppendLine($"Hi {user.Name}, this notification acknowledges receipt of your report")
                        .AppendLine()
                        .AppendLine($"Ref: {report.Reference}")
                        .AppendLine($"Type: {report.ReportType}")
                        .AppendLine($"Date: {report.CreatedOn}")
                        .AppendLine("Forek Online")
                        .ToString();

                    // _messageHelper.SendSms(message, user.Cellphone);
                    // _logger.LogInformation("SMS notification sent to {Cellphone}", user.Cellphone);
                }
                else
                {
                    _logger.LogWarning("Invalid or missing cellphone number for user {UserId}. SMS notification skipped.", user.Id);
                }

                Helper.OnSendMailNotification(_emailAccounts, $"Forek Online - {userToNotify}'s {report.ReportType.ToString()} Report",
                Helper.OnSendMessage($"{OnGetCurrentUser()?.Name} {OnGetCurrentUser()?.LastName}", report.ReportType.ToString(), _helperService.GetCurrentTime().ToString(), report.Reference, user.Department.ToString()), "Submission");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while notifying the user.");

                throw;
            }
        }

        /// <summary>
        /// Checks if the user has already submitted a report for the specified time period.
        /// Ensures that Monthly reports are only submitted once per month, Weekly once per week, and Annual once per year.
        /// </summary>
        /// <param name="report">The report to validate.</param>
        /// <returns>A ValidationResponse indicating success or failure.</returns>
        private async Task<ValidationResponse> HasUserSubmittedReportAsync(Report report)
        {
            if (report == null)
            {
                return _helperService.ErrorResponse("Invalid report submission.");
            }

            var now = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            var reports = await _context.Reports.GetAllAsync();

            if (reports == null || !reports.Any())
            {
                return _helperService.SuccessResponse("No previous reports found.");
            }

            var decryptedIdNumber = EncryptionHelper.Decrypt(report.IdPass);

            var userReports = reports
                            .Where(r => r.IdPass == decryptedIdNumber && r.ReportType == report.ReportType)
                            .OrderByDescending(r => _helperService.ConvertToDateTimeNoReference(r.CreatedOn) ?? DateTime.MinValue)
                            .ToList();

            if (!userReports.Any())
            {
                return _helperService.SuccessResponse("No duplicate report found.");
            }

            var userReportDates = userReports
                                   .Select(r => _helperService.ConvertToDateTimeNoReference(r.CreatedOn))
                                   .Where(d => d.HasValue)
                                   .Select(d => d.Value)
                                   .ToList();

            bool reportExists = false;

            switch (report.ReportType)
            {
                case ReportType.Monthly:

                    reportExists = userReportDates.Any(d => d.Year == now.Year && d.Month == now.Month);
                    break;

                case ReportType.Weekly:

                    var currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                    reportExists = userReportDates.Any(d =>
                        CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == currentWeek &&
                        d.Year == now.Year);
                    break;

                case ReportType.Annual:

                    reportExists = userReportDates.Any(d => d.Year == now.Year);
                    break;

                default:
                    return _helperService.ErrorResponse("Invalid report type.");
            }

            if (reportExists)
            {
                return _helperService.ErrorResponse($"A {report.ReportType} report has already been submitted for this period.");
            }

            return _helperService.SuccessResponse("No duplicate report found.");
        }

        /// <summary>
        /// Prepares a report for saving by initializing its properties.
        /// </summary>
        /// <param name="report">The report to prepare.</param>
        /// <param name="createdByUser">The user creating the report.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> or <paramref name="createdByUser"/> is null.</exception>
        private void PrepareReportForSave(Report report, User createdByUser)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            if (createdByUser == null) throw new ArgumentNullException(nameof(createdByUser));

            report.IsActive = true;
            report.ReportId = Helper.GenerateGuid();
            report.Reference = $"REP-{Helper.RandomStringGenerator(4)}";
            report.CreatedBy = $"{createdByUser.Name} {createdByUser.LastName}";
            report.CreatedOn = _helperService.GetCurrentTime().ToString();
            report.Operation = eOperation.Upload;
        }

        /// <summary>
        /// Retrieves a list of reports for a specific user based on their ID or Passport number.
        /// </summary>
        /// <param name="idPass">The ID or Passport number of the user.</param>
        /// <returns>A list of reports sorted in descending order by creation date.</returns>
        private async Task<List<Report>> GetUserReportsAsync(string idPass)
        {
            var reports = await _context.Reports.GetAllAsync();

            var reportList = reports
                            .Where(r => r.IdPass == idPass)
                            .OrderByDescending(r => ParseReportDate(r.CreatedOn))
                            .ToList();

            return reportList;
        }

        /// <summary>
        /// Sets the user-related data in the ViewData dictionary.
        /// </summary>
        /// <param name="user">The user object containing the data.</param>
        /// <param name="idPass">The ID or Passport number of the user.</param>
        private void SetUserViewData(User user, string idPass)
        {
            ViewData["UName"] = $"{user.Name.ToUpper()} {user.LastName.ToUpper()}";
            ViewData["UId"] = idPass;
            ViewData["UImage"] = user.ProfileImage;
            ViewData["facilitatorId"] = OnGetCurrentUser()?.Id;
        }

        /// <summary>
        /// Retrieves the current user from the session data.
        /// </summary>
        /// <returns>The current <see cref="User"/> object, or null if no user is found.</returns>

        /// <summary>
        /// Determines if a report of the same type already exists within the allowed submission period.
        /// </summary>
        /// <param name="reportType">The type of report (Monthly, Weekly, Annual).</param>
        /// <param name="userReports">List of user reports.</param>
        /// <param name="now">Current DateTime.</param>
        /// <returns>True if a report exists within the period, otherwise false.</returns>
        private bool ReportAlreadyExists(ReportType reportType, List<Report> userReports, DateTime now)
        {
            return userReports.Any(r =>
            {
                var reportDate = _helperService.ConvertToDateTimeNoReference(r.CreatedOn);
                return reportType switch
                {
                    ReportType.Monthly => reportDate.Value.Year == now.Year && reportDate.Value.Month == now.Month,
                    ReportType.Weekly => reportDate.Value.Year == now.Year && GetWeekOfYear(reportDate.Value) == GetWeekOfYear(now),
                    ReportType.Annual => reportDate.Value.Year == now.Year,
                    _ => false
                };
            });
        }

        /// <summary>
        /// Gets the ISO 8601 week number for a given date.
        /// </summary>
        /// <param name="date">The date to evaluate.</param>
        /// <returns>The ISO 8601 week number.</returns>
        private int GetWeekOfYear(DateTime date)
        {
            var culture = CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Saves the report to the database and handles success or failure responses.
        /// </summary>
        /// <param name="report">The report to be saved.</param>
        /// <param name="uploadedFileUrl">The uploaded file URL.</param>
        /// <returns>Returns an IActionResult indicating success or failure.</returns>
        private async Task<IActionResult> SaveReportAsync(Report report, User user)
        {
            try
            {
                var savedReport = await _context.Reports.AddAsync(report);

                if (savedReport != null && await _context.SaveAsync() > 0)
                {
                    TempData["success"] = "Report successfully saved";
                    NotifyUser(user, report);
                    return RedirectToAction(nameof(UserReports), new { idPass = EncryptionHelper.Encrypt(report.IdPass) });
                }
            }
            catch (Exception ex)
            {
                //await _blobFileService.DeleteFileAsync(uploadedFileUrl, _containerName);
                TempData["error"] = ex.Message;
            }

            return View();
        }

        /// <summary>
        /// Uploads the report document file to Azure Blob Storage.
        /// </summary>
        /// <param name="report">The report containing the file.</param>
        /// <returns>Returns the uploaded file URL if successful, otherwise an empty string.</returns>
        private async Task<IActionResult> DownloadReportFile(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return Content("Sorry, no attachment found!!!");
            }

            var attachment = await _fileUploadService.DownloadAsync(fileId, HttpContext.RequestAborted);

            return File(attachment.FileStream, attachment.ContentType ?? "application/octet-stream", attachment.FileName);

        }

        /// <summary>
        /// Validates whether the report submission is allowed.
        /// </summary>
        /// <param name="report">The report being validated.</param>
        /// <returns>Returns <c>true</c> if the submission is valid, otherwise <c>false</c>.</returns>
        private bool IsReportSubmissionValid(Report report)
        {
            return report.Operation == eOperation.Upload && report.DocumentFile != null;
        }

        private DateTime ParseReportDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                _logger.LogError("Invalid date: empty or null");
                return DateTime.MinValue; 
            }

            string[] formats =
            {
                "dddd, dd MMMM yyyy hh:mm tt", // e.g., Friday, 26 April 2024 04:32 PM
                "M/d/yyyy hh:mm:ss tt",        // e.g., 3/9/2025 10:25:01 PM
                "MM/dd/yyyy hh:mm:ss tt",      // e.g., 03/09/2025 10:25:01 PM
                "yyyy-MM-dd HH:mm:ss",         // ISO format
                "yyyy-MM-ddTHH:mm:ss",         // Another ISO format
                "dd MMM yyyy HH:mm",           // e.g., 07 Mar 2025 14:30
            };

            if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
            {
                return parsedDateTime;
            }

            if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsedDateTime))
            {
                return parsedDateTime;
            }

            DateTime? reconstructedDate = TryExtractDateFromMessyString(input);
            if (reconstructedDate.HasValue)
            {
                return reconstructedDate.Value;
            }

            _logger.LogError($"Failed to parse date: {input}");
            return DateTime.MinValue;
        }
        private DateTime? TryExtractDateFromMessyString(string input)
        {
            var dateRegex = new Regex(@"\b(\d{1,2})[/-](\d{1,2})[/-](\d{4})\b");
            var timeRegex = new Regex(@"\b(\d{1,2}):(\d{2}):?(\d{2})?\s?(AM|PM)?\b", RegexOptions.IgnoreCase);

            Match dateMatch = dateRegex.Match(input);
            Match timeMatch = timeRegex.Match(input);

            if (!dateMatch.Success) return null; 

            int day = int.Parse(dateMatch.Groups[1].Value);
            int month = int.Parse(dateMatch.Groups[2].Value);
            int year = int.Parse(dateMatch.Groups[3].Value);

            int hour = 0, minute = 0, second = 0;
            bool isPM = false;

            if (timeMatch.Success)
            {
                hour = int.Parse(timeMatch.Groups[1].Value);
                minute = int.Parse(timeMatch.Groups[2].Value);
                if (timeMatch.Groups[3].Success)
                {
                    second = int.Parse(timeMatch.Groups[3].Value);
                }
                isPM = timeMatch.Groups[4].Success && timeMatch.Groups[4].Value.Equals("PM", StringComparison.OrdinalIgnoreCase);
            }

            if (isPM && hour < 12) hour += 12;
            if (!isPM && hour == 12) hour = 0;

            return new DateTime(year, month, day, hour, minute, second);
        }

        private User? OnGetCurrentUser()
        {
            return _userService.OnGetCurrentUser();
        }

        #endregion

    }
}


