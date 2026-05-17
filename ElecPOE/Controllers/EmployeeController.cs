// <copyright file="EmployeeController.cs" company="Forek ICT Services">
//     Copyright © TFG.
// </copyright>
// Created By:      HO\ItumelengO (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the EmployeeController class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Controller to manage employee-related operations such as files, information, and attachments.
    /// </summary>
    [Authorize(Roles = "Admin,Facilitator")]
    public class EmployeeController : Controller
    {
        #region Private Readonly
        private readonly IUnitOfWork _context;
        private readonly IPayslipRequestService _payslipRequestService;
        private readonly IHelperService _helperService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IFileUploadService _fileUploadService;
        private readonly IUserService _userService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="context">The unit of work used to manage database transactions and repositories.</param>
        /// <param name="payslipRequestService">The service used to handle payslip-related requests and operations.</param>
        /// <param name="helperService">A helper service for retrieving configuration values and performing utility operations.</param>
        /// <param name="logger">The logger instance used for logging messages and errors related to the <see cref="EmployeeController"/>.</param>
        /// <param name="fileUploadService">The service used for provider-agnostic file upload and download operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if any required parameter is <see langword="null"/>.</exception>
        public EmployeeController(IUnitOfWork context, IUserService userService,
                                IPayslipRequestService payslipRequestService,
                                IHelperService helperService,
                                ILogger<EmployeeController> logger,
                                IFileUploadService fileUploadService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _payslipRequestService = payslipRequestService ?? throw new ArgumentNullException(nameof(payslipRequestService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Displays the Employees page.
        /// </summary>
        /// <returns>The Employees view.</returns>
        public IActionResult Employees()
        {
            return View();
        }

        /// <summary>
        /// Displays employee files based on their ID.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>The EmployeeFiles view with employee-specific data.</returns>
        [HttpGet]
        public async Task<IActionResult> EmployeeFiles(string employeeId)
        {
            var decryptId = EncryptionHelper.Decrypt(employeeId);

            var users = await _context.Users.GetAllAsync();

            var reports = await _context.Reports.GetAllAsync();

            var plans = await _context.LessonPlans.GetAllAsync();

            var selectedUser = users.FirstOrDefault(u => u.IDPass == decryptId);

            if (selectedUser is null)
            {
                return NotFound();
            }

            ViewData["UserId"] = selectedUser.Id;
            ViewData["Id"] = decryptId;
            ViewData["Name"] = $"{selectedUser.Name} {selectedUser.LastName}";

            var files = await _context.Files.GetAllAsync();

            ViewData["FileCounts"] = GenerateFileCounts(files, selectedUser.Id);

            ViewData["annual"] = reports.Count(r => r.ReportType == ReportType.Annual && r.IdPass == selectedUser.IDPass);
            ViewData["monthly"] = reports.Count(r => r.ReportType == ReportType.Monthly && r.IdPass == selectedUser.IDPass);
            ViewData["weekly"] = reports.Count(r => r.ReportType == ReportType.Weekly && r.IdPass == selectedUser.IDPass);
            ViewData["plans"] = plans.Count(p => p.IdPass == selectedUser.IDPass);

            return View();
        }

        /// <summary>
        /// Handles file upload for an employee.
        /// </summary>
        /// <param name="file">The file model containing file details.</param>
        /// <returns>Redirects to EmployeeFiles or the current view with error messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeFiles(ForekOnline.Domain.Entities.File file)
        {
            file.FileId = Helper.GenerateGuid();
            var user = _userService.OnGetCurrentUser();
            file.CreatedBy = $"{user?.Name} {user?.LastName}";
            file.CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString();
            file.IsActive = true;

            if (ModelState.IsValid)
            {
                var fileId = await UploadAttachmentAsync(file, HttpContext.RequestAborted);

                if (string.IsNullOrEmpty(fileId))
                {
                    TempData["error"] = "Error: File upload failed!";
                    return View();
                }

                file.BlobFileURL = fileId;

                var uploadFile = await _context.Files.AddAsync(file);

                if (uploadFile != null)
                {
                    int rc = await _context.SaveAsync();

                    if (rc > 0)
                    {
                        TempData["success"] = $"{Helper.GetDisplayName(file.Type)} successfully saved";

                        return RedirectToAction(nameof(EmployeeFiles), new { EmployeeId = OnConvertUserId(file.UserId) });
                    }
                    else
                    {
                        TempData["error"] = $"Error: Unable to save file!!!";
                    }
                }
                else
                {
                    TempData["error"] = $"Error: An error occured!!!";

                }

            }
            else
            {
                TempData["error"] = $"(Model) Error: An error occured!!!";
            }

            return View();
        }

        /// <summary>
        /// Displays employee information.
        /// </summary>
        /// <returns>The EmployeeInfo view.</returns>
        public async Task<IActionResult> EmployeeInfo()
        {
            try
            {
                var user = _userService.OnGetCurrentUser();
                ViewData["user"] = user;

                var viewModel = new EmployeeContactsViewModel
                {
                    User = user,
                    Contacts = await _context.EmployeeContact.GetAllAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return View();
            }
        }

        /// <summary>
        /// Returns the view for the administrator employee portal.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the administrator employee portal view.</returns>
        public IActionResult AdminEmployeePortal()
        {
            return View();
        }

        /// <summary>
        /// Handles the submission of an employee payslip request.
        /// </summary>
        /// <param name="model">The payslip request data provided by the employee.</param>
        /// <returns>
        /// A redirect to the EmployeeInfo action on success, or the appropriate view with error information on failure.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if a critical error occurs.</exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeRequest(PayslipRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid data provided. Please check your inputs and try again.";

                return View(model);
            }

            try
            {
                var user = _userService.OnGetCurrentUser();
                var currentUser = user;

                if (currentUser == null)
                {
                    TempData["error"] = "User not found. Please log in and try again.";

                    return RedirectToAction(nameof(EmployeeInfo));
                }

                var response = await _payslipRequestService.CreateRequestAsync(model, currentUser.Id);

                if (response.IsError)
                {
                    TempData["error"] = response.Message;

                    return View(model);
                }

                TempData["success"] = response.Message;

                return RedirectToAction(nameof(EmployeeInfo));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the payslip request.");

                TempData["error"] = "An unexpected error occurred. Please try again later.";

                return RedirectToAction(nameof(EmployeeInfo));
            }
        }

        /// <summary>
        /// Fetches employees from a specific department.
        /// </summary>
        /// <param name="Dept">The department name.</param>
        /// <returns>The OnGetDeptEmployees view with filtered employees.</returns>
        public async Task<IActionResult> OnGetDeptEmployees(string Dept)
        {
            var employees = await _context.Users.GetAllAsync();

            var filteredEmployees = employees.Where(e => e.Department.ToString() == Dept).OrderBy(e => e.Username);

            return View(filteredEmployees);
        }

        /// <summary>
        /// Displays files for a specific user based on document type.
        /// </summary>
        /// <param name="docType">The document type.</param>
        /// <param name="UserId">The user ID.</param>
        /// <returns>The OnViewFiles view with filtered documents.</returns>
        public async Task<IActionResult> OnViewFiles(string docType, Guid UserId)
        {
            var files = await _context.Files.GetAllAsync();

            var filteredFiles = files.Where(f => f.Type.ToString() == docType && f.UserId == UserId).ToList();

            return View(filteredFiles);
        }

        /// <summary>
        /// Downloads an employee file attachment by its file ID using the file upload service.
        /// </summary>
        /// <param name="fileId">The unique file identifier returned during upload.</param>
        /// <returns>A file result for downloading the attachment, or a content message if no file ID is provided.</returns>
        public async Task<IActionResult> AttachmentDownload(string fileId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileId))
                {
                    return Content("Sorry, no attachment found!!!");
                }

                var attachment = await _fileUploadService.DownloadAsync(fileId, HttpContext.RequestAborted);

                return File(attachment.FileStream, attachment.ContentType ?? "application/octet-stream", attachment.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download file with ID {FileId}.", fileId);

                TempData["error"] = $"Error downloading file: {ex.Message}";

                return RedirectToAction(nameof(Employees));
            }
        }

        #region Private

        /// <summary>
        /// Uploads an employee file attachment using the file upload service and returns the resulting file ID.
        /// </summary>
        /// <param name="attachment">The file entity whose <see cref="ForekOnline.Domain.Entities.File.AttachmentFile"/> will be uploaded.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the upload operation.</param>
        /// <returns>The file ID string if the upload succeeds; otherwise, an empty string.</returns>
        private async Task<string> UploadAttachmentAsync(ForekOnline.Domain.Entities.File attachment, CancellationToken cancellationToken = default)
        {
            if (attachment.AttachmentFile is null || attachment.AttachmentFile.Length <= 0)
            {
                return string.Empty;
            }

            try
            {
                await using var stream = attachment.AttachmentFile.OpenReadStream();

                var uploadResponse = await _fileUploadService.UploadAsync(
                    new UploadFileRequest(
                        FileStream: stream,
                        FileName: attachment.AttachmentFile.FileName,
                        ContentType: attachment.AttachmentFile.ContentType,
                        Metadata: new Dictionary<string, string>
                        {
                            ["Entity"] = "EmployeeFile",
                            ["UserId"] = attachment.UserId.ToString("D"),
                            ["FileType"] = attachment.Type.ToString()
                        },
                        ProviderHint: null,
                        ExpiryDate: null,
                        TenantId: null,
                        DocumentType: $"Employee.{attachment.Type}"),
                    cancellationToken).ConfigureAwait(false);

                attachment.Attachment = attachment.AttachmentFile.FileName;

                return uploadResponse.FileId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload employee file for UserId {UserId}.", attachment.UserId);

                return string.Empty;
            }
        }

        /// <summary>
        /// Converts a user ID to an IDPass.
        /// </summary>
        /// <param name="UserId">The user ID.</param>
        /// <returns>The IDPass for the user.</returns>
        private string OnConvertUserId(Guid UserId)
        {
            var idPass = _context.Users?.GetAsync(filter: u => u.Id == UserId).Result.IDPass;
            return EncryptionHelper.Encrypt(idPass);
        }

        /// <summary>
        /// Generates a dictionary of file counts for a given user.
        /// </summary>
        /// <param name="files">The list of files.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>A dictionary of file type counts.</returns>
        private Dictionary<string, int> GenerateFileCounts(IEnumerable<ForekOnline.Domain.Entities.File> files, Guid userId)
        {
            return new Dictionary<string, int>
        {
            { "Timetable", files.Count(f => f.Type == eFileType.Timetable && f.UserId == userId) },
            { "TrainingSchedule", files.Count(f => f.Type == eFileType.TrainingSchedule && f.UserId == userId) },
            { "StudyMaterial", files.Count(f => f.Type == eFileType.StudyMaterial && f.UserId == userId) },
            { "AssessmentPlan", files.Count(f => f.Type == eFileType.AssessmentPlan && f.UserId == userId) },
            { "Attendance", files.Count(f => f.Type == eFileType.Attendance && f.UserId == userId) }
        };
        }

        #endregion
    }
}