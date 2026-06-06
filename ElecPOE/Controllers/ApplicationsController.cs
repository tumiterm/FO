// <copyright file="ApplicationsController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the ApplicationsController class

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Manages application-related operations, including viewing, applying, and processing applications.
    /// </summary>
    /// <remarks>This controller handles various actions related to applications, such as retrieving dashboard
    /// data, managing application submissions, and processing application statuses. It interacts with services for
    /// database operations, file management, and logging. The controller also provides functionality for downloading
    /// files and sending notifications.</remarks>
    public class ApplicationsController : Controller
    {
        #region Private Fields
        private readonly IUnitOfWork _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<ApplicationsController> _logger;
        private readonly IHelperService _helperService;
        private readonly IApplicationsService _applicationsService;
        private readonly IBlobFileService _blobFileService;
        private readonly string _containerName;
        private static readonly HashSet<Guid> TradesAndNonTradesCourseIds = new()
        {
            Guid.Parse("5F94C7CA-CF9C-4B12-859E-0B2C58C3031A"),
            Guid.Parse("C47D8114-7F73-46A2-B5D4-15F860D1F907"),
            Guid.Parse("EDCBDB89-22DF-4736-9740-25820564DED5"),
            Guid.Parse("27B0905A-F1D0-4587-A7CD-2FF40D7FDD50"),
            Guid.Parse("A94468D0-8F20-49B3-86E9-3C0139D45CF5"),
            Guid.Parse("2EA1A2A7-EB0A-4754-988A-4B6737A42928"),
            Guid.Parse("92E378BE-0976-4AF3-8FCD-6C0DDEF92393"),
            Guid.Parse("2A150F05-861F-4912-B648-71E71D84D870"),
            Guid.Parse("784E06C2-5103-4EC6-8B47-776661DC813E"),
            Guid.Parse("6854D25D-7CF0-41C1-928D-935C6592BC6F"),
            Guid.Parse("8E2D3FCF-E6B5-4212-8C33-A65C5E533502"),
            Guid.Parse("DC469368-1956-4BEE-9C00-B8F039906DB5"),
            Guid.Parse("22C29976-392B-4247-9474-D4C8462F0C1B"),
            Guid.Parse("2ACFCBA2-0476-46CD-A5F8-E043071B71F8"),
            Guid.Parse("AE5F8CE6-3521-4E5F-8E68-EE81556DA59C"),
            Guid.Parse("DDD586CA-A67A-45AB-A206-F9F05DA9E422"),
            Guid.Parse("48B0CFE6-EE27-4D2C-8239-C3DD7C5D564F"),
            Guid.Parse("F3DFFCE8-2312-4013-AE8A-7224C634EF18"),
            Guid.Parse("E63E019A-4FF4-4E67-9DE2-F5FCAC2A41EA"),
            Guid.Parse("B21E2B97-35C7-4CE0-902F-2B035B92FD28"),
            Guid.Parse("FA7ABC47-5745-4C46-BA61-B693F700913A"),
            Guid.Parse("99F4DD0C-1FCB-40CD-8B4F-E441106BD116"),
            Guid.Parse("6060FAD1-A953-4846-8F9C-357CADAEBCA8"),
            Guid.Parse("B8AC2658-D26A-46B0-9DCF-ABD69DC759B0"),
            Guid.Parse("1FD160DA-33DA-4405-B8B3-B9ABF5D2FF39"),
            Guid.Parse("D2E883F8-785B-42CA-9A5A-278C80996E19"),
            Guid.Parse("C8D09822-1898-4010-85F9-F4AE8C1657A7"),
            Guid.Parse("7C59E964-8C58-4C7D-91ED-0A3268DA1002"),
            Guid.Parse("DB269864-ECF7-46BE-A6F0-6CC3AA5200E5"),
            Guid.Parse("9E358CF0-71A7-4E4D-86B0-7D99AD70B138"),
            Guid.Parse("8E50E683-8C16-4016-B79C-8812D7C5CCB1"),

        };
        private static readonly HashSet<Guid> ShortSkillsTradeTestCourseIds = new()
        {
            #region Short Skills

            Guid.Parse("713cbc32-982a-44ef-98db-0a147ccb8439"), //scafolding
            Guid.Parse("bb4e521c-bad0-4f02-aeba-1161d12ac35e"), //SHE Rep
            Guid.Parse("b1c8e8eb-9bfc-4d3a-a33a-19b866affc3b"), //Conveyance of Dangerous Goods
            Guid.Parse("c28356c2-bea0-4e7a-8f01-294fd5c875e2"), //Hot and Cold Water Systems
            Guid.Parse("14709bd1-856c-4d46-8e1b-2e55398ee282"), //Working in a confined place
            Guid.Parse("761d032d-09f1-47c1-ab43-2fcc80e90420"), //HIRA
            Guid.Parse("1bbb9cf1-32dd-4138-b187-30d97ec5ef71"), //Roadside Safety Procedure / Flagperson
            Guid.Parse("4b94807d-7fb1-4adc-bca8-35188d90c84c"), //Shielded Metal Arc Welding 
            Guid.Parse("03a5cfbb-0add-40c9-b556-36f5b557d929"), //Basic Kitchen Appliance Repairer
            Guid.Parse("84991c8a-0810-4987-8061-47f3fffefbdf"), //New Venture Creation
            Guid.Parse("63390c8f-7629-42b7-9121-756323596463"), //Conflict management 
            Guid.Parse("3d989f72-e9f1-443b-b11f-7c101e962a1f"), //Handling Dangerous Goods
            Guid.Parse("ca7e22c7-ea29-4659-871d-8b7ea77f27f4"), //Assistant Handyperson
            Guid.Parse("d03f9a76-0622-4fc8-8565-8eee20e77b67"), //Bricklayer Assistant 
            Guid.Parse("7e9b8d37-2503-41e7-83dc-904307d7147f"), //Working at Heights
            Guid.Parse("365b503a-af13-405f-a37a-a4863a35b1d1"), //Supervisory
            Guid.Parse("67392acd-b16f-4692-bc9f-ac3b9f5623d5"), //Legal Liability / OHS
            Guid.Parse("85a364a5-fa94-4a23-a449-b11a8cf5e139"), //Plumbing Hand
            Guid.Parse("b174bf83-274e-426e-87fa-bbb215731eb4"), //Coded Welding
            Guid.Parse("5d058ea8-c199-4ba7-aee6-bd0b1cca7fff"), //Fire Fighting
            Guid.Parse("3022b9cf-5671-4aea-889c-dfb01aa933b6"), //Basic Hand Tools
            Guid.Parse("BAFE3BFB-7690-4AF4-849A-DDF161D2A18C"),
            Guid.Parse("F16393B8-8670-4FC2-9F0E-468C4736A8BB"),

            #endregion

            #region Trade Test

            #endregion
        };
        private readonly IFileUploadService _fileUploadService;
        private readonly ICourseService _courseService;
        private readonly IInAppNotificationService _inAppNotificationService;
        private readonly IUserService _userService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsController"/> class.
        /// </summary>
        /// <param name="context">The unit of work context used for database operations.</param>
        /// <param name="hostEnvironment">The hosting environment information.</param>
        /// <param name="logger">The logger instance used for logging operations.</param>
        /// <param name="helperService">The helper service providing utility functions.</param>
        /// <param name="applicationsService">The service handling application dashboard operations.</param>
        /// <param name="blobFileService">The service for managing blob file operations.</param>
        public ApplicationsController(IUnitOfWork context, IWebHostEnvironment hostEnvironment, ILogger<ApplicationsController> logger, IHelperService helperService, IApplicationsService applicationsService, IBlobFileService blobFileService, IFileUploadService fileUploadService, ICourseService courseService, IInAppNotificationService inAppNotificationService, IUserService userService)
        {
            _logger = logger;
            _helperService = helperService;
            _applicationsService = applicationsService;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _blobFileService = blobFileService;
            _containerName = _helperService.GetConfigurationValue("AzureStorage:Containers:Applications", string.Empty);
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _inAppNotificationService = inAppNotificationService ?? throw new ArgumentNullException(nameof(inAppNotificationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Retrieves the dashboard data asynchronously and returns the corresponding view.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the dashboard view with data.</returns>
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = await _applicationsService.GetDashboardData();

            return View(dashboard);
        }

        /// <summary>
        /// Handles the retrieval and presentation of application data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="IActionResult"/> with the application data.</returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Applications()
        {
            try
            {
                var list = await _context.Applications.GetAllAsync(includeProperties: new[] { nameof(Application.ApplicantAddress), nameof(Application.ApplicantGuardian) });

                list = list.OrderByDescending(item => ConvertToDateTime(
                Helper.ExtractDateFromReference(item.ReferenceNumber ?? $"FOR{DateTime.Now.ToShortDateString()}{Helper.RandomStringGenerator(3)}"))).ToList();

                List<ApplicationsViewModel> applications = new();

                foreach (var item in list)
                {
                    var dto = await ConvertToApplicationsDTOAsync(item);

                    applications.Add(dto);
                }

                ViewData["New"] = await NewApplicationCountAsync();

                ViewData["Pending"] = await PendingApplicationCountAsync();

                return View(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing applications.");
                throw;
            }
        }

        /// <summary>
        /// Processes the application submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            var courses = await _context.Courses.GetAllAsync();

            courses = courses.Where(m => m.IsActive).ToList();

            var courseList = courses.Select(s => new SelectListItem
            {
                Value = s.CourseId.ToString(),

                Text = $"{s.CourseName} ({Helper.GetDisplayName(s.Type)}) {s.NType}"
            });

            ViewBag.CourseId = new SelectList(courseList, "Value", "Text");

            return View();
        }

        /// <summary>
        /// Processes the application submission.
        /// </summary>
        /// <param name="model">The application data transfer object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HandleInvalidModel(model);
            }

            Application application = null;

            try
            {
                if (await IsApplicationLimitReached(model))
                {
                    TempData["error"] = "Error: Application Limit reached for this academic year";
                    return RedirectToAction("Logout", "Auth");
                }

                application = CreateApplication(model);

                if (await IsDuplicateApplication(application))
                {
                    return HandleDuplicateApplication(model);
                }

                await UploadFilesAsync(application, HttpContext.RequestAborted);

                var saveResponse = await SaveApplicationAsync(application);

                if (saveResponse != 3)
                {
                    await CleanupUploadedFilesAsync(application, HttpContext.RequestAborted);
                    return HandleApplicationSaveFailure(model);
                }

                await SendEmailNotificationAsync(application);

                await _inAppNotificationService.SendToRoleAsync(eSysRole.Admin, $"{application.ApplicantName} {application.ApplicantSurname} made an submitted an online application (Ref: {application.ReferenceNumber})",
                    actionUrl: Url.Action("OnApply", "Applications", new { ApplicationId = application.ApplicationId}),
                    iconCss: "fa fa-file-alt",
                    createdBy: $"{application.ApplicantName} {application.ApplicantSurname}");

                return HandleApplicationSuccess(application);
            }
            catch (Exception ex)
            {
                if (application is not null)
                {
                    await CleanupUploadedFilesAsync(application, HttpContext.RequestAborted);
                }

                return HandleUnexpectedError(ex, model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Start()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Identity()
        {
            return View();
        }

        /// <summary>
        /// Returns the Calendar view for the current request.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Calendar view.</returns>
        [HttpGet]
        public IActionResult Calendar()
        {
            return View();
        }

        /// <summary>
        /// Returns the view that displays a list of calendar events.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the calendar events list view.</returns>
        [HttpGet]
        public IActionResult ListCalendarEvents()
        {
            return View();
        }

        /// <summary>
        /// Retrieves calendar events occurring within the specified date range and returns them as a JSON-formatted
        /// collection suitable for client-side calendar display.
        /// </summary>
        /// <remarks>The returned events are formatted for compatibility with typical calendar UI
        /// components. All date and time values are provided in ISO 8601 format. If no events are found within the
        /// specified range, the collection will be empty.</remarks>
        /// <param name="start">The start date and time of the range to filter events. If null, events are not filtered by a lower bound.</param>
        /// <param name="end">The end date and time of the range to filter events. If null, events are not filtered by an upper bound.</param>
        /// <returns>A JSON result containing a collection of calendar events, each with properties such as ID, title, start and
        /// end times, all-day status, color information, and extended properties.</returns>
        public async Task<IActionResult> CalendarEvents(DateTime? start, DateTime? end)
        {
            var events = await _applicationsService.GetCalendarEventsAsync(start, end);
            return Json(events.Select(e => new { id = e.EventId, title = e.Title, start = e.StartUtc.ToString("o"), end = e.EndUtc?.ToString("o"), allDay = e.AllDay, backgroundColor = e.ColorHex, borderColor = e.ColorHex, extendedProps = new { category = e.Category, description = e.Description } }));
        }

        /// <summary>
        /// Creates a new calendar event using the data provided in the form.
        /// </summary>
        /// <remarks>This method processes a POST request to create a calendar event. The event is
        /// associated with the current user. If the operation is cancelled via the provided token, the event will not
        /// be created.</remarks>
        /// <param name="form">The form data containing details of the calendar event to create. Must not be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the event creation view.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] CalendarEventFormViewModel form, CancellationToken ct)
        {
            string currentUser = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}" ?? "System";  

            await _applicationsService.CreateEventAsync(form, currentUser, ct);

            return View();
        }

        /// <summary>
        /// Updates an existing calendar event using the data provided in the form.
        /// </summary>
        /// <remarks>This action requires the user to be authenticated. If the user is not authenticated,
        /// the update will be attributed to "System". The method expects valid event data; invalid or incomplete data
        /// may result in validation errors.</remarks>
        /// <param name="form">The form data containing the updated event details. Must include all required fields for the calendar event.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the view after the event has been updated.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateEvent([FromForm] CalendarEventFormViewModel form)
        {
            await _applicationsService.UpdateEventAsync(form, User?.Identity?.Name ?? "System");
            return View();
        }

        /// <summary>
        /// Deletes the event with the specified identifier and redirects to the calendar view.
        /// </summary>
        /// <param name="id">The unique identifier of the event to delete.</param>
        /// <param name="token">A cancellation token that can be used to cancel the delete operation.</param>
        /// <returns>An IActionResult that redirects the user to the calendar view after the event is deleted.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteEvent(Guid id, CancellationToken token)
        {
            await _applicationsService.DeleteEventAsync(id, token);
            return RedirectToAction("Calendar");
        }

        /// <summary>
        /// Handles the application process for a specific application ID.
        /// </summary>
        /// <param name="ApplicationId">The ID of the application to process.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> OnApply(Guid ApplicationId)
        {
            try
            {
                var application = await _context.Applications.GetAsync(filter: a => a.ApplicationId == ApplicationId, includeProperties: new[] { nameof(Application.ApplicantAddress), nameof(Application.ApplicantGuardian) });

                if (application is null)
                {
                    _logger.LogWarning("Application with ID {ApplicationId} not found.", ApplicationId);

                    return RedirectToAction("RouteNotFound", "Global");
                }

                var applyDto = ConvertToApplyDTO(application);

                ViewData["Course"] = await ConvertCourseIdToStringAsync(application.CourseId);

                ViewBag.Application = application;

                return View(applyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the application with ID {ApplicationId}.", ApplicationId);

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates an existing application with the provided application data transfer object.
        /// </summary>
        /// <param name="model">The application data transfer object containing the updated application details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnApply(ApplyViewModel model)
        {
            var application = await _context.Applications.GetAsync(filter: a => a.ApplicationId == model.ApplicantId, includeProperties: new[] { nameof(Application.ApplicantGuardian), nameof(Application.ApplicantAddress) });

            if (application == null)
            {
                TempData["error"] = "Application not found.";

                return View(model);
            }

            var guardian = await LoadGuardianAsync(application.ApplicationId);

            if (guardian == null)
            {
                TempData["error"] = "Guardian not found.";

                return View(model);
            }

            if (model.Status == ApplicationStatus.Pending)
            {
                if (string.IsNullOrWhiteSpace(model.PendingStatusReason))
                {
                    TempData["error"] = "Pending reason is required when status is Pending.";

                    return RedirectToAction(nameof(OnApply), new { ApplicationId = model.ApplicantId });
                }

                application.StatusReason = model.PendingStatusReason.Trim();
                application.PendingStatusMessage = string.IsNullOrWhiteSpace(model.PendingStatusMessage)
                    ? null
                    : model.PendingStatusMessage.Trim();

                await SendPendinMailAsync(application, model.PendingStatusReason);
            }
            else
            {
                application.StatusReason = null;
                application.PendingStatusMessage = null;
            }

            // UpdateModelWithGuardianDetails(model, guardian);

            if (model.Status == ApplicationStatus.Approved)
            {
                application.Status = ApplicationStatus.Approved;
                var results = await ProcessApprovedApplicationAsync(application);
            }

            if (model.Status == ApplicationStatus.AptituteTest)
            {
                var dateTime = _helperService.GetNextBusinessDayWithTime();

                await SendAptitudeInvitationAsync(application, dateTime.ToString("yy/M/d hh:mm tt"));
            }

            // var updatedApplication = CreateUpdatedApplication(application, model);

            if (model.Status == ApplicationStatus.Rejected)
            {
                if (!await IsRejectionFormSubmittedAsync(model.ApplicantId))
                {
                    TempData["error"] = "Rejection form not submitted. Please provide reasons for rejection.";

                    return View(model);
                }
            }

            application.Status = model.Status;

            var modifyApp = await _context.Applications.UpdateApplicationAsync(application);

            if (modifyApp != null)
            {
                TempData["success"] = "Application saved successfully.";

                return RedirectToAction(nameof(Applications));
            }

            TempData["error"] = "Error: Unable to save application.";

            return View(model);
        }

        /// <summary>
        /// Handles requests for the default page of the application.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>
        /// that renders the default view.</returns>
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Rejects an application based on the provided rejection data transfer object.
        /// </summary>
        /// <param name="model">The rejection data transfer object containing the details of the rejection.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnRejectApplication(RejectionViewModel model)
        {
            var doesApplicationExist = await _context.Applications.ExistsAsync(a => a.ApplicationId != model.ApplicationId);

            if (!doesApplicationExist)
            {
                return NotFound();
            }

            if (await _context.Rejections.ExistsAsync(a => a.Id == model.Id))
            {
                TempData["error"] = "Application Already Rejected!!";

                return RedirectToAction(nameof(OnApply), new { ApplicationId = model.ApplicationId });
            }
            var application = await _context.Applications.GetAsync(filter: a => a.ApplicationId == model.ApplicationId);

            if (application == null)
            {
                return NotFound();
            }

            var applicationRejection = CreateApplicationRejection(model);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Validation error.";

                return RedirectToAction(nameof(OnApply), new { ApplicationId = model.ApplicationId });
            }

            var response = await SaveApplicationRejectionAsync(applicationRejection);

            TempData["success"] = response > 0 ? "Application successfully rejected." : "Error: Application rejection failed.";

            await SendRejectionMailAsync(application, applicationRejection.Reason);

            return RedirectToAction("Applications", "Applications");
        }

        /// <summary>
        /// Downloads a file from the specified destination folder.
        /// </summary>
        /// <param name="fileName">The name of the file to download.</param>
        /// <param name="destinationFolder">The folder where the file is located.</param>
        /// <returns>A file result for downloading the file.</returns>
        public IActionResult DownloadFile(string fileName, string destinationFolder)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string folderPath = Path.Combine(wwwRootPath, destinationFolder);

                string filePath = Path.Combine(folderPath, fileName);

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return RedirectToAction(nameof(Applications));
            }
        }

        /// <summary>
        /// Downloads a file from the specified destination folder.
        /// </summary>
        /// <param name="fileName">The name of the file to download.</param>
        /// <param name="destinationFolder">The folder where the file is located.</param>
        /// <returns>A file result for downloading the file or an appropriate error message if the file cannot be found or accessed.</returns>
        public IActionResult DownloadFileAsync(string fileName, string destinationFolder)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(destinationFolder))
                {
                    TempData["error"] = "Invalid file name or destination folder.";

                    return RedirectToAction(nameof(Applications));
                }

                string wwwRootPath = _hostEnvironment.WebRootPath;

                string filePath = Path.Combine(wwwRootPath, destinationFolder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    TempData["error"] = "File not found.";
                    return RedirectToAction(nameof(Applications));
                }

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                string contentType = GetContentType(filePath) ?? "application/octet-stream";

                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while attempting to download the file: {ex.Message}";
                return RedirectToAction(nameof(Applications));
            }
        }

        /// <summary>
        /// Builds a notification message for the application.
        /// </summary>
        /// <param name="model">The application model.</param>
        /// <param name="programme">The name of the program applied for.</param>
        /// <returns>The notification message.</returns>
        public string MessageBuilder(Application model, string programme)
        {
            string message = $"Dear {model.ApplicantName} {model.ApplicantSurname},\n\n" +

           $"This email is to confirm that we have received your application for the {programme} program.\n\n\n" +
           "Your application is currently being reviewed by our admissions committee.\n\n" +
           "\nContact Information: If you have any questions or need to provide additional information, please do not hesitate to contact our admissions office. You can reach us at 060 728 6757 or via email at info@forek.co.za\n\n" +
           "\nWe appreciate your patience and understanding throughout the admissions process. We will make every effort to keep you informed about your application status and provide a timely response.\n\n" +
           "\nThank you once again for choosing Forek Institute of Technology for your educational aspirations. We wish you the best of luck and look forward to reviewing your application.\n\n" +
           "\nKind regards,\n\n" +
           "\nT Manyimo\n" +
           "\nCampus Manager\n" +
           "\nForek Institute of Technology";

            return message;
        }

        /// <summary>
        /// Downloads an attachment file asynchronously.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <returns>A file result for downloading the attachment.</returns>
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"AppIDPass", filename);

                var memory = new MemoryStream();

                using (var stream = new FileStream(folder, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return View();
            }
        }

        /// <summary>
        /// Uploads the qualification document for an application asynchronously.
        /// </summary>
        /// <param name="attachment">The application containing the qualification document.</param>
        public async Task QualificationUploader(Application attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.HighestQualFile.FileName);

                string extension = Path.GetExtension(attachment.HighestQualFile.FileName);

                attachment.HighestQualDoc = fileName = fileName + extension;

                string path = Path.Combine(wwwRootPath + "/HighestQualification/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await attachment.HighestQualFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }

        /// <summary>
        /// Uploads the identification document for an application asynchronously.
        /// </summary>
        /// <param name="attachment">The application containing the identification document.</param>
        public async Task AttachmentUploader(Application attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.IDPassFile.FileName);

                string extension = Path.GetExtension(attachment.IDPassFile.FileName);

                attachment.IDPassDoc = fileName = fileName + extension;

                string path = Path.Combine(wwwRootPath + "/AppIDPass/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await attachment.IDPassFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }

        /// <summary>
        /// Downloads a qualification document file asynchronously.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <returns>A file result for downloading the qualification document.</returns>
        public async Task<IActionResult> QualificationDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"HighestQualification", filename);

                var memory = new MemoryStream();

                using (var stream = new FileStream(folder, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return View();
            }
        }

        /// <summary>
        /// Uploads the residence document for an application asynchronously.
        /// </summary>
        /// <param name="attachment">The application containing the residence document.</param>
        public async Task ResidenceUploader(Application attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.ResidenceFile.FileName);

                string extension = Path.GetExtension(attachment.ResidenceFile.FileName);

                attachment.ResidenceDoc = fileName = fileName + extension;

                string path = Path.Combine(wwwRootPath + "/Residence/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await attachment.ResidenceFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }

        /// <summary>
        /// Downloads a residence document file asynchronously.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <returns>A file result for downloading the residence document.</returns>
        public async Task<IActionResult> ResidenceDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"Residence", filename);

                var memory = new MemoryStream();

                using (var stream = new FileStream(folder, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return View();
            }
        }

        /// <summary>
        /// Downloads a file from the specified folder.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <param name="folder">The folder where the file is located.</param>
        /// <returns>A file result for downloading the file.</returns>
        public IActionResult FileDownload(string filename, string folder)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename))
                {
                    TempData["error"] = "File name cannot be null or empty.";
                    return Content("Sorry, no attachment found!!!");
                }

                const string root = "wwwroot";

                var directoryRoot = Directory.GetCurrentDirectory() + "\\" + root;

                string filePath = directoryRoot + filename;

                if (!System.IO.File.Exists(filePath))
                {
                    TempData["error"] = "The specified file does not exist.";

                    return Content("Sorry, the file does not exist!!!");
                }

                var contentType = GetContentType(filePath);

                var fileBytes = System.IO.File.ReadAllBytes(filePath);

                return File(fileBytes, contentType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";

                return RedirectToAction(nameof(Applications));
            }
        }

        /// <summary>
        /// Notifies the applicant via SMS or email.
        /// </summary>
        /// <param name="UserEmail">The email of the user.</param>
        /// <param name="UserPhone">The phone number of the user.</param>
        /// <param name="Message">The message to be sent.</param>
        /// <param name="IsSMS">Whether to send an SMS.</param>
        /// <param name="IsEmail">Whether to send an email.</param>
        [HttpPost]
        public IActionResult OnNotifyApplicant(string UserEmail, string UserPhone, string Message, bool IsSMS, bool IsEmail)
        {
            try
            {
                Helper.SendSMS(Message, UserPhone);
                return Json(new { success = true, message = "Message sent successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportApplicationsCsv(DateTime? from, DateTime? to, string? q)
        {
            var list2 = /* get from data source */ Enumerable.Empty<ApplicationsViewModel>().ToList();
            var list = await _context.Applications.GetAllAsync();

            //var referenceDate = DateTime.ParseExact(Helper.ExtractDateFromReference(a.ReferenceNumber),format,CultureInfo.InvariantCulture);

            //if (from.HasValue)
            //    list = list.Where(a => a.date.Date >= from.Value.Date).ToList();
            //if (to.HasValue)
            //    list = list.Where(a => a.SubmittedDate.Date <= to.Value.Date).ToList();
            //if (!string.IsNullOrWhiteSpace(q))
            //{
            //    var term = q.Trim().ToLowerInvariant();
            //    list = list.Where(a =>
            //           (a.Reference ?? "").ToLower().Contains(term)
            //        || (a.Names ?? "").ToLower().Contains(term)
            //        || (a.IDNumber ?? "").ToLower().Contains(term)
            //        || (a.Cellphone ?? "").ToLower().Contains(term)
            //        || (a.Course ?? "").ToLower().Contains(term)
            //    ).ToList();
            //}

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Reference,Names,ID,Phone,Course,SubmittedDate,Status");
            foreach (var a in list)
            {
                string line = string.Join(",",
                    Csv(a.ReferenceNumber),
                    Csv(a.ApplicantName),
                    Csv(a.IDNumber),
                    Csv(a.Cellphone)
                //Csv(a.CourseId),
                //Csv(a.SubmittedDate.ToString("yyyy-MM-dd")),
                //Csv(a.Status)
                );
                sb.AppendLine(line);
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"applications-{DateTime.UtcNow:yyyyMMdd}.csv";
            return File(bytes, "text/csv", fileName);

            static string Csv(string? v) =>
                "\"" + (v ?? string.Empty).Replace("\"", "\"\"") + "\"";
        }

        /// <summary>
        /// Returns a file download response for the applicant's attachment identified by the specified file ID.
        /// </summary>
        /// <param name="fileId">The unique identifier of the attachment file to download. Cannot be null, empty, or consist only of
        /// white-space characters.</param>
        /// <returns>An <see cref="IActionResult"/> that, on success, contains the file stream for the requested attachment;
        /// otherwise, a content result indicating that no attachment was found.</returns>
        public async Task<IActionResult> ApplicantAttachmentDownload(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return Content("Sorry, no attachment found!!!");
            }

            var attachment = await _fileUploadService.DownloadAsync(fileId, HttpContext.RequestAborted);

            return File(attachment.FileStream, attachment.ContentType ?? "application/octet-stream", attachment.FileName);
        }

        #region Finance

        /// <summary>
        /// Returns the financial clearance status for an approved application.
        /// Creates a default AwaitingPayment record if none exists yet.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> FinancialGate(Guid applicationId)
        {
            if (applicationId == Guid.Empty)
                return BadRequest();

            var application = await _context.Applications.GetAsync(filter: a => a.ApplicationId == applicationId);

            if (application is null || application.Status != ApplicationStatus.Approved)
                return NotFound("Application not found or not approved.");

            var clearance = await _context.FinancialClearances.GetAsync(filter: fc => fc.ApplicationId == applicationId);

            if (clearance is null)
            {
                clearance = new FinancialClearance
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = applicationId,
                    Status = eFinancialClearanceStatus.AwaitingPayment,
                    Name = $"{application.ApplicantName} {application.ApplicantSurname}",
                    Code = application.ReferenceNumber                   
                };

                await _context.FinancialClearances.AddAsync(clearance);
                await _context.SaveAsync();
            }

            return Json(new
            {
                clearanceId = clearance.Id,
                applicationId = clearance.ApplicationId,
                status = clearance.Status.ToString(),
                amountExpected = clearance.AmountExpected,
                amountReceived = clearance.AmountReceived,
                proofFileName = clearance.ProofOfPaymentFileName,
                proofFileId = clearance.ProofOfPaymentFileId,
                overrideReason = clearance.OverrideReason,
                userCreated = clearance.UserCreated,
                clearedUtc = clearance.DateCreated,
                notes = clearance.Notes
            });
        }

        /// <summary>
        /// Uploads a proof-of-payment document for an approved application.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UploadProofOfPayment(Guid applicationId, IFormFile proofFile)
        {
            if (applicationId == Guid.Empty || proofFile is null || proofFile.Length == 0)
                return BadRequest("Application ID and proof file are required.");

            var clearance = await _context.FinancialClearances.GetAsync(filter: fc => fc.ApplicationId == applicationId);

            if (clearance is null)
                return NotFound("No financial clearance record found.");

            await using var stream = proofFile.OpenReadStream();

            var uploadResponse = await _fileUploadService.UploadAsync(
                new UploadFileRequest(
                    FileStream: stream,
                    FileName: proofFile.FileName,
                    ContentType: proofFile.ContentType,
                    Metadata: new Dictionary<string, string>
                    {
                        ["Entity"] = "FinancialClearance",
                        ["ApplicationId"] = applicationId.ToString("D"),
                        ["FileType"] = "ProofOfPayment"
                    },
                    ProviderHint: null,
                    ExpiryDate: null,
                    TenantId: null,
                    DocumentType: "FinancialClearance.ProofOfPayment"),
                HttpContext.RequestAborted);

            var currentUser = OnGetCurrentUser();

            clearance.ProofOfPaymentFileId = uploadResponse.FileId;
            clearance.ProofOfPaymentFileName = proofFile.FileName;
            clearance.DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset();
            clearance.UserCreated = $"{currentUser?.Name} {currentUser?.LastName}";
            clearance.Status = eFinancialClearanceStatus.ProofUploaded;

            await _context.FinancialClearances.Update(clearance);
            await _context.SaveAsync();

            return Json(new { success = true, message = "Proof of payment uploaded successfully.", fileName = proofFile.FileName });
        }

        /// <summary>
        /// Finance officer clears the applicant after reviewing proof-of-payment.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ClearApplicant(Guid applicationId, decimal? amountReceived, string? notes)
        {
            var clearance = await _context.FinancialClearances.GetAsync(
                filter: fc => fc.ApplicationId == applicationId);

            if (clearance is null)
                return NotFound("No financial clearance record found.");

            if (clearance.Status == eFinancialClearanceStatus.Cleared || clearance.Status == eFinancialClearanceStatus.Overridden)
                return Json(new { success = true, message = "Already cleared." });

            var currentUser = OnGetCurrentUser();

            clearance.Status = eFinancialClearanceStatus.Cleared;
            clearance.AmountReceived = amountReceived;
            clearance.Notes = notes?.Trim();
            clearance.UserCreated = $"{currentUser?.Name} {currentUser?.LastName}";
            clearance.DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset();

            await _context.FinancialClearances.Update(clearance);
            await _context.SaveAsync();

            return Json(new { success = true, message = "Applicant financially cleared." });
        }

        /// <summary>
        /// Overrides the financial gate (funded student, payment arrangement, etc.).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> OverrideClearance(Guid applicationId, string overrideReason, string? notes)
        {
            if (string.IsNullOrWhiteSpace(overrideReason))
                return BadRequest("Override reason is required.");

            var clearance = await _context.FinancialClearances.GetAsync(filter: fc => fc.ApplicationId == applicationId);

            if (clearance is null)
                return NotFound("No financial clearance record found.");

            var currentUser = OnGetCurrentUser();

            clearance.Status = eFinancialClearanceStatus.Overridden;
            clearance.OverrideReason = overrideReason.Trim();
            clearance.Notes = notes?.Trim();
            clearance.UserCreated = $"{currentUser?.Name} {currentUser?.LastName}";
            clearance.DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset();

            await _context.FinancialClearances.Update(clearance);
            await _context.SaveAsync();

            return Json(new { success = true, message = "Financial gate overridden. Applicant may proceed to registration." });
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Checks if a rejection form has been submitted for the specified applicantID asynchronously.
        /// </summary>
        /// <param name="applicantId">The applicant ID.</param>
        /// <returns>True if a rejection form exists; otherwise, false.</returns>
        private async Task<bool> IsRejectionFormSubmittedAsync(Guid applicantId)
        {
            return await Task.Run(() => _context.Rejections.ExistsAsync(r => r.ApplicationId == applicantId));
        }

        /// <summary>
        /// Creates an updated application entity from the existing application and the application data transfer object.
        /// </summary>
        /// <param name="application">The existing application entity.</param>
        /// <param name="model">The application data transfer object.</param>
        /// <returns>The updated application entity.</returns>
        private Application CreateUpdatedApplication(Application application, ApplyViewModel model)
        {
            return new Application
            {
                ApplicationId = application.ApplicationId,

                ReferenceNumber = application.ReferenceNumber ?? model.ReferenceNumber,

                CourseId = application.CourseId,

                HighestQualDoc = application.HighestQualDoc ?? model.HighestQualDoc,

                HighestQualification = application.HighestQualification,

                ApplicantTitle = application.ApplicantTitle,

                ApplicantName = application.ApplicantName,

                ApplicantSurname = application.ApplicantSurname,

                Cellphone = application.Cellphone,

                Email = application.Email,

                Gender = application.Gender,

                IDNumber = application.IDNumber ?? "0000000000000",

                PassportNumber = application.PassportNumber,

                Selection = application.Selection,

                Status = model.Status,

                StudyPermitCategory = application.StudyPermitCategory,

                IDPassDoc = application.IDPassDoc ?? model.IDPassDoc,

                ResidenceDoc = application.ResidenceDoc ?? model.ResidenceDoc,

            };
        }

        /// <summary>
        /// Updates the application data transfer object with the guardian details.
        /// </summary>
        /// <param name="model">The application data transfer object.</param>
        /// <param name="guardian">The guardian details.</param>
        private void UpdateModelWithGuardianDetails(ApplyViewModel model, Guardian guardian)
        {
            model.GuardianFirstName = guardian.FirstName;

            model.GuardianLastName = guardian.LastName;

            model.GuardianCellphone = guardian.Cellphone;

            model.GuardianRelationship = guardian.Relationship;
        }

        /// <summary>
        /// Loads the guardian details associated with the specified applicationID asynchronously.
        /// </summary>
        /// <param name="applicationId">The application ID.</param>
        /// <returns>The guardian details.</returns>
        private async Task<Guardian> LoadGuardianAsync(Guid applicationId)
        {
            var guardians = await _context.Guardian.GetAllAsync();

            return guardians.FirstOrDefault(m => m.ApplicationId == applicationId);
        }

        /// <summary>
        /// Saves the provided ApplicationRejection instance to the database asynchronously.
        /// </summary>
        /// <param name="applicationRejection">The application rejection instance to save.</param>
        /// <returns>The number of state entries written to the database.</returns>
        private async Task<int> SaveApplicationRejectionAsync(ApplicationRejection applicationRejection)
        {
            if (applicationRejection == null)
                throw new ArgumentNullException(nameof(applicationRejection), "The application rejection DTO cannot be null.");

            if (applicationRejection.ApplicationId == Guid.Empty)
                throw new ArgumentException("Invalid Application ID.", nameof(applicationRejection.ApplicationId));

            if (string.IsNullOrWhiteSpace(applicationRejection.Reason))
                throw new ArgumentException("Rejection reason cannot be empty.", nameof(applicationRejection.Reason));

            try
            {
                _logger.LogInformation("Starting the application rejection process for Application ID: {ApplicationId}", applicationRejection.ApplicationId);

                var rejApplication = new ApplicationRejection
                {
                    ApplicationId = applicationRejection.ApplicationId,
                    Reason = applicationRejection.Reason,
                    NextSteps = applicationRejection.NextSteps,
                    CreatedBy = applicationRejection.CreatedBy,
                    CreatedOn = applicationRejection.CreatedOn,
                    FollowUpDate = applicationRejection.FollowUpDate,
                    Id = applicationRejection.Id,
                    IsActive = applicationRejection.IsActive,
                    IsFinal = applicationRejection.IsFinal,
                    AdditionalComments = applicationRejection.AdditionalComments,
                    ModifiedBy = applicationRejection.ModifiedBy,
                    ModifiedOn = applicationRejection.ModifiedOn,
                };

                await _context.Rejections.AddAsync(applicationRejection);

                int rejectionSaveResult = await _context.SaveAsync();

                if (rejectionSaveResult <= 0)
                {
                    _logger.LogWarning("Failed to save application rejection for Application ID: {ApplicationId}", applicationRejection.ApplicationId);

                    return 0;
                }

                var applicationLookUp = await _context.Applications.GetAsync(filter: a => a.ApplicationId == applicationRejection.ApplicationId);

                if (applicationLookUp == null)
                {
                    _logger.LogError("Application with ID {ApplicationId} not found.", applicationRejection.ApplicationId);

                    throw new InvalidOperationException($"Application with ID {applicationRejection.ApplicationId} does not exist.");
                }

                applicationLookUp.Status = ApplicationStatus.Rejected;

                await _context.Applications.UpdateApplicationAsync(applicationLookUp);

                int applicationUpdateResult = await _context.SaveAsync();

                if (applicationUpdateResult != 0)
                {
                    _logger.LogWarning("Failed to update application status for Application ID: {ApplicationId}", applicationRejection.ApplicationId);

                    return 0;
                }

                _logger.LogInformation("Successfully rejected Application ID: {ApplicationId}", applicationRejection.ApplicationId);

                return rejectionSaveResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while rejecting Application ID: {ApplicationId}", applicationRejection?.ApplicationId);

                throw;
            }

        }

        /// <summary>
        /// Creates an ApplicationRejection instance from the provided RejectionViewModel model.
        /// </summary>
        /// <param name="model">The rejection data transfer object.</param>
        /// <returns>The created ApplicationRejection instance.</returns>
        private ApplicationRejection CreateApplicationRejection(RejectionViewModel model)
        {
            return new ApplicationRejection
            {
                FollowUpDate = model.FollowUpDate,
                AdditionalComments = model.AdditionalComments,
                NextSteps = model.NextSteps,
                ApplicationId = model.ApplicationId,
                CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",
                CreatedOn = _helperService.GetCurrentTime().ToString(),
                Id = _helperService.GenerateGuid(),
                IsFinal = model.IsFinal,
                IsActive = true,
                Reason = model.Reason
            };
        }

        /// <summary>
        /// Creates an <see cref="Application"/> entity from the given <see cref="ApplyDTO"/> model.
        /// </summary>
        /// <param name="model">The model containing the application data.</param>
        /// <returns>The created <see cref="Application"/> entity.</returns>
        private Application CreateApplication(ApplyViewModel model)
        {
            try
            {
                Guid key = Helper.GenerateGuid();

                string getID = model.ApplicantIDPassFile.FileName;

                string getQualification = model.AplicantHighestQualFile.FileName;

                string getResidence = model.ApplicantResidenceFile.FileName;

                Application application = new()
                {
                    ApplicationId = key,

                    Email = model.Email,

                    Cellphone = model.Cellphone,

                    ResidenceDoc = getResidence,

                    HighestQualDoc = getQualification,

                    IDNumber = model.IDNumber,

                    IDPassDoc = getID,

                    ApplicantName = model.ApplicantName,

                    ApplicantSurname = model.ApplicantSurname,

                    Selection = model.Selection,

                    Status = ApplicationStatus.Submitted,

                    StudyPermitCategory = model.StudyPermitCategory ?? eCategory.NA,

                    ApplicantTitle = model.ApplicantTitle,

                    ReferenceNumber = $"FOR{DateTime.Now.ToShortDateString()}{Helper.RandomStringGenerator(3)}",
                    //FOR6/20/2023iYs
                    CourseId = model.CourseId,

                    Gender = model.Gender,

                    PassportNumber = model.PassportNumber,

                    HighestQualification = model.HighestQualification,

                    HighestQualFile = model.AplicantHighestQualFile,

                    IDPassFile = model.ApplicantIDPassFile,
                    ResidenceFile = model.ApplicantResidenceFile,

                    ApplicantAddress = CreateApplicantAddress(model, key),

                    ApplicantGuardian = CreateApplicantGuardian(model, key),

                };

                return application;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the application: {ex.Message}");

                throw;

            }
        }

        /// <summary>
        /// Determines the content type based on the file extension.
        /// </summary>
        /// <param name="filePath">The file path to analyze.</param>
        /// <returns>The MIME type as a string, or null if unknown.</returns>
        private string GetContentType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();

            return provider.TryGetContentType(filePath, out var contentType)
                ? contentType
                : null;
        }

        /// <summary>
        /// Creates an <see cref="Address"/> entity from the given <see cref="ApplyDTO"/> model and application ID.
        /// </summary>
        /// <param name="model">The model containing the address data.</param>
        /// <param name="applicationId">The ID of the application.</param>
        /// <returns>The created <see cref="Address"/> entity.</returns>
        private Address CreateApplicantAddress(ApplyViewModel model, Guid applicationId)
        {
            return new Address
            {
                AddressId = Helper.GenerateGuid(),

                StreetName = model.StreetName,

                AssociativeId = applicationId,

                City = model.City,

                Line1 = model.Line1,

                PostalCode = model.PostalCode,

                Province = model.Province
            };
        }

        /// <summary>
        /// Creates a <see cref="Guardian"/> entity from the given <see cref="ApplyDTO"/> model and application ID.
        /// </summary>
        /// <param name="model">The model containing the guardian data.</param>
        /// <param name="applicationId">The ID of the application.</param>
        /// <returns>The created <see cref="Guardian"/> entity.</returns>
        private Guardian CreateApplicantGuardian(ApplyViewModel model, Guid applicationId)
        {
            return new Guardian
            {
                IDDoc = model.IDPassDoc,

                IDFile = model.GuardianIDFile,

                ApplicationId = applicationId,

                Cellphone = model.GuardianCellphone,

                GuardianId = Helper.GenerateGuid(),

                FirstName = model.GuardianFirstName,

                LastName = model.GuardianLastName,

                Relationship = model.GuardianRelationship,
            };
        }

        /// <summary>
        /// Asynchronously saves the given <see cref="Application"/> entity to the database.
        /// </summary>
        /// <param name="application">The application entity to save.</param>
        /// <returns>A task representing the asynchronous operation, containing the number of state entries written to the database.</returns>
        private async Task<int> SaveApplicationAsync(Application application)
        {
            var response = await _context.Applications.AddAsync(application);

            if (response != null)
            {
                return await _context.SaveAsync();
            }
            return 0;
        }

        /// <summary>
        /// Asynchronously uploads a file to the specified destination folder and updates the corresponding property of the application entity.
        /// </summary>
        /// <param name="application">The application entity to update.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="destinationFolder">The folder to upload the file to.</param>
        /// <param name="propertyName">The name of the property to update with the file path.</param>
        private async Task FileUploader(Application application, IFormFile file, string destinationFolder, string propertyName)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string folderPath = Path.Combine(wwwRootPath, destinationFolder);

                string absolutePath = wwwRootPath + folderPath;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(file.FileName);

                string extension = Path.GetExtension(file.FileName);

                PropertyInfo property = typeof(Application).GetProperty(propertyName + "Doc");

                if (property != null)
                {
                    property.SetValue(application, fileName + Helper.GenerateGuid() + extension);
                }

                string path = wwwRootPath + Path.Combine(folderPath, fileName + extension);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
            }
        }

        /// <summary>
        /// Asynchronously uploads a file if it is not null.
        /// </summary>
        /// <param name="application">The application entity to update.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="directory">The directory to upload the file to.</param>
        /// <param name="fileName">The name of the property to update with the file path.</param>
        private async Task UploadFileIfNotNullAsync(Application application, IFormFile file, string directory, string fileName)
        {
            if (file != null)
            {
                await FileUploader(application, file, directory, fileName);
            }
        }

        /// <summary>
        /// Sends an SMS notification about a new application.
        /// </summary>
        /// <param name="model">The application model containing the data.</param>
        private void OnSendSMS(Application model)
        {

            Helper.SendSMS("Dear Sir - a new application needing your attention" +
                            $" was just recieved - ref: {model.ReferenceNumber}", "0661401781");
        }

        /// <summary>
        /// Asynchronously sends an email notification about a new application.
        /// </summary>
        /// <param name="model">The application model containing the data.</param>
        private async Task SendEmailNotificationAsync(Application model)
        {
            try
            {
                string programme = await ConvertCourseIdToStringAsync(model.CourseId);

                var userEmailData = CreateUserEmailData(model, programme);

                var adminEmailData = CreateAdminEmailData(model, programme);

                var sendEmailTasks = new[]
                {
                    _helperService.SendMailNotificationAsync(userEmailData),
                    _helperService.SendMailNotificationAsync(adminEmailData)
                };

                await Task.WhenAll(sendEmailTasks);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notifications for application {ReferenceNumber}", model.ReferenceNumber);

                throw;
            }

        }

        /// <summary>
        /// Sends a rejection email to the applicant with the specified reason.
        /// </summary>
        /// <param name="application">The application details.</param>
        /// <param name="reason">The reason for rejection.</param>
        private async Task SendRejectionMailAsync(Application application, string reason)
        {
            ValidateApplication(application);

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason cannot be empty.", nameof(reason));

            string programme = await ConvertCourseIdToStringAsync(application.CourseId);

            var emailData = CreateEmailData(application, programme, EmailType.Rejection, reason);

            await SendEmailAsync(emailData);
        }


        private async Task SendPendinMailAsync(Application application, string reason)
        {
            ValidateApplication(application);

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Pending reason cannot be empty.", nameof(reason));

            string programme = await ConvertCourseIdToStringAsync(application.CourseId);

            var emailData = CreateEmailData(application, programme, EmailType.Pending, reason);

            await SendEmailAsync(emailData);
        }


        /// <summary>
        /// Sends an approval email to trades for the given application.
        /// </summary>
        /// <param name="application">The application details.</param>
        private async Task SendApprovalMailToTradesAsync(Application application)
        {
            ValidateApplication(application);

            string programme = await ConvertCourseIdToStringAsync(application.CourseId);

            var emailData = CreateEmailData(application, programme, EmailType.TradesAndNonTrades);

            await SendEmailAsync(emailData);

        }
        private async Task SendApprovalMailToShortSkillsTradeTestAsync(Application application)
        {
            ValidateApplication(application);

            string programme = await ConvertCourseIdToStringAsync(application.CourseId);

            var emailData = CreateEmailData(application, programme, EmailType.ShortSkillsTradeTest);

            await SendEmailAsync(emailData);
        }

        private async Task SendGenericApprovalMailAsync(Application application)
        {
            ValidateApplication(application);

            string programme = await ConvertCourseIdToStringAsync(application.CourseId);

            var emailData = CreateEmailData(application, programme, EmailType.GenericApproval);

            await SendEmailAsync(emailData);
        }

        /// <summary>
        /// Sends an aptitude test invitation email to the applicant.
        /// </summary>
        /// <param name="application">The application details.</param>
        /// <param name="dateTime">The scheduled date and time of the aptitude test.</param>
        private async Task SendAptitudeInvitationAsync(Application application, string dateTime)
        {
            ValidateApplication(application);

            if (string.IsNullOrWhiteSpace(dateTime))
                throw new ArgumentException("DateTime for aptitude test cannot be empty.", nameof(dateTime));

            string programme = await ConvertCourseIdToStringAsync(application.CourseId);

            var emailData = CreateEmailData(application, programme, EmailType.Aptitude, dateTime);

            await SendEmailAsync(emailData);

        }

        /// <summary>
        /// Converts the date string from to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateString">The date string to be converted.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        private DateTime ConvertToDateTime(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new ArgumentException("Input string cannot be null or empty.", nameof(dateString));
            }

            string dateFormat = "dd/MM/yyyy";


            if (DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new FormatException("The input string is not in the correct format.");
            }
        }

        /// <summary>
        /// Inspects a list of applications and checks if there are any with a status of "Pending" for more than 24 hours.
        /// If such applications are found, notifications are sent.
        /// </summary>
        /// <param name="applications">The list of applications to inspect.</param>
        private void InspectPendingApplications(List<ApplicationsViewModel> applications)
        {
            if (applications == null || !applications.Any())
            {
                _logger.LogInformation("No applications to inspect.");

                return;
            }

            DateTime twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);

            var staleApplications = applications.Where(x => x.Status.ToString() == ApplicationStatus.Pending.ToString() && x.SubmittedDate <= twentyFourHoursAgo).ToList();

            if (staleApplications.Any())
            {
                _logger.LogInformation($"{staleApplications.Count} pending applications are older than 24 hours.");

                foreach (var application in staleApplications)
                {
                    SendNotification(application);
                }
            }
            else
            {
                _logger.LogInformation("No pending applications older than 24 hours.");
            }
        }

        /// <summary>
        /// Sends a notification for a given application.
        /// </summary>
        /// <param name="application">The application for which to send the notification.</param>
        private void SendNotification(ApplicationsViewModel application)
        {
            Helper.SendSMS("", "");
        }

        /// <summary>
        /// Asynchronously gets the count of new applications with a status of "Submitted".
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the count of new applications with a status of "Submitted".</returns>
        private async Task<int> NewApplicationCountAsync()
        {
            var applications = await _context.Applications.GetAllAsync();

            return applications.Count(a => a.Status == ApplicationStatus.Submitted);
        }

        /// <summary>
        /// Asynchronously gets the count of pending applications with a status of "Pending".
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the count of pending applications with a status of "Pending".</returns>
        private async Task<int> PendingApplicationCountAsync()
        {
            var applications = await _context.Applications.GetAllAsync();

            return applications.Count(a => a.Status == ApplicationStatus.Pending);
        }

        /// <summary>
        /// Converts an <see cref="Application"/> entity to an <see cref="ApplicationsViewModel"/>.
        /// </summary>
        /// <param name="item">The application entity to convert.</param>
        /// <returns>A task representing the asynchronous operation, containing the converted <see cref="ApplicationsViewModel"/>.</returns>
        private async Task<ApplicationsViewModel> ConvertToApplicationsDTOAsync(Application item)
        {
            return new ApplicationsViewModel
            {
                ApplicationId = item.ApplicationId,
                IDNumber = item.IDNumber ?? "0000000000000",
                Email = item.Email,
                Status = item.Status.ToString(),
                SubmittedDate = ConvertToDateTime(Helper.ExtractDateFromReference(item.ReferenceNumber)),
                Cellphone = item.Cellphone,
                Course = await ConvertCourseIdToStringAsync(item.CourseId),
                Names = $"{item.ApplicantName} {item.ApplicantSurname}",
                Reference = item.ReferenceNumber,
                IDPassDoc = item.IDPassDoc ?? "Id Error",
                QualificationDoc = item.HighestQualDoc ?? "Qualification Error",
                ApplicantGuardian = item.ApplicantGuardian == null ? null : new Guardian
                {
                    IDDoc = item.ApplicantGuardian.IDDoc ?? "N/A",
                    ApplicationId = item.ApplicantGuardian.ApplicationId,
                    Cellphone = item.ApplicantGuardian.Cellphone ?? "0000000000",
                    FirstName = item.ApplicantGuardian.FirstName ?? "N/A",
                    LastName = item.ApplicantGuardian.LastName ?? "N/A",
                    GuardianId = item.ApplicantGuardian.GuardianId,
                    Relationship = item.ApplicantGuardian.Relationship
                },
                ApplicantAddress = item.ApplicantAddress
            };
        }

        /// <summary>
        /// Converts an <see cref="Application"/> entity to an <see cref="ApplyViewModel"/>.
        /// </summary>
        /// <param name="application">The application entity to convert.</param>
        /// <returns>The converted <see cref="ApplyViewModel"/>.</returns>
        private ApplyViewModel ConvertToApplyDTO(Application application)
        {
            return new ApplyViewModel
            {
                #region Applicant
                ApplicantId = application.ApplicationId,
                ApplicantIDPassFile = application.IDPassFile,
                ApplicantName = application.ApplicantName,
                ApplicantSurname = application.ApplicantSurname,
                ApplicantTitle = application.ApplicantTitle,
                Gender = application.Gender,
                IDNumber = application.IDNumber,
                IDPassDoc = application.IDPassDoc,
                ResidenceDoc = application.ResidenceDoc,
                #endregion

                #region Contact
                Cellphone = application.Cellphone,
                Email = application.Email,
                #endregion

                #region Course
                CourseId = application.CourseId,
                Status = application.Status,
                Selection = application.Selection,
                ReferenceNumber = application.ReferenceNumber,
                StudyPermitCategory = application.StudyPermitCategory,
                HighestQualification = application.HighestQualification,
                HighestQualDoc = application.HighestQualDoc,
                #endregion

                #region Guardian
                GuardianFirstName = application.ApplicantGuardian.FirstName,
                GuardianLastName = application.ApplicantGuardian.LastName,
                GuardianCellphone = application.ApplicantGuardian.Cellphone,
                GuardianId = application.ApplicantGuardian.GuardianId,
                GuardianIDDoc = application.ApplicantGuardian.IDDoc,
                #endregion

                #region Address
                Line1 = application.ApplicantAddress.Line1,
                StreetName = application.ApplicantAddress.StreetName,
                AddressId = application.ApplicantAddress.AddressId,
                City = application.ApplicantAddress.City,
                Province = application.ApplicantAddress.Province,
                PostalCode = application.ApplicantAddress.PostalCode,
                #endregion

                #region Links
                HighestQualFileUrl = application.HighestQualFileUrl,
                ResidenceFileUrl = application.ResidenceFileUrl,
                IDPassFileUrl = application.IDPassFileUrl
                #endregion
            };
        }

        /// <summary>
        /// Converts a courseID to a meaningful string representation asynchronously.
        /// </summary>
        /// <param name="courseId">The courseID to convert.</param>
        /// <returns>A task representing the asynchronous operation, containing the course name.</returns>
        private async Task<string> ConvertCourseIdToStringAsync(Guid courseId)
        {
            if (courseId == Guid.Empty)
            {
                throw new ArgumentException("Invalid course ID provided.");
            }

            var course = await _context.Courses.GetAsync(filter: c => c.CourseId == courseId);

            if (course == null)
            {
                throw new InvalidOperationException("Course not found.");
            }

            return $"{course.CourseName} ({Helper.GetDisplayName(course.Type)}) {course.NType}";
        }

        /// <summary>
        /// Creates email data for user notification.
        /// </summary>
        private EmailDataViewModel CreateUserEmailData(Application model, string programme)
        {
            return new EmailDataViewModel
            {
                Recipient = model.Email,
                Subject = "Application Acknowledgement",
                Body = _helperService.OnSendMessage($"{model.ApplicantName} {model.ApplicantSurname}", programme, model.ReferenceNumber),
                From = "Online Application"
            };
        }

        /// <summary>
        /// Creates email data for admin notification.
        /// </summary>
        private EmailDataViewModel CreateAdminEmailData(Application model, string programme)
        {
            return new EmailDataViewModel
            {
                Recipient = _helperService.GetConfigurationValue("EmailAccounts:GeneralManager", "EmailAccounts:ICTManager"),
                Subject = "New Application Alert",
                Body = _helperService.OnSendMailToAdmin($"{model.ApplicantName} {model.ApplicantSurname}", programme, model.ReferenceNumber),
                From = "Online Application"
            };
        }

        /// <summary>
        /// Retrieves the currently logged-in user from session storage.
        /// </summary>
        /// <returns>The current user if found and successfully deserialized; otherwise, null.</returns>
        private User OnGetCurrentUser()
        {
            string sessionUserJson = HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrEmpty(sessionUserJson))
            {
                return null;
            }

            try
            {
                User user = JsonConvert.DeserializeObject<User>(sessionUserJson);

                return user;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize session user data.");

                return null;
            }
        }

        /// <summary>
        /// Handles the uploading of application-related files and updates the application object with file paths.
        /// </summary>
        /// <param name="application">The application object to be updated.</param>
        /// <param name="model">The DTO containing the file inputs.</param>
        private async Task UploadFilesBackUpAsync(Application application, ApplyViewModel model)
        {
            try
            {
                var uploadTasks = new List<Task>();

                if (application.IDPassFile != null)
                {
                    uploadTasks.Add(_blobFileService.UploadAttachmentAsync(application.IDPassFile, _containerName));
                }

                if (application.HighestQualFile != null)
                {
                    uploadTasks.Add(_blobFileService.UploadAttachmentAsync(application.HighestQualFile, _containerName));
                }

                if (application.ResidenceFile != null)
                {
                    uploadTasks.Add(_blobFileService.UploadAttachmentAsync(application.ResidenceFile, _containerName));
                }

                await Task.WhenAll(uploadTasks);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while uploading files: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously uploads multiple files for an application to blob storage, tracks the success or failure of each upload, and stores the resulting URLs.
        /// </summary>
        /// <param name="application">The application containing the files to upload and where URLs will be stored.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="application"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when one or more file uploads fail.</exception>
        private async Task UploadFilesAsync(Application application, CancellationToken cancellationToken = default)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            var fileUploadResults = new List<(string FileType, string FileName, bool Success, string ErrorMessage, string FileId)>();

            var fileMappings = new (IFormFile? File, string FileType, string DocumentType, Action<string> FileIdSetter)[]
            {
                (application.IDPassFile, "IDPass", "Application.IDPass", fileId => application.IDPassFileUrl = fileId),
                (application.HighestQualFile, "HighestQual", "Application.HighestQual", fileId => application.HighestQualFileUrl = fileId),
                (application.ResidenceFile, "Residence", "Application.Residence", fileId => application.ResidenceFileUrl = fileId),
            };

            foreach (var (file, fileType, documentType, fileIdSetter) in fileMappings)
            {
                if (file is null || file.Length <= 0)
                {
                    continue;
                }

                await UploadFileWithTrackingAsync(
                    applicationId: application.ApplicationId,
                    file: file,
                    fileType: fileType,
                    documentType: documentType,
                    fileIdSetter: fileIdSetter,
                    results: fileUploadResults,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var failedUploads = fileUploadResults.Where(r => !r.Success).ToList();
            if (failedUploads.Any())
            {
                var errorMessages = string.Join("; ", failedUploads.Select(r => $"{r.FileType} upload failed: {r.ErrorMessage}"));
                _logger.LogWarning("File uploads failed: {ErrorMessages}", errorMessages);
                throw new InvalidOperationException($"One or more file uploads failed: {errorMessages}");
            }
        }

        /// <summary>
        /// Asynchronously uploads a single file to blob storage, tracks the result, and stores the resulting URL.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="fileType">The type of file (e.g., IDPass, HighestQual, Residence).</param>
        /// <param name="urlSetter">An action to set the URL on the application object.</param>
        /// <param name="containerName">The name of the blob storage container.</param>
        /// <param name="results">The list to store upload results, including success status, error messages, and URLs.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UploadFileWithTrackingAsync(Guid applicationId, IFormFile file, string fileType, string documentType, Action<string> fileIdSetter, List<(string FileType, string FileName, bool Success, string ErrorMessage, string FileId)> results, CancellationToken cancellationToken)
        {
            try
            {
                await using var stream = file.OpenReadStream();

                var uploadResponse = await _fileUploadService.UploadAsync(
                    new UploadFileRequest(
                        FileStream: stream,
                        FileName: file.FileName,
                        ContentType: file.ContentType,
                        Metadata: new Dictionary<string, string>
                        {
                            ["Entity"] = "Application",
                            ["ApplicationId"] = applicationId.ToString("D"),
                            ["FileType"] = fileType,
                        },
                        ProviderHint: null,
                        ExpiryDate: null,
                        TenantId: null,
                        DocumentType: documentType),
                    cancellationToken).ConfigureAwait(false);

                fileIdSetter(uploadResponse.FileId);
                results.Add((fileType, file.FileName, true, string.Empty, uploadResponse.FileId));
            }
            catch (Exception ex)
            {
                results.Add((fileType, file.FileName, false, ex.Message, string.Empty));
                _logger.LogWarning(ex, "Failed to upload {FileType} file: {FileName}", fileType, file.FileName);
            }
        }

        /// <summary>
        /// Asynchronously deletes uploaded files associated with the specified application.
        /// </summary>
        /// <remarks>This method attempts to delete all uploaded files referenced by the application. If a
        /// file cannot be deleted, the failure is logged and the cleanup continues for remaining files.</remarks>
        /// <param name="application">The application whose uploaded files are to be deleted. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous cleanup operation.</returns>
        private async Task CleanupUploadedFilesAsync(Application application, CancellationToken cancellationToken)
        {
            var fileIds = new[]
            {
                application.IDPassFileUrl,
                application.HighestQualFileUrl,
                application.ResidenceFileUrl
           };

            foreach (var fileId in fileIds)
            {
                if (string.IsNullOrWhiteSpace(fileId))
                {
                    continue;
                }

                try
                {
                    await _fileUploadService.DeleteAsync(fileId, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to clean up uploaded fileId {FileId} after application save failure.", fileId);
                }
            }
        }

        /// <summary>
        /// Checks if an application with the same ID number and course ID already exists in the database.
        /// </summary>
        /// <param name="application">The application object containing details such as ID number and course ID to check for duplicates.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains 
        /// a boolean value indicating whether a duplicate application exists 
        /// (true if a duplicate is found, otherwise false).
        /// </returns>
        /// <remarks>
        /// This method performs an synchronous check in the database using the provided application details.
        /// It is used to prevent duplicate applications for the same course by the same person.
        /// </remarks>
        private async Task<bool> IsDuplicateApplication(Application application)
        {
            return await _context.Applications.ExistsAsync(a =>
                    a.IDNumber == application.IDNumber && a.CourseId == application.CourseId);
        }

        /// <summary>
        /// Checks if the user has exceeded the maximum number of applications for the current academic year.
        /// </summary>
        /// <param name="IDPassport">The unique identifier (passport/ID number) of the applicant.</param>
        /// <param name="max">The maximum allowed applications per academic year (default is 3).</param>
        /// <returns>A `ValidationResponse` indicating success or failure.</returns>
        private async Task<ValidationResponse> IsApplicationCountExceeded(string IDPassport, int academicYear, int max = 3)
        {
            int currentYear = DateTime.Now.Year;

            var applicantions = await _context.Applications.GetAllAsync();

            int applicationCount = applicantions.Where(a => a.IDNumber == IDPassport && academicYear == currentYear).Count();

            if (applicationCount >= max)
            {
                return _helperService.ErrorResponse($"Error: You have reached the maximum of {max} applications for the {currentYear} academic year.");
            }

            return _helperService.SuccessResponse("Success");

        }

        /// <summary>
        /// Processes an approved application by sending the appropriate email notification.
        /// </summary>
        /// <param name="application">The application object to process.</param>
        /// <returns>A validation response indicating success or failure.</returns>
        private async Task<ValidationResponse> ProcessApprovedApplicationAsync(Application application)
        {
            if (application == null)
            {
                const string errorMessage = "Application object is null.";

                _logger.LogError(errorMessage);

                return _helperService.ErrorResponse(errorMessage);
            }

            if (application.Status != ApplicationStatus.Approved)
            {
                string errorMessage = $"Application {application.ApplicationId} is not approved, skipping email notification.";

                _logger.LogInformation(errorMessage);

                return _helperService.ErrorResponse("Application is NOT approved");
            }

            try
            {
                return await SendApprovalEmailAsync(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email for application {ApplicationId}.", application.ApplicationId);

                return _helperService.ErrorResponse("An error occurred while processing the application.");
            }
        }

        #endregion
        #region New
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailData">The email data to send.</param>
        private async Task SendEmailAsync(EmailDataViewModel emailData)
        {
            try
            {
                _logger.LogInformation($"Sending email to {emailData.Recipient} with subject {emailData.Subject}");

                await _helperService.SendMailNotificationAsync(emailData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {emailData.Recipient}");
                throw;
            }
        }

        /// <summary>
        /// Creates an email data object based on the type of email.
        /// </summary>
        /// <param name="application">The application details.</param>
        /// <param name="programme">The program name.</param>
        /// <param name="emailType">The type of email to send.</param>
        /// <param name="additionalInfo">Additional information (e.g., reason for rejection or aptitude test date).</param>
        /// <returns>EmailDataViewModel containing the email details.</returns>
        private EmailDataViewModel CreateEmailData(Application application, string programme, EmailType emailType, string additionalInfo = null)
        {
            string subject;
            string body;

            switch (emailType)
            {
                case EmailType.Rejection:
                    subject = "Forek Online - Application Feedback";
                    body = _helperService.OnSendRejectionEmail(
                        $"{application.ApplicantName} {application.ApplicantSurname}",
                        programme, application.ReferenceNumber, additionalInfo);
                    break;

                case EmailType.Aptitude:
                    subject = "Forek Online - Aptitude Test Invitation";
                    body = _helperService.OnSendAptitudeTestInvitation(
                        $"{application.ApplicantName} {application.ApplicantSurname}",
                        programme, application.ReferenceNumber, additionalInfo);
                    break;

                case EmailType.ShortSkillsTradeTest:
                    subject = "Forek Online - Application Feedback";
                    body = _helperService.OnSendApprovalEmailToTradeAndSkills(
                        $"{application.ApplicantName} {application.ApplicantSurname}",
                        programme, application.ReferenceNumber);
                    break;

                case EmailType.TradesAndNonTrades:
                    subject = "Forek Online - Aptitude Test Invitation";
                    body = _helperService.OnSendAptitudeTestInvitation(
                        $"{application.ApplicantName} {application.ApplicantSurname}",
                        programme, application.ReferenceNumber, additionalInfo);
                    break;

                case EmailType.GenericApproval:
                    subject = "Forek Online - Application Feedback";
                    body = _helperService.OnSendGenericApprovalEmail(
                        $"{application.ApplicantName} {application.ApplicantSurname}",
                        programme, application.ReferenceNumber);
                    break;

                case EmailType.Pending:
                    subject = "Forek Online - Application Feedback";
                    body = _helperService.OnSendPendingEmail(
                        $"{application.ApplicantName} {application.ApplicantSurname}",
                        programme, application.ReferenceNumber, additionalInfo);
                    break;

                default:
                    throw new InvalidOperationException("Invalid email type specified.");
            }

            return new EmailDataViewModel
            {
                Recipient = application.Email,
                Subject = subject,
                Body = body,
                From = "Online Application"
            };
        }

        /// <summary>
        /// Sends an approval email based on the application's course category.
        /// </summary>
        /// <param name="application">The application object containing details.</param>
        /// <returns>A validation response indicating success or failure.</returns>
        private async Task<ValidationResponse> SendApprovalEmailAsync(Application application)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            var course = await _courseService.GetCourseByIdAsync(application.CourseId, includeModules: false);

            if (course is null)
            {
                _logger.LogWarning("Course not found for application {ApplicationId}. CourseId: {CourseId}", application.ApplicationId, application.CourseId);
                await SendGenericApprovalMailAsync(application);
                return _helperService.SuccessResponse("Email successfully sent for application.");
            }

            var courseType = course.Type?.Trim();

            if (string.Equals(courseType, eCourseType.OccupationalTrade.ToString(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(courseType, eCourseType.OccupationalNonTrade.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                await SendApprovalMailToTradesAsync(application);
            }
            else if (string.Equals(courseType, eCourseType.ShortSkills.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                await SendApprovalMailToShortSkillsTradeTestAsync(application);
            }
            else
            {
                await SendGenericApprovalMailAsync(application);
            }

            _logger.LogInformation("Email successfully sent for application {ApplicationId}.", application.ApplicationId);

            return _helperService.SuccessResponse("Email successfully sent for application.");
        }
        /// <summary>
        /// Validates the application object to ensure required properties are set.
        /// </summary>
        /// <param name="application">The application to validate.</param>
        private void ValidateApplication(Application application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application), "Application cannot be null.");

            if (string.IsNullOrWhiteSpace(application.Email))
                throw new ArgumentException("Applicant email cannot be empty.", nameof(application.Email));

            if (string.IsNullOrWhiteSpace(application.ApplicantName) || string.IsNullOrWhiteSpace(application.ApplicantSurname))
                throw new ArgumentException("Applicant name and surname cannot be empty.", nameof(application));
        }

        /// <summary>
        /// Handles invalid model state by logging a warning and returning the view with an error message.
        /// </summary>
        /// <param name="model">The application data submitted by the user.</param>
        /// <returns>An IActionResult representing the validation failure response.</returns>
        private IActionResult HandleInvalidModel(ApplyViewModel model)
        {
            _logger.LogWarning("Validation failed for application: {Model}", model);

            TempData["error"] = "Error: Validation error.";

            return View(model);
        }

        /// <summary>
        /// Checks if the applicant has exceeded the maximum number of allowed applications for the academic year.
        /// </summary>
        /// <param name="model">The application data submitted by the user.</param>
        /// <returns>A boolean indicating whether the applicant has reached their limit.</returns>
        private async Task<bool> IsApplicationLimitReached(ApplyViewModel model)
        {
            var response = await IsApplicationCountExceeded(!string.IsNullOrEmpty(model.IDNumber) ? model.IDNumber : model.PassportNumber, DateTime.Now.Year, 3);

            if (response.IsError)
            {
                _logger.LogWarning(response.Message);

                TempData["error"] = response.Message;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles cases where the applicant has already submitted an application for the same qualification.
        /// </summary>
        /// <param name="model">The application data submitted by the user.</param>
        /// <returns>An IActionResult representing the duplicate application response.</returns>
        private IActionResult HandleDuplicateApplication(ApplyViewModel model)
        {
            TempData["error"] = "Application already submitted - consider applying for a different qualification!";

            return View(model);
        }

        /// <summary>
        /// Handles cases where saving the application to the database fails.
        /// </summary>
        /// <param name="model">The application data submitted by the user.</param>
        /// <returns>An IActionResult representing the failure response.</returns>
        private IActionResult HandleApplicationSaveFailure(ApplyViewModel model)
        {
            TempData["error"] = "Error: Application not saved!";

            return View(model);
        }

        /// <summary>
        /// Handles a successful application submission and redirects the user to the acknowledgment page.
        /// </summary>
        /// <param name="application">The application that was successfully submitted.</param>
        /// <returns>An IActionResult representing the success response.</returns>
        private IActionResult HandleApplicationSuccess(Application application)
        {
            TempData["success"] = "Application successfully saved and submitted.";
            TempData["ApplicantName"] = application.ApplicantName;
            TempData["Reference"] = application.ReferenceNumber;

            return RedirectToAction("Acknowledgement", "Global", new
            {
                applicationId = application.ApplicationId
            });
        }

        /// <summary>
        /// Handles unexpected errors by logging the exception and returning an error response.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="model">The application data submitted by the user.</param>
        /// <returns>An IActionResult representing the error response.</returns>
        private IActionResult HandleUnexpectedError(Exception ex, ApplyViewModel model)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing the application.");

            TempData["error"] = "An unexpected error occurred. Please try again later.";

            return View(model);
        }

        #endregion
    }
}
