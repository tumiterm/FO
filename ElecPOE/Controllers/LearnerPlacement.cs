// <copyright file="LearnerPlacement.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the LearnerPlacement controller

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Handles the operations related to learner placements.
    /// </summary>
    public class LearnerPlacement : Controller
    {
        #region Private Variables
        private readonly IUnitOfWork _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHelperService _helperService;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly IPlacementService _placementService;
        private readonly ApplicationDbContext _db;
        #endregion

        /// <summary>
        /// Initializes the controller with dependencies.
        /// </summary>
        public LearnerPlacement(
            IUnitOfWork context,
            IWebHostEnvironment hostEnvironment,
            IHelperService helperService,
            IStudentService studentService,
            IUserService userService,
            IPlacementService placementService,
            ApplicationDbContext db)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _placementService = placementService ?? throw new ArgumentNullException(nameof(placementService));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// Displays the placement details of a student.
        /// </summary>
        public async Task<IActionResult> PlacementDetails(string StudentNumber)
        {
            if (string.IsNullOrEmpty(StudentNumber))
                return RedirectToAction("RouteNotFound", "Global");

            var student = await _helperService.GetStudentAsync(StudentNumber);
            if (student is null)
                return RedirectToAction("RouteNotFound", "Global");

            string fullName = $"{student.FirstName} {student.LastName}";

            var placement = await _placementService.GetPlacementByStudentNameAsync(fullName);
            if (placement is null)
                return RedirectToAction("RouteNotFound", "Global");

            ViewData["company"] = await _placementService.GetCompanyNameAsync(placement.CompanyId);
            ViewData["student"] = student;
            ViewData["PlacementId"] = placement.PlacementId;
            ViewData["placement"] = placement;
            ViewBag.Timesheets = await _db.WeeklyTimesheets
                .AsNoTracking()
                .Where(t => t.PlacementId == placement.PlacementId)
                .OrderByDescending(t => t.WeekStartDate)
                .Take(8)
                .ToListAsync();

            return View();
        }

        /// <summary>
        /// Displays the page where a learner can be placed in a company.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> PlaceLearner(string StudentNumber)
        {
            var student = await _studentService.GetStudentAsync(StudentNumber);
            if (student is null)
                return RedirectToAction("RouteNotFound", "Global");

            ViewBag.UserId = await _placementService.GetUserSelectListAsync();
            ViewBag.CompanyId = await _placementService.GetCompanySelectListAsync();
            ViewBag.CourseId = await _placementService.GetCourseSelectListAsync();

            var users = await _userService.GetUsersByRolesAsync(new[] { eSysRole.Admin, eSysRole.Facilitator });
            var company = await _context.Company.GetAllAsync();

            LearnerPlacementViewModel vm = new()
            {
                Student = $"{student.FirstName} {student.LastName}",
                StudentId = student.StudentId,
                StudentNumber = student.StudentNumber,
                Users = users,
                Companys = company
            };

            return View(vm);
        }

        /// <summary>
        /// Posts placement details to place a learner in a company.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceLearner(LearnerPlacementViewModel placement, string StudentNumber)
        {
            if (string.IsNullOrWhiteSpace(StudentNumber))
                return RedirectToAction("RouteNotFound", "Global");

            var student = await _studentService.GetStudentAsync(StudentNumber);
            if (student is null)
                return RedirectToAction("RouteNotFound", "Global");

            string studentFullName = $"{student.FirstName} {student.LastName}";

            if (string.IsNullOrWhiteSpace(placement.WorkplaceMentorName) ||
                string.IsNullOrWhiteSpace(placement.WorkplaceMentorEmail) ||
                string.IsNullOrWhiteSpace(placement.WorkplaceMentorPhone))
            {
                TempData["error"] = "Workplace mentor name, email and phone are required before confirming a placement.";
                return RedirectToAction(nameof(PlaceLearner), new { StudentNumber });
            }

            if (await _placementService.IsStudentPlacedAsync(placement.CompanyId, studentFullName))
            {
                TempData["error"] = "Error: Student is already placed at the selected company.";
                return RedirectToAction(nameof(PlaceLearner), new { StudentNumber });
            }

            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                TempData["error"] = "Session expired. Please sign in again.";
                return RedirectToAction("RouteNotFound", "Global");
            }

            placement.PlacementId = Helper.GenerateGuid();
            placement.CreatedOn = _helperService.GetCurrentTime();
            placement.CreatedBy = $"{currentUser.Name} {currentUser.LastName}";
            placement.Student ??= studentFullName;
            placement.StudentId ??= student.StudentId;
            placement.IsActive = true;

            if (placement.PlacementAgreementFile is not null && placement.PlacementAgreementFile.Length > 0)
                placement.PlacementAgreement = await UploadPlacementAgreementAsync(placement.PlacementId, placement.PlacementAgreementFile);

            var entity = MapToPlacement(placement, currentUser);

            var saved = await _placementService.CreatePlacementAsync(entity);
            if (saved is null)
            {
                TempData["error"] = "Could not save placement record.";
                return View(placement);
            }

            try
            {
                await SendPlacementOnboardingAsync(placement, student);
            }
            catch
            {
                TempData["warning"] = "Placement saved, but one or more onboarding notifications could not be sent.";
            }

            TempData["success"] = "Learner placement successful.";
            return RedirectToAction(nameof(PlacedLearners));
        }

        /// <summary>
        /// Displays a list of all placed learners.
        /// </summary>
        public async Task<IActionResult> PlacedLearners()
        {
            var placementList = await _placementService.GetActivePlacementsAsync();

            ViewBag.StatusCounts = _placementService.GetStatusCounts(placementList);
            ViewBag.StatusList = placementList.Select(p => p.Status.ToString()).ToList();
            ViewBag.PendingTimesheets = await _db.WeeklyTimesheets.CountAsync(t => t.Status == "Pending Workplace Approval");
            ViewBag.PendingCampusAcknowledgements = await _db.WeeklyTimesheets.CountAsync(t => t.Status == "Approved - Pending Campus Acknowledgement");
            ViewBag.TotalVisits = await _db.Visits.CountAsync(v => v.PlacementId != null);

            return View(placementList);
        }


        /// <summary>
        /// Submits a weekly learner timesheet against a placement and starts workplace approval.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitWeeklyTimesheet(WeeklyTimesheetViewModel model, string? StudentNumber)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please complete the required weekly timesheet fields.";
                return RedirectToAction(nameof(PlacementDetails), new { StudentNumber });
            }

            model.WeekStartDate = model.WeekStartDate == default ? GetCurrentMonday() : model.WeekStartDate.Date;
            model.WeekEndDate = model.WeekEndDate == default ? model.WeekStartDate.AddDays(6) : model.WeekEndDate.Date;

            if (model.WeekStartDate.DayOfWeek != DayOfWeek.Monday || model.WeekEndDate != model.WeekStartDate.AddDays(6))
            {
                TempData["error"] = "Timesheets must run from Monday to Sunday for one full week.";
                return RedirectToAction(nameof(PlacementDetails), new { StudentNumber });
            }

            if (model.TotalHours <= 0 || model.TotalHours > 168)
            {
                TempData["error"] = "Total hours must be between 0.25 and 168 for the selected week.";
                return RedirectToAction(nameof(PlacementDetails), new { StudentNumber });
            }

            var summedHours =
                (model.MondayHours ?? 0) +
                (model.TuesdayHours ?? 0) +
                (model.WednesdayHours ?? 0) +
                (model.ThursdayHours ?? 0) +
                (model.FridayHours ?? 0) +
                (model.SaturdayHours ?? 0) +
                (model.SundayHours ?? 0);

            if (summedHours > 0 && Math.Abs(summedHours - model.TotalHours) > 0.01m)
            {
                TempData["error"] = "Total hours must match the sum of the day-by-day hours.";
                return RedirectToAction(nameof(PlacementDetails), new { StudentNumber });
            }

            var placement = await _db.Placements.FindAsync(model.PlacementId);
            if (placement is null)
                return RedirectToAction("RouteNotFound", "Global");

            var weekAlreadySubmitted = await _db.WeeklyTimesheets
                .AsNoTracking()
                .AnyAsync(t => t.PlacementId == model.PlacementId &&
                               t.WeekStartDate == model.WeekStartDate &&
                               t.WeekEndDate == model.WeekEndDate);
            if (weekAlreadySubmitted)
            {
                TempData["warning"] = "A timesheet for this week has already been submitted.";
                return RedirectToAction(nameof(PlacementDetails), new { StudentNumber });
            }

            var timesheet = new WeeklyTimesheet
            {
                WeeklyTimesheetId = Helper.GenerateGuid(),
                PlacementId = model.PlacementId,
                WeekStartDate = model.WeekStartDate,
                WeekEndDate = model.WeekEndDate,
                TotalHours = model.TotalHours,
                MondayHours = model.MondayHours,
                TuesdayHours = model.TuesdayHours,
                WednesdayHours = model.WednesdayHours,
                ThursdayHours = model.ThursdayHours,
                FridayHours = model.FridayHours,
                SaturdayHours = model.SaturdayHours,
                SundayHours = model.SundayHours,
                ActivityDescription = model.ActivityDescription,
                SkillsApplied = model.SkillsApplied,
                LearningOutcomes = model.LearningOutcomes,
                ChallengesFaced = model.ChallengesFaced,
                Status = "Pending Workplace Approval",
                SubmittedOn = _helperService.GetCurrentTime(),
                CreatedOn = _helperService.GetCurrentTime().ToString(),
                CreatedBy = placement.Student,
                IsActive = true
            };

            if (model.EvidenceFile is not null && model.EvidenceFile.Length > 0)
                timesheet.EvidenceFileName = await UploadPlacementEvidenceAsync(timesheet.WeeklyTimesheetId, model.EvidenceFile);

            _db.WeeklyTimesheets.Add(timesheet);
            placement.ModifiedOn = Helper.OnGetCurrentDateTime();
            placement.ModifiedBy = placement.Student;
            await _db.SaveChangesAsync();

            TempData["success"] = "Weekly timesheet submitted and is pending workplace approval.";
            return RedirectToAction(nameof(PlacementDetails), new { StudentNumber });
        }

        /// <summary>
        /// Records a workplace mentor approval/rejection decision for a timesheet.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WorkplaceTimesheetDecision(Guid weeklyTimesheetId, bool approve, string? comments)
        {
            var currentUser = GetCurrentUser();
            var timesheet = await _db.WeeklyTimesheets.Include(t => t.Placement).FirstOrDefaultAsync(t => t.WeeklyTimesheetId == weeklyTimesheetId);
            if (timesheet is null)
                return RedirectToAction("RouteNotFound", "Global");

            timesheet.Status = approve ? "Approved - Pending Campus Acknowledgement" : "Rejected by Workplace Mentor";
            timesheet.WorkplaceMentorComments = comments;
            timesheet.WorkplaceMentorDecisionBy = currentUser?.Id;
            timesheet.WorkplaceMentorDecisionOn = _helperService.GetCurrentTime();
            timesheet.ModifiedOn = Helper.OnGetCurrentDateTime();
            timesheet.ModifiedBy = currentUser is null ? "Workplace Mentor" : $"{currentUser.Name} {currentUser.LastName}";

            if (timesheet.Placement is not null)
            {
                timesheet.Placement.ModifiedOn = Helper.OnGetCurrentDateTime();
                timesheet.Placement.ModifiedBy = timesheet.ModifiedBy;
            }

            await _db.SaveChangesAsync();
            TempData[approve ? "success" : "warning"] = approve
                ? "Timesheet approved. Campus mentor acknowledgement has been requested."
                : "Timesheet rejected with workplace mentor comments.";
            return RedirectToAction(nameof(OnPlaceLearner), new { PlacementId = timesheet.PlacementId });
        }

        /// <summary>
        /// Lets the campus mentor acknowledge an approved workplace timesheet.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcknowledgeTimesheet(Guid weeklyTimesheetId, string? comments)
        {
            var currentUser = GetCurrentUser();
            var timesheet = await _db.WeeklyTimesheets.Include(t => t.Placement).FirstOrDefaultAsync(t => t.WeeklyTimesheetId == weeklyTimesheetId);
            if (timesheet is null)
                return RedirectToAction("RouteNotFound", "Global");

            timesheet.Status = "Final Approved";
            timesheet.CampusMentorComments = comments;
            timesheet.CampusMentorAcknowledgedBy = currentUser?.Id;
            timesheet.CampusMentorAcknowledgedOn = _helperService.GetCurrentTime();
            timesheet.ModifiedOn = Helper.OnGetCurrentDateTime();
            timesheet.ModifiedBy = currentUser is null ? "Campus Mentor" : $"{currentUser.Name} {currentUser.LastName}";

            if (timesheet.Placement is not null)
            {
                timesheet.Placement.ModifiedOn = Helper.OnGetCurrentDateTime();
                timesheet.Placement.ModifiedBy = timesheet.ModifiedBy;
            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Timesheet acknowledged and final approved; placement progress has been refreshed.";
            return RedirectToAction(nameof(OnPlaceLearner), new { PlacementId = timesheet.PlacementId });
        }

        /// <summary>
        /// Displays the form to edit placement details for a learner.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> OnPlaceLearner(Guid PlacementId, string StudentNumber)
        {
            if (PlacementId == Guid.Empty)
                return RedirectToAction("RouteNotFound", "Global");

            var placement = await _placementService.GetPlacementByIdAsync(PlacementId);
            if (placement is null)
                return RedirectToAction("RouteNotFound", "Global");

            ViewBag.UserId = await _placementService.GetUserSelectListAsync(selectedValue: placement.PlacedBy);
            ViewBag.CompanyId = await _placementService.GetCompanySelectListAsync();
            ViewBag.CourseId = await _placementService.GetCourseSelectListAsync();
            ViewBag.ModuleId = placement.CourseId.HasValue
                ? await _placementService.GetModuleSelectListAsync(placement.CourseId.Value)
                : new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");

            string placedByName = string.Empty;
            string companyName = await _placementService.GetCompanyNameAsync(placement.CompanyId);

            try { placedByName = await ResolveUserNameAsync(placement.PlacedBy); }
            catch {  }

            PlacementViewModel dto = new()
            {
                PlacementId = placement.PlacementId,
                Student = placement.Student,
                CompanyId = placement.CompanyId.ToString(),
                PlacedBy = placement.PlacedBy.ToString(),
                StartDate = FormatDate(placement.StartDate),
                EndDate = FormatDate(placement.EndDate),
                Status = placement.Status,
                IsActive = placement.IsActive,
                CompletionDate = placement.StartDate,
                WorkplaceMentorName = placement.WorkplaceMentorName,
                WorkplaceMentorEmail = placement.WorkplaceMentorEmail,
                WorkplaceMentorPhone = placement.WorkplaceMentorPhone,
                PlacementAgreement = placement.PlacementAgreement,
                DigitalSignature = placement.DigitalSignature,
                ProgressPercentage = CalculateProgress(placement.StartDate, placement.EndDate, placement.Status),
                RiskLevel = CalculateRiskLevel(placement.StartDate, placement.EndDate, placement.Status, placement.WorkplaceMentorName),
                LastActivity = string.IsNullOrWhiteSpace(placement.ModifiedOn) ? $"Created {placement.CreatedOn}" : $"Updated {placement.ModifiedOn}",
            };

            ViewData["PlacementId"] = dto.PlacementId;
            ViewData["CompanyName"] = companyName;
            ViewData["CampusMentorName"] = placedByName;
            ViewBag.Timesheets = await _db.WeeklyTimesheets
                .AsNoTracking()
                .Where(t => t.PlacementId == placement.PlacementId)
                .OrderByDescending(t => t.WeekStartDate)
                .ToListAsync();
            ViewBag.VisitHistory = await _db.Visits
                .AsNoTracking()
                .Where(v => v.PlacementId == placement.PlacementId)
                .OrderByDescending(v => v.Date)
                .ToListAsync();

            return View(dto);
        }

        /// <summary>
        /// Updates the details of an existing placement.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPlaceLearner(Placement placement)
        {
            if (!ModelState.IsValid)
                return View();

            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                TempData["error"] = "Session expired. Please sign in again.";
                return RedirectToAction("RouteNotFound", "Global");
            }

            placement.ModifiedBy = $"{currentUser.Name} {currentUser.LastName}";
            placement.ModifiedOn = Helper.OnGetCurrentDateTime();

            if (placement.PlacementAgreementFile is not null && placement.PlacementAgreementFile.Length > 0)
                placement.PlacementAgreement = await UploadPlacementAgreementAsync(placement.PlacementId, placement.PlacementAgreementFile);

            var updated = await _placementService.UpdatePlacementAsync(placement);
            if (updated is null)
            {
                TempData["error"] = "Failed to update placement.";
                return View();
            }

            ViewData["PlacementId"] = placement.PlacementId;
            ViewData["CompanyId"] = placement.CompanyId;

            TempData["success"] = "Learner changes saved successfully.";
            return RedirectToAction(nameof(PlacedLearners));
        }

        /// <summary>
        /// Displays the learner placement modules for a specific student.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> LearnerPlacementModules(Guid StudentId)
        {
            if (StudentId == Guid.Empty)
                return NotFound();

            return View();
        }

        /// <summary>
        /// Returns modules belonging to the specified course as JSON (used by AJAX dropdowns).
        /// </summary>
        public async Task<JsonResult> GetCourseModuleId(Guid CourseId)
        {
            var modules = await _context.Modules.GetAllAsync(filter: m => m.CourseIdFK == CourseId);
            return Json(modules.ToList());
        }

        /// <summary>
        /// AJAX endpoint — checks whether a student has an existing active placement by display name.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckStudentPlacement(string studentName)
        {
            if (string.IsNullOrWhiteSpace(studentName))
                return Json(new { isPlaced = false, found = false });

            var placement = await _placementService.GetPlacementByStudentNameAsync(studentName.Trim());

            if (placement is null)
                return Json(new { isPlaced = false, found = false, studentName });

            string companyName = await _placementService.GetCompanyNameAsync(placement.CompanyId);

            return Json(new
            {
                isPlaced = true,
                found = true,
                studentName = placement.Student,
                companyName,
                startDate = FormatDate(placement.StartDate),
                endDate = FormatDate(placement.EndDate),
                status = placement.Status.ToString()
            });
        }

        #region Private

        /// <summary>
        /// Retrieves the current user from the session.
        /// </summary>
        private User? GetCurrentUser()
        {
            string? sessionJson = HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrEmpty(sessionJson))
                return null;

            try { return JsonConvert.DeserializeObject<User>(sessionJson); }
            catch (JsonException) { return null; }
        }

        /// <summary>
        /// Formats a nullable <see cref="DateTime"/> to "yyyy-MM-dd", returning an empty string when null.
        /// </summary>
        private static string FormatDate(DateTime? value)
            => value.HasValue ? value.Value.ToString("yyyy-MM-dd") : string.Empty;

        private async Task<string> ResolveUserNameAsync(Guid userId)
        {
            var user = await _userService.GetUserInfoAsync(userId);
            return user is null ? string.Empty : $"{user.Name} {user.LastName}";
        }

        private static Placement MapToPlacement(LearnerPlacementViewModel vm, User currentUser) => new()
        {
            PlacementId = vm.PlacementId,
            CompanyId = vm.CompanyId,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            Status = vm.Status,
            Student = vm.Student,
            PlacedBy = vm.PlacedBy,
            CourseId = vm.CourseId,
            Module = vm.Module,
            Duration = vm.Duration,
            StudentId = vm.StudentId,
            CreatedBy = vm.CreatedBy,
            CreatedOn = vm.CreatedOn.ToString(),
            IsActive = vm.IsActive,
            ModifiedBy = $"{currentUser.Name} {currentUser.LastName}",
            ModifiedOn = Helper.OnGetCurrentDateTime(),
            SendNotification = vm.SendNotification,
            WorkplaceMentorName = vm.WorkplaceMentorName,
            WorkplaceMentorEmail = vm.WorkplaceMentorEmail,
            WorkplaceMentorPhone = vm.WorkplaceMentorPhone,
            PlacementAgreement = vm.PlacementAgreement,
            DigitalSignature = vm.DigitalSignature
        };



        private static DateTime GetCurrentMonday()
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            return today.AddDays(-diff).Date;
        }

        private async Task<string> UploadPlacementEvidenceAsync(Guid ownerId, IFormFile file)
        {
            string folder = Path.Combine(_hostEnvironment.WebRootPath, "PlacementEvidence");
            Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(file.FileName);
            string fileName = $"{ownerId:N}{extension}";
            string path = Path.Combine(folder, fileName);

            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }

        private async Task<string> UploadPlacementAgreementAsync(Guid placementId, IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string uploadDirectory = Path.Combine(wwwRootPath, "PlacementAgreements");
            Directory.CreateDirectory(uploadDirectory);

            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string safeFileName = $"{fileName}-{placementId:N}{extension}";
            string path = Path.Combine(uploadDirectory, safeFileName);

            await using var fileStream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return safeFileName;
        }

        private async Task SendPlacementOnboardingAsync(LearnerPlacementViewModel placement, Student student)
        {
            string companyName = await _placementService.GetCompanyNameAsync(placement.CompanyId);
            string start = FormatDate(placement.StartDate);
            string end = FormatDate(placement.EndDate);
            string mentorName = string.IsNullOrWhiteSpace(placement.WorkplaceMentorName) ? "your workplace mentor" : placement.WorkplaceMentorName;
            string campusMentor = await ResolveUserNameAsync(placement.PlacedBy);

            if (placement.SendNotification && !string.IsNullOrWhiteSpace(student.Cellphone))
            {
                _helperService.SendSms(
                    $"Dear {student.FirstName}, welcome to your workplace placement at {companyName} from {start} to {end}. " +
                    $"Workplace mentor: {mentorName}. Campus mentor: {campusMentor}.",
                    student.Cellphone);
            }

            var emails = new List<EmailDataViewModel>();

            if (!string.IsNullOrWhiteSpace(student.Email))
            {
                emails.Add(new EmailDataViewModel
                {
                    Recipient = student.Email,
                    Header = "Learner Placement Confirmation",
                    Subject = "Your workplace placement has been confirmed",
                    From = "Forek Online",
                    Body = $"Welcome {student.FirstName},<br/><br/>Your placement at <strong>{companyName}</strong> runs from {start} to {end}.<br/>" +
                           $"Workplace mentor: {mentorName} ({placement.WorkplaceMentorEmail}, {placement.WorkplaceMentorPhone}).<br/>" +
                           $"Campus mentor: {campusMentor}.<br/><br/>Please submit weekly activity logs and upload evidence throughout your placement."
                });
            }

            if (!string.IsNullOrWhiteSpace(placement.WorkplaceMentorEmail))
            {
                emails.Add(new EmailDataViewModel
                {
                    Recipient = placement.WorkplaceMentorEmail,
                    Header = "New Learner Placement",
                    Subject = $"Learner assigned: {student.FirstName} {student.LastName}",
                    From = "Forek Online",
                    Body = $"Dear {mentorName},<br/><br/>{student.FirstName} {student.LastName} has been placed at {companyName} from {start} to {end}.<br/>" +
                           "Please support weekly log approvals, learning outcomes and workplace evidence."
                });
            }

            var campusUser = await _userService.GetUserInfoAsync(placement.PlacedBy);
            if (!string.IsNullOrWhiteSpace(campusUser?.Username))
            {
                emails.Add(new EmailDataViewModel
                {
                    Recipient = campusUser.Username,
                    Header = "Placement Notification",
                    Subject = $"New placement: {student.FirstName} {student.LastName}",
                    From = "Forek Online",
                    Body = $"A new placement has been created for {student.FirstName} {student.LastName} at {companyName}.<br/>" +
                           $"Workplace mentor: {mentorName}. Start: {start}. End: {end}."
                });
            }

            await Task.WhenAll(emails.Select(_helperService.SendMailNotificationAsync));
        }

        private static int CalculateProgress(DateTime? startDate, DateTime? endDate, eStatus status)
        {
            if (status == eStatus.Completed) return 100;
            if (!startDate.HasValue || !endDate.HasValue) return 0;
            if (DateTime.Today <= startDate.Value.Date) return status == eStatus.StartingSoon ? 5 : 10;
            if (DateTime.Today >= endDate.Value.Date) return 95;
            double totalDays = Math.Max(1, (endDate.Value.Date - startDate.Value.Date).TotalDays + 1);
            double elapsedDays = Math.Max(0, (DateTime.Today - startDate.Value.Date).TotalDays + 1);
            return Math.Clamp((int)Math.Round((elapsedDays / totalDays) * 100), 0, 99);
        }

        private static string CalculateRiskLevel(DateTime? startDate, DateTime? endDate, eStatus status, string? workplaceMentorName)
        {
            if (status == eStatus.DroppedOut) return "At Risk";
            if (status == eStatus.Completed) return "Good";
            if (string.IsNullOrWhiteSpace(workplaceMentorName)) return "At Risk";
            if (endDate.HasValue && endDate.Value.Date < DateTime.Today) return "At Risk";
            return status == eStatus.Started ? "Good" : "Attention";
        }

        #endregion
    }
}
