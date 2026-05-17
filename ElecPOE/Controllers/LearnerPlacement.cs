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
            IPlacementService placementService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _placementService = placementService ?? throw new ArgumentNullException(nameof(placementService));
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

            var entity = MapToPlacement(placement, currentUser);

            var saved = await _placementService.CreatePlacementAsync(entity);
            if (saved is null)
            {
                TempData["error"] = "Could not save placement record.";
                return View(placement);
            }

            if (placement.SendNotification)
            {
                try
                {
                    string companyName = await _placementService.GetCompanyNameAsync(placement.CompanyId);
                    _helperService.SendSms(
                        $"Dear {student.FirstName}, congratulations on being selected for workplace at {companyName}. " +
                        $"You are expected to report from {FormatDate(placement.StartDate)} until {FormatDate(placement.EndDate)}",
                        student.Cellphone);
                }
                catch
                {
                    TempData["warning"] = "Placement saved, but SMS notification could not be sent.";
                }
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

            return View(placementList);
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
                CompanyId = companyName,
                PlacedBy = placedByName,
                StartDate = FormatDate(placement.StartDate),
                EndDate = FormatDate(placement.EndDate),
                Status = placement.Status,
                IsActive = placement.IsActive,
                CompletionDate = placement.StartDate,
            };

            ViewData["PlacementId"] = dto.PlacementId;

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
            SendNotification = vm.SendNotification
        };

        #endregion
    }
}