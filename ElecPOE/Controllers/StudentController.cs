// <copyright file="StudentController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the StudentController class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Syncfusion.EJ2.Linq;
using System.Data;
using System.Dynamic;
using System.Xml.Linq;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Provides actions for managing student-related operations, including retrieving student lists, viewing student
    /// details, managing student documents, and handling analytics.
    /// </summary>
    /// <remarks>The <see cref="StudentController"/> class is responsible for handling HTTP requests related
    /// to students. It includes actions for retrieving student data, managing student documents, and displaying
    /// analytics. This controller requires authorization and is intended for use by roles such as "Admin",
    /// "SuperAdmin", and "Facilitator".</remarks>
    [Authorize]
    public class StudentController : Controller
    {
        #region Private Variables
        private readonly IFileUploadService _fileUploadService;
        private readonly IUnitOfWork _context;
        private readonly IHelperService _helperService;
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly string _electrical;
        private IWebHostEnvironment _hostEnvironment;
        private readonly IMemoryCache _cache;
        private readonly IPdfReportService _pdfReportService;
        private readonly EnrollmentOrchestrationService _enrollmentOrchestration;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentController"/> class.
        /// </summary>
        /// <param name="context">The unit of work for database operations.</param>
        /// <param name="hostEnvironment">The web host environment for accessing file storage.</param>
        /// <param name="helperService">The helper service for retrieving configuration values.</param>
        /// <param name="logger">The logger for logging application events.</param>
        /// <param name="studentService">The student service for handling student-related operations.</param>
        /// <param name="userService">The user service for managing user-related data.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="hostEnvironment"/> is null.</exception>
        public StudentController(IUnitOfWork context, IWebHostEnvironment hostEnvironment, IHelperService helperService, ILogger<StudentController> logger, IStudentService studentService, IUserService userService, IMemoryCache cache, IPdfReportService pdfReportService, EnrollmentOrchestrationService enrollmentOrchestration, IFileUploadService fileUploadService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _helperService = helperService;
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _electrical = _helperService.GetConfigurationValue("Courses:Occupational:Electrical", string.Empty);
            _logger = logger;
            _studentService = studentService;
            _userService = userService;
            _cache = cache;
            _pdfReportService = pdfReportService;
            _enrollmentOrchestration = enrollmentOrchestration;
        }

        /// <summary>
        /// Retrieves a list of students and populates the student list view model with the current user's name, role, and the student collection.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the student list view.</returns>

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> StudentList(Guid courseId)
        {
            try
            {
                var currentUser = _userService.OnGetCurrentUser();
                var raw = courseId == Guid.Empty
                    ? await _studentService.GetStudentListAsync()
                    : await _studentService.GetStudentsByCourseAsync(courseId, true);

                const int firstPageSize = 20;
                var firstPage = raw.OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                                   .Take(firstPageSize).ToList();

                var vm = new StudentListViewModel
                {
                    Students = firstPage,
                    Role = User.IsInRole("Admin") ? "Admin" :
                                 (User.IsInRole("Facilitator") ? "Facilitator" : "User"),
                    Name = $"{currentUser?.Name} {currentUser?.LastName}",
                    TotalCount = raw.Count(),
                    Source = "Live"
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the student list.");
                return RedirectToAction("Error", "Global");
            }
        }

        /// <summary>
        /// Retrieves a paginated list of students based on the specified filters and search criteria.
        /// </summary>
        /// <remarks>The results are sorted by the student's last name, followed by their first name. If
        /// no filters are provided, all students  are included in the results.</remarks>
        /// <param name="page">The page number to retrieve. Must be 1 or greater. Defaults to 1.</param>
        /// <param name="pageSize">The number of students to include per page. Must be between 5 and 500, inclusive. Defaults to 20.</param>
        /// <param name="q">An optional search term to filter students by their first name, last name, student number, ID number, or
        /// cellphone number. The search is case-insensitive and matches partial terms.</param>
        /// <param name="status">An optional filter for the student's active status. Use "active" to retrieve active students or "inactive"
        /// to retrieve inactive students. The comparison is case-insensitive.</param>
        /// <param name="registrationSource">An optional filter for the student's registration source. Accepts either the raw source name (e.g.,
        /// "Website", "Walk-In")  or a normalized slug (e.g., "website", "walkin"). The comparison is case-insensitive.</param>
        /// <returns>A JSON object containing the paginated list of students, including the current page, page size, total pages,
        /// total count,  and the list of student items. Each student item includes their first name, last name, ID
        /// number, cellphone number,  student number, active status, and registration source.</returns>
        [HttpGet]
        public async Task<IActionResult> PagedStudents(int page = 1, int pageSize = 20, string? q = null, string? status = null, string? registrationSource = null)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 5 or > 500 ? 20 : pageSize;

            var query = await _studentService.GetStudentListAsync();

            IEnumerable<Student> filtered = query;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                filtered = filtered.Where(s =>
                    (s.FirstName ?? "").ToLower().Contains(term) ||
                    (s.LastName ?? "").ToLower().Contains(term) ||
                    (s.StudentNumber ?? "").ToLower().Contains(term) ||
                    (s.IDNumber ?? "").ToLower().Contains(term) ||
                    (s.Cellphone ?? "").ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                bool active = status.Equals("active", StringComparison.OrdinalIgnoreCase);
                filtered = filtered.Where(s => s.IsActive == active);
            }

            if (!string.IsNullOrWhiteSpace(registrationSource))
            {
                var raw = registrationSource.Trim();
                var norm = raw.ToLowerInvariant()
                              .Replace("walkin", "walk-in")
                              .Replace("forek", "forek online");
                filtered = filtered.Where(s => string.Equals(
                    (s.RegistrationSource ?? "").Trim(),
                    norm.Equals("walk-in", StringComparison.OrdinalIgnoreCase) ? "Walk-In" :
                    norm.Equals("forek online", StringComparison.OrdinalIgnoreCase) ? "Forek Online" :
                    raw,
                    StringComparison.OrdinalIgnoreCase));
            }


            var total = filtered.Count();

            var pageItems = filtered
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    firstName = s.FirstName,
                    lastName = s.LastName,
                    idNumber = s.IDNumber,
                    cellphone = s.Cellphone,
                    studentNumber = s.StudentNumber,
                    isActive = s.IsActive,
                    registrationSource = s.RegistrationSource ?? "Unknown"
                })
                .ToList();

            return Json(new
            {
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                totalCount = total,
                items = pageItems
            });
        }

        /// <summary>
        /// Retrieves a distinct, case-insensitive list of student registration sources.
        /// </summary>
        /// <remarks>This method fetches the list of students, extracts their registration sources, and
        /// returns  a distinct, alphabetically ordered list of sources. If a student's registration source is  null, it
        /// is replaced with "Unknown".</remarks>
        /// <returns>A JSON-formatted list of distinct registration sources, sorted alphabetically. If no  students are found,
        /// the list will be empty.</returns>
        [HttpGet]
        public async Task<IActionResult> DistinctSources()
        {
            var sources = (await _studentService.GetStudentListAsync())
                .Select(s => s.RegistrationSource ?? "Unknown")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            return Json(sources);
        }

        /// <summary>
        /// Serves the Learning Portal view to the client.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Learning Portal view.</returns>
        [HttpGet]
        public IActionResult LearningPortal()
        {
            return View();
        }

        /// <summary>
        /// Redirects to the StudentAnalytics action.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that performs the redirection.</returns>
        public async Task<IActionResult> ForekOnlineStudents()
        {
            return RedirectToAction(nameof(StudentAnalytics));
        }

        /// <summary>
        /// Retrieves a paginated list of students enrolled in the Forek Online section.
        /// </summary>
        /// <remarks>The students are ordered by last name and then by first name. The response includes a
        /// custom header "X-Total-Count" indicating the total number of matched students.</remarks>
        /// <param name="page">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of students per page. Defaults to 50. Must be between 10 and 200.</param>
        /// <returns>An <see cref="IActionResult"/> containing a partial view with the list of students and the total count.</returns>
        [HttpGet]
        public async Task<IActionResult> ForekOnlineStudentsSection(int page = 1, int pageSize = 50)
        {
            pageSize = Math.Clamp(pageSize, 10, 200);

            var apiStudents = await _studentService.GetStudentListAsync();
            var (matched, count) = await _studentService.GetForekOnlineStudentsAsync(apiStudents);

            var paged = matched
                    .OrderBy(s => s.LastName)
                    .ThenBy(s => s.FirstName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

            var vm = new ForekOnlineStudentsSectionViewModel
            {
                Count = count,
                Students = paged
            };

            Response.Headers["X-Total-Count"] = count.ToString();
            return PartialView("_ForekOnlineStudents", vm);
        }

        /// <summary>
        /// Retrieves the enrollment history for a student identified by their ID or passport number.
        /// </summary>
        /// <param name="identity">The student's ID number or passport number. This parameter cannot be null or whitespace.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation. The task result contains a
        /// partial view displaying the student's enrollment history if the student is found; otherwise, a bad request
        /// response if the identity is invalid.</returns>
        [HttpGet]
        public async Task<IActionResult> ForekOnlineStudentEnrollments(string identity)
        {
            if (string.IsNullOrWhiteSpace(identity))
                return BadRequest("identity required");

            var apiStudents = await _studentService.GetStudentListAsync();
            var student = apiStudents.FirstOrDefault(s =>
                string.Equals(s.IDNumber, identity, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(s.PassportNumber, identity, StringComparison.OrdinalIgnoreCase));

            var enrollments = student?.EnrollmentHistory ?? Enumerable.Empty<EnrollmentHistory>();
            return PartialView("_ForekOnlineStudentEnrollments", enrollments);
        }

        /// <summary>
        /// Displays the "StudentAnalytics" view for students.
        /// </summary>
        /// <returns>The "StudentAnalytics" view.</returns>
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public IActionResult StudentAnalytics()
        {
            return View();
        }

        /// <summary>
        /// Retrieves the details of a student based on their student number.
        /// Redirects to "RouteNotFound" if the student number is invalid or not found.
        /// </summary>
        /// <param name="StudentNumber">The student number to search for.</param>
        /// <returns>A view displaying student details or a redirect action.</returns>
        public async Task<IActionResult> StudentDetail(string StudentNumber)
        {
            if (string.IsNullOrEmpty(StudentNumber))
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var studentList = await _studentService.GetStudentListAsync();

            var student = studentList.FirstOrDefault(s => s.StudentNumber == StudentNumber);

            if (student == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var currentUser = _userService.OnGetCurrentUser();

            ViewData["CurrentUser"] = currentUser != null ? $"{currentUser.Name} {currentUser.LastName}" : "Guest";

            ViewData["role"] = currentUser?.Role?.ToString() ?? eSysRole.None.ToString();

            return View(student);
        }

        /// <summary>
        /// Retrieves documents associated with a student based on their student number.
        /// Redirects to "RouteNotFound" if the student number is invalid or not found.
        /// </summary>
        /// <param name="StudentNumber">The student number whose documents are retrieved.</param>
        /// <returns>A view displaying student documents.</returns>
        [HttpGet]
        public async Task<IActionResult> StudentDocuments(string StudentNumber)
        {
            Student student = await _studentService.GetStudentAsync(StudentNumber);

            if (String.IsNullOrEmpty(StudentNumber) || student == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var attachments = await _context.StudentAttachment.GetAllAsync();

            var filterAttachments = from n in attachments

                                    where n.StudentNumber == StudentNumber

                                    select n;

            List<StudentAttachment> list = filterAttachments.ToList();

            dynamic refObj = new ExpandoObject();

            refObj.FileModel = list;

            ViewData["StudentNumber"] = StudentNumber;

            ViewData["name"] = $"{student.FirstName} {student.LastName}";

            ViewData["StudentId"] = $"{student.StudentId}";

            return View(refObj);
        }

        /// <summary>
        /// Handles the upload and saving of student documents.
        /// Redirects to the "StudentDocuments" page upon successful upload.
        /// </summary>
        /// <param name="attachment">The student attachment object containing file details.</param>
        /// <returns>A redirect action or an error message if upload fails.</returns
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentDocuments(StudentAttachment attachment)
        {
            Student student = await _studentService.GetStudentAsync(attachment.StudentNumber);

            attachment.AttachmentId = Helper.GenerateGuid();

            attachment.CreatedBy = $"{_userService.OnGetCurrentUser()?.Name} {_userService.OnGetCurrentUser()?.LastName}";

            attachment.CreatedOn = _helperService.GetCurrentTime().ToString();

            attachment.IsActive = true;

            attachment.StudentNumber = student.StudentNumber;

            string fileName = attachment.AttachmentFile.FileName;

            attachment.Document = $"{fileName}";

            attachment.StudentId = student.StudentId;

            if (ModelState.IsValid)
            {
                AttachmentUploader(attachment);

                StudentAttachment file = await _context.StudentAttachment.AddAsync(attachment);

                if (file != null)
                {
                    int rc = await _context.SaveAsync();

                    if (rc > 0)
                    {
                        TempData["success"] = $"{file.DocumentName} successfully saved and uploaded";

                        return RedirectToAction("StudentDocuments", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });
                    }
                    else
                    {
                        TempData["error"] = $"Error: UNABLE to saved and uploaded file!!!";
                    }
                }
                else
                {
                    TempData["error"] = $"Error: something went wrong!!!";
                }
            }
            else
            {
                TempData["error"] = $"Error: File upload required!!!";
            }

            return View();

        }

        /// <summary>
        /// Uploads a student attachment file to the server storage.
        /// </summary>
        /// <param name="attachment">The student attachment object containing file details.</param>
        public async void AttachmentUploader(StudentAttachment attachment, CancellationToken ct = default)
        {
            await using var stream = attachment.AttachmentFile.OpenReadStream();

            var upload = await _fileUploadService.UploadAsync(new UploadFileRequest(
                FileStream: stream,
                FileName: attachment.AttachmentFile.FileName,
                ContentType: attachment.AttachmentFile.ContentType,
                Metadata: new Dictionary<string, string>
                {
                    ["Entity"] = "Student",
                    ["Purpose"] = "Attachments"
                },
                ProviderHint: null,
                ExpiryDate: null,
                TenantId: null,
                DocumentType: "AssessmentQuestionImage"
            ), ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads a student attachment file.
        /// Returns an error message if the file is not found.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <returns>The requested file as a downloadable response.</returns>
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            if (filename == null)

                return Content("Sorry NO Attachment found!!!");


            var path1 = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");

            string folder = path1 + @"\Docs\" + filename;

            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
        }

        /// <summary>
        /// Removes a document from the system based on the attachment ID.
        /// Redirects to "RouteNotFound" if the attachment ID is invalid or not found.
        /// </summary>
        /// <param name="AttachmentId">The ID of the attachment to remove.</param>
        /// <returns>A redirect action or a view if the removal fails.</returns>
        public async Task<IActionResult> RemoveDocument(Guid AttachmentId)
        {
            if (AttachmentId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var attachment = await _context.StudentAttachment.GetAsync(filter: a => a.AttachmentId == AttachmentId);

            if (attachment is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var isSuccess = await _context.StudentAttachment.RemoveAsync(attachment);

            if (isSuccess)
            {
                return RedirectToAction("StudentDocuments", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });
            }

            return View();

        }

        /// <summary>
        /// Displays the "Unassigned" view for students without enrollment history.
        /// </summary>
        /// <returns>The "Unassigned" view.</returns>
        public IActionResult Unassigned()
        {
            return View();
        }

        /// <summary>
        /// Retrieves a paginated list of lesson attendees, optionally filtered by a search query.
        /// </summary>
        /// <remarks>Only users with the Admin, SuperAdmin, or Facilitator roles are authorized to access
        /// this endpoint. The attendee list excludes students without an email address. Results are ordered by last
        /// name, then first name.</remarks>
        /// <param name="q">An optional search term used to filter attendees by name, email, or student number. If null or empty, no
        /// filtering is applied.</param>
        /// <param name="page">The page number of results to return. Must be 1 or greater. Defaults to 1.</param>
        /// <param name="pageSize">The number of attendees to include per page. Must be between 10 and 100. Defaults to 20.</param>
        /// <returns>A JSON result containing the current page, page size, total count of matching attendees, and a list of
        /// attendee details for the specified page.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> LessonAttendees(string? q = null, int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 10, 100);

            var students = await _studentService.GetStudentListAsync();

            var filtered = students
                .Where(s => !string.IsNullOrWhiteSpace(s.Email))
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                filtered = filtered.Where(s =>
                    ($"{s.FirstName} {s.LastName}").Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.Email ?? "").Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.StudentNumber ?? "").Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var total = filtered.Count();

            var items = filtered
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentLookupItem(
                    FullName: $"{s.FirstName} {s.LastName}".Trim(),
                    Email: s.Email!,
                    StudentNumber: s.StudentNumber ?? string.Empty))
                .ToList();

            return Json(new { page, pageSize, totalCount = total, items });
        }

        /// <summary>
        /// Schedules a new lesson and creates attendance records for the specified attendees.
        /// </summary>
        /// <remarks>Only users in the Admin, SuperAdmin, or Facilitator roles are authorized to call this
        /// method. The method creates a new lesson and associates attendance records for students whose email addresses
        /// match those provided in the request. Duplicate or invalid email addresses are ignored. The lesson is
        /// persisted to the database, but email invitations are not sent as part of this operation.</remarks>
        /// <param name="request">The lesson invitation details, including topic, room name, start and end times, password, and attendee email
        /// addresses. Must not be null, and the end time must be after the start time.</param>
        /// <returns>An HTTP 200 response containing the scheduled lesson's details if successful; otherwise, an HTTP 400
        /// response with validation errors or a message describing the failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> ScheduleLesson([FromBody] LessonInviteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.EndUtc <= request.StartUtc)
                return BadRequest("EndUtc must be after StartUtc.");

            var roomName = request.RoomName;
            var joinUrl = BuildMeetingJoinUrl(roomName);
            request.RoomName = joinUrl;

            var organizerName = User?.Identity?.Name ?? "Facilitator";

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                RoomName = joinUrl,
                Topic = request.Topic.Trim(),
                StartUtc = request.StartUtc,
                EndUtc = request.EndUtc,
                JoinUrl = joinUrl,
                Password = request.Password,
                CreatedBy = organizerName,
                CreatedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                Status = "Scheduled",
            };

            await _context.Lessons.AddAsync(lesson);
            await _context.SaveAsync();

            var students = await _studentService.GetStudentListAsync();
            var byEmail = students
                .Where(s => !string.IsNullOrWhiteSpace(s.Email))
                .GroupBy(s => s.Email!.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var email in request.AttendeeEmails.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(email))
                    continue;

                if (!byEmail.TryGetValue(email.Trim(), out var student))
                    continue;

                var attendance = new LessonAttendance
                {
                    Id = Guid.NewGuid(),
                    LessonId = lesson.Id,
                    StudentId = student.StudentId,
                    LastEventUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                };

                await _context.LessonAttendance.AddAsync(attendance);
            }

            await _context.SaveAsync();

            await SendEmailNotificationAsync(request);

            return Ok(new
            {
                lessonId = lesson.Id,
                roomName = roomName,
                topic = request.Topic,
                joinUrl,
                startUtc = request.StartUtc,
                endUtc = request.EndUtc
            });
        }

        /// <summary>
        /// Processes a lesson attendance event for the current user, recording when the user joins or leaves a lesson.
        /// </summary>
        /// <remarks>This action is restricted to authenticated users. Users with the roles "Admin",
        /// "SuperAdmin", or "Facilitator" will receive a successful response without updating attendance records. For
        /// other users, the method updates the attendance record for the specified lesson based on the event type. The
        /// request must specify an event type of either "Joined" or "Left" (case-insensitive).</remarks>
        /// <param name="request">The lesson event details, including the lesson identifier and the type of event ("Joined" or "Left").</param>
        /// <returns>An IActionResult indicating the outcome of the operation. Returns Ok if the event is processed successfully;
        /// BadRequest if the request is invalid or the event type is not recognized; Unauthorized if the current user
        /// cannot be resolved; or NotFound if the student or attendance record is not found.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> LessonEvent([FromBody] LessonEventRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Facilitator"))
                return Ok();

            var currentUser = _userService.OnGetCurrentUser();
            var userEmail = (currentUser?.Username ?? currentUser?.EmailSignatureLink ?? User?.Identity?.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(userEmail))
                return Unauthorized("Unable to resolve current user email.");

            var students = await _studentService.GetStudentListAsync();
            var student = students.FirstOrDefault(s =>
                !string.IsNullOrWhiteSpace(s.Email) &&
                s.Email.Trim().Equals(userEmail, StringComparison.OrdinalIgnoreCase));

            if (student is null)
                return NotFound("Student record not found for current user.");

            var row = await _context.LessonAttendance.GetAsync(x =>
                x.LessonId == request.LessonId &&
                x.StudentId == student.StudentId);

            if (row is null)
                return NotFound("Attendance row not found for this student + lesson.");

            var now = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            if (request.EventType.Equals("Joined", StringComparison.OrdinalIgnoreCase))
            {
                row.JoinedUtc ??= now;
                row.LastEventUtc = now;
            }
            else if (request.EventType.Equals("Left", StringComparison.OrdinalIgnoreCase))
            {
                row.LeftUtc = now;
                row.LastEventUtc = now;

                if (row.JoinedUtc.HasValue)
                {
                    var seconds = (int)Math.Max(0, (row.LeftUtc.Value - row.JoinedUtc.Value).TotalSeconds);
                    row.DurationSeconds = seconds;
                }
            }
            else if (request.EventType.Equals("Heartbeat", StringComparison.OrdinalIgnoreCase))
            {
                row.LastEventUtc = now;
            }
            else
            {
                return BadRequest("Invalid EventType. Use 'Joined', 'Left', or 'Heartbeat'.");
            }

            await _context.LessonAttendance.UpdateLessonAttendanceAsync(row);
            return Ok();
        }

        /// <summary>
        /// Performs an advanced search for students based on course title, course type, and year, returning a paginated
        /// list of matching students.
        /// </summary>
        /// <remarks>Only users with the Admin, SuperAdmin, or Facilitator roles are authorized to access
        /// this endpoint. The returned data is paginated; to implement paging UI, additional information such as total
        /// count may be required and can be added if needed.</remarks>
        /// <param name="courseTitle">The title of the course to filter students by. Specify null to include students from all courses.</param>
        /// <param name="courseType">The type of the course to filter students by. Specify null to include students from all course types.</param>
        /// <param name="year">The year to filter students by. Specify null to include students from all years.</param>
        /// <param name="page">The page number of results to retrieve. Must be greater than or equal to 1. Defaults to 1.</param>
        /// <param name="pageSize">The number of students to include per page. Must be between 5 and 200. Defaults to 20.</param>
        /// <returns>A JSON result containing the requested page of students matching the search criteria. The result includes
        /// student details such as student number, full name, course information, start date, year, and active status.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> AdvancedStudentSearch(string? courseTitle, string? courseType, int? year, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 5, 200);

            var items = await _studentService.AdvancedStudentSearchAsync(courseTitle, courseType, year, page, pageSize);

            return Json(new
            {
                page,
                pageSize,
                items = items.Select(x => new
                {
                    studentNumber = x.StudentNumber,
                    fullName = x.FullName,
                    courseTitle = x.CourseTitle,
                    courseType = x.CourseType,
                    startDate = x.StartDate == DateTime.MinValue ? null : x.StartDate.ToString("yyyy-MM-dd"),
                    year = x.StartDate == DateTime.MinValue ? (int?)null : x.StartDate.Year,
                    isActive = x.IsActive
                })
            });
        }

        /// <summary>
        /// Generates a PDF report containing the results of an advanced student search based on the specified course
        /// title, course type, and year.
        /// </summary>
        /// <remarks>The PDF report includes up to 1,000 student records matching the provided search
        /// criteria. If no criteria are specified, all students are included in the report. The PDF does not display
        /// headers or footers.</remarks>
        /// <param name="courseTitle">The title of the course to filter students by. Specify null to include all course titles.</param>
        /// <param name="courseType">The type of the course to filter students by. Specify null to include all course types.</param>
        /// <param name="year">The academic year to filter students by. Specify null to include all years.</param>
        /// <returns>An IActionResult containing the generated PDF file of the advanced student search results. The file is
        /// returned with the MIME type 'application/pdf' and named 'advanced-student-search.pdf'.</returns>
        [HttpGet]
        public async Task<IActionResult> AdvancedStudentSearchPdf(string? courseTitle, string? courseType, int? year)
        {
            var model = await _studentService.AdvancedStudentSearchAsync(courseTitle, courseType, year, page: 1, pageSize: 1000);

            var pdfBytes = await _pdfReportService.RenderViewToPdfAsync(
                viewPath: "~/Views/Student/AdvancedStudentSearchPdf.cshtml",
                model: model,
                options: new PdfDocumentOptions
                {
                    Title = "Advanced Student Search",
                    DisplayHeader = false,
                    DisplayFooter = false
                });

            return File(pdfBytes, "application/pdf", "advanced-student-search.pdf");
        }

        /// <summary>
        /// Generates a PDF file containing detailed information about a student, including placement and workplace
        /// details if requested.
        /// </summary>
        /// <remarks>The generated PDF is based on the student's placement details and may include
        /// workplace information depending on the value of <paramref name="includeWorkplace"/>. The file is returned
        /// with the MIME type 'application/pdf' and a filename formatted as 'student-{studentNumber}.pdf'.</remarks>
        /// <param name="studentNumber">The unique identifier of the student whose details are to be printed. Cannot be null, empty, or whitespace.</param>
        /// <param name="includeWorkplace">A value indicating whether workplace information should be included in the PDF. If <see langword="true"/>,
        /// workplace details are included; otherwise, they are omitted. Defaults to <see langword="true"/>.</param>
        /// <returns>A PDF file containing the student's details if found; otherwise, a BadRequest result if the student number
        /// is invalid, or a NotFound result if the student does not exist.</returns>
        [HttpGet]
        public async Task<IActionResult> PrintStudentDetailsPdf(string studentNumber, bool includeWorkplace = true)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                return BadRequest("Student number is required.");
            }

            var response = await _studentService.GetStudentPlacementDetailsAsync(studentNumber, includeWorkplace);

            if (response?.Student is null)
            {
                return NotFound();
            }

            var placement = response.Placement;
            var hasCompany = placement?.Company is not null;

            if (hasCompany)
            {
                placement!.Company!.CompanyName = placement.Company.CompanyName ?? "N/A";
            }

            var pdfBytes = await _pdfReportService.RenderViewToPdfAsync(
                viewPath: "~/Views/Student/PrintStudentDetailsPdf.cshtml",
                model: response,
                options: new PdfDocumentOptions
                {
                    Title = $"Student Details - {studentNumber}",
                    DisplayHeader = false,
                    DisplayFooter = false
                });

            return File(pdfBytes, "application/pdf", $"student-{studentNumber}.pdf");
        }

        /// <summary>
        /// Handles the applicant lookup by ID/Passport.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Lookup(string idOrPassport)
        {
            if (string.IsNullOrWhiteSpace(idOrPassport))
            {
                TempData["error"] = "Please enter an ID Number or Passport.";
                return View(nameof(EnrollStudent), new EnrollStudentViewModel());
            }

            var model = await _enrollmentOrchestration.LookupApplicantAsync(idOrPassport);
            model.IsLookupComplete = true;
            model.AvailableCourses = await GetCourseSelectListAsync();

            return View(nameof(EnrollStudent), model);
        }

        /// <summary>
        /// Handles the enrollment form submission — enqueues a Hangfire job.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> SubmitEnrollment(EnrollStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsLookupComplete = true;
                model.AvailableCourses = await GetCourseSelectListAsync();
                return View(nameof(EnrollStudent), model);
            }

            try
            {
                var jobId = _enrollmentOrchestration.SubmitEnrollment(model);

                var vm = new EnrollStudentViewModel
                {
                    SuccessMessage = $"Enrollment submitted successfully! Job ID: {jobId}. The student will be processed shortly."
                };

                return View(nameof(EnrollStudent), vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit enrollment.");
                TempData["error"] = "An error occurred while submitting the enrollment. Please try again.";
                model.IsLookupComplete = true;
                model.AvailableCourses = await GetCourseSelectListAsync();
                return View(nameof(EnrollStudent), model);
            }
        }

        /// <summary>
        /// Enqueues a one-time Hangfire bulk import job to sync students from the
        /// selected data source (API or SQLite) into the main SQL Server database.
        /// </summary>
        /// <param name="source">The data source to import from: "API" or "SQLite". Defaults to "SQLite".</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult BulkImportStudents(string source = "SQLite")
        {
            if (!source.Equals("API", StringComparison.OrdinalIgnoreCase) && !source.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "Invalid source. Use 'API' or 'SQLite'.";
                return RedirectToAction(nameof(StudentAnalytics));
            }

            try
            {
                var jobId = _enrollmentOrchestration.EnqueueBulkImport(source);

                TempData["success"] = $"Bulk import from {source} enqueued successfully. Hangfire Job ID: {jobId}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue bulk import from {Source}.", source);
                TempData["error"] = $"Failed to enqueue bulk import: {ex.Message}";
            }

            return RedirectToAction(nameof(StudentAnalytics));
        }

        /// <summary>
        /// Displays the enrollment form. If applicationId is provided (from financial gate),
        /// auto-performs the lookup and skips to step 2.
        /// </summary>
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> EnrollStudent(Guid? applicationId = null)
        {
            if (applicationId.HasValue && applicationId.Value != Guid.Empty)
            {
                var application = await _context.Applications.GetAsync(filter: a => a.ApplicationId == applicationId.Value);

                if (application is not null)
                {
                    var identity = application.IDNumber?.Trim() ?? application.PassportNumber?.Trim();

                    if (!string.IsNullOrWhiteSpace(identity))
                    {
                        var model = await _enrollmentOrchestration.LookupApplicantAsync(identity);
                        model.IsLookupComplete = true;
                        model.AvailableCourses = await GetCourseSelectListAsync();
                        return View(model);
                    }
                }
            }

            return View(new EnrollStudentViewModel());
        }

        #region Private Methods

        /// <summary>
        /// Loads active courses as SelectListItems for the enrollment dropdown.
        /// </summary>
        private async Task<List<SelectListItem>> GetCourseSelectListAsync()
        {
            var courses = await _context.Courses.GetAllAsync(filter: c => c.IsActive, asNoTracking: true);

            return courses
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.CourseName
                })
                .OrderBy(c => c.Text)
                .ToList();
        }

        /// <summary>
        /// Builds a meeting join URL for the specified room name.
        /// </summary>
        /// <param name="roomName">The name of the meeting room to join. If null, empty, or whitespace, the default meeting URL is returned.</param>
        /// <returns>A string containing the full meeting join URL. If <paramref name="roomName"/> is null, empty, or whitespace,
        /// returns the default meeting URL; otherwise, returns a URL including the specified room name.</returns>
        private string BuildMeetingJoinUrl(string roomName)
        {
            roomName = (roomName ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(roomName))
            {
                throw new ArgumentException("roomName is required.", nameof(roomName));
            }

            foreach (var ch in roomName)
            {
                var ok = char.IsLetterOrDigit(ch) || ch is '_' or '.' or '-';
                if (!ok)
                {
                    throw new ArgumentException("roomName contains invalid characters. Use letters, numbers, underscore, dot, or dash.", nameof(roomName));
                }
            }

            return $"https://meet.jit.si/{Uri.EscapeDataString(roomName)}";
        }

        /// <summary>
        /// Builds an iCalendar (ICS) meeting invitation string with the specified details.
        /// </summary>
        /// <remarks>The returned ICS string uses the 'REQUEST' method and includes the join URL in both
        /// the location and description fields. All date and time values are formatted in UTC according to the
        /// iCalendar specification.</remarks>
        /// <param name="topic">The subject or title of the meeting event. This value appears as the event summary in the calendar invite.</param>
        /// <param name="joinUrl">The URL that participants can use to join the meeting. This value is included in both the event location and
        /// description fields.</param>
        /// <param name="startUtc">The start date and time of the meeting, specified in Coordinated Universal Time (UTC).</param>
        /// <param name="endUtc">The end date and time of the meeting, specified in Coordinated Universal Time (UTC).</param>
        /// <returns>A string containing the formatted ICS data representing the meeting invitation, suitable for use as an
        /// iCalendar file or email attachment.</returns>
        private static string BuildIcsInvite(string topic, string joinUrl, DateTime startUtc, DateTime endUtc)
        {
            var uid = Guid.NewGuid().ToString("N");
            return $"""
                    BEGIN:VCALENDAR
                    VERSION:2.0
                    PRODID:-//Forek//Learning Portal//EN
                    CALSCALE:GREGORIAN
                    METHOD:REQUEST
                    BEGIN:VEVENT
                    UID:{uid}
                    DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
                    DTSTART:{startUtc:yyyyMMddTHHmmssZ}
                    DTEND:{endUtc:yyyyMMddTHHmmssZ}
                    SUMMARY:{EscapeIcs(topic)}
                    DESCRIPTION:{EscapeIcs("Join link: " + joinUrl)}
                    LOCATION:{EscapeIcs(joinUrl)}
                    END:VEVENT
                    END:VCALENDAR
                    """;
        }

        /// <summary>
        /// Escapes special characters in a string for use in iCalendar (ICS) text fields.
        /// </summary>
        /// <remarks>This method replaces backslashes (\), semicolons (;), and commas (,) with their
        /// escaped forms, and normalizes line breaks to the ICS-compliant '\n' sequence. Use this method to ensure that
        /// text values are safely embedded in ICS files without breaking the format.</remarks>
        /// <param name="value">The string to be escaped for inclusion in an ICS file. If null, an empty string is used.</param>
        /// <returns>A string with backslashes, semicolons, commas, and line breaks escaped according to the iCalendar
        /// specification.</returns>
        private static string EscapeIcs(string value)
            => (value ?? string.Empty)
                .Replace(@"\", @"\\")
                .Replace(";", @"\;")
                .Replace(",", @"\,")
                .Replace("\r\n", @"\n")
                .Replace("\n", @"\n");

        /// <summary>
        /// Asynchronously sends an email notification about a new application.
        /// </summary>
        /// <param name="lessonInviteRequest">The application model containing the data.</param>
        private async Task SendEmailNotificationAsync(LessonInviteRequest lessonInviteRequest)
        {
            try
            {
                var emails = (lessonInviteRequest.AttendeeEmails ?? new List<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (emails.Count == 0)
                {
                    _logger.LogWarning("No attendee emails provided for lesson invite. Topic: {Topic}", lessonInviteRequest.Topic);
                    return;
                }

                var sendEmailTasks = emails.Select(email =>
                {
                    var userEmailData = CreateUserEmailData(lessonInviteRequest, email);
                    return _helperService.SendMailNotificationAsync(userEmailData);
                });

                await Task.WhenAll(sendEmailTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send lesson invitation emails. Topic: {Topic}", lessonInviteRequest.Topic);
                throw;
            }
        }

        /// <summary>
        /// Creates an email data model for sending a lesson invitation to a user based on the specified lesson invite
        /// request.
        /// </summary>
        /// <param name="lessonInviteRequest">The lesson invitation request containing details such as attendee email addresses, lesson topic, and other
        /// relevant information. Cannot be null.</param>
        /// <returns>An EmailDataViewModel instance populated with recipient, subject, body, sender, and an invitation attachment
        /// for the lesson.</returns>
        private EmailDataViewModel CreateUserEmailData(LessonInviteRequest lessonInviteRequest, string recipientEmail)
        {
            return new EmailDataViewModel
            {
                Recipient = recipientEmail,
                Header = "Forek Virtual Classroom",
                Subject = $"Lesson Invitation: {lessonInviteRequest.Topic}",
                Body = _helperService.OnSendInvitation(lessonInviteRequest),
                From = "Forek Online",
                Attachments = new List<EmailAttachmentViewModel>
                {
                    _helperService.BuildInvitationIcsAttachment(lessonInviteRequest)
                }
            };
        }
        #endregion
    }
}


