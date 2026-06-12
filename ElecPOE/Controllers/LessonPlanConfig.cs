// <copyright file="LessonPlanConfig.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the LessonPlanConfig controller

#region Usings
using AutoMapper;
using ElecPOE.Common;
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using ForekOnline.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Controller for managing lesson plans.
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
    public class LessonPlanConfig : Controller
	{
        #region Fields
        private readonly IUnitOfWork _context;
		private readonly IHelperService _helperService;
		private readonly IUserService _userService;
        private readonly ILogger<LessonPlanConfig> _logger;
        private readonly IMapper _mapper;
        private readonly IFileUploadService _fileUploadService;
        private readonly IInAppNotificationService _inAppNotificationService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LessonPlanConfig"/> class.
        /// </summary>
        /// <param name="context">The unit of work for data operations.</param>
        /// <param name="userService">Service for managing user-related operations.</param>
        /// <param name="logger">Logger for capturing system events and errors.</param>
        /// <param name="helperService">Helper service for utility functions.</param>
        /// <param name="mapper">Automapper for entity mapping.</param>
        /// <exception cref="ArgumentNullException">Thrown if any required dependency is null.</exception>
        public LessonPlanConfig(
            IUnitOfWork context,
            IUserService userService,
            ILogger<LessonPlanConfig> logger,
            IInAppNotificationService inAppNotificationService,
            IHelperService helperService,
            IMapper mapper,
            IFileUploadService fileUploadService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _inAppNotificationService = inAppNotificationService ?? throw new ArgumentNullException(nameof(inAppNotificationService));
        }

        /// <summary>
        /// GET: Returns the view for creating a new lesson plan.
        /// Retrieves courses, modules, plans, and user details based on the provided IdPass.
        /// </summary>
        /// <param name="IdPass">The identifier used to filter the user and lesson plans.</param>
        /// <returns>
        /// A view with necessary data (courses, modules, user info, and active lesson plans) 
        /// or a redirection to an error page if the IdPass is invalid.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> OnCreatePlan(string IdPass)
        {
            _logger.LogInformation("Entering OnCreatePlan GET with IdPass: {IdPass}", IdPass);

            if (string.IsNullOrWhiteSpace(IdPass))
            {
                return RedirectToAction("IncorrectID", "Report");
            }

            LessonPlanViewModel viewModel = new()
            {
                IdPass = IdPass
            };

            if (!await PopulateCreatePlanLookupsAsync(viewModel))
            {
                return RedirectToAction("IncorrectID", "Report");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnCreatePlan(LessonPlanViewModel lessonPlanViewModel)
        {
            _logger.LogInformation("Entering OnCreatePlan POST for IdPass: {IdPass}", lessonPlanViewModel?.IdPass);

            if (lessonPlanViewModel == null)
            {
                _logger.LogWarning("Received null model in OnCreatePlan POST");
                TempData["error"] = "Error: Invalid request data.";

                LessonPlanViewModel emptyModel = new()
                {
                    Courses = new SelectList(Array.Empty<SelectListItem>(), "Value", "Text"),
                    Modules = new SelectList(Array.Empty<SelectListItem>(), "Value", "Text"),
                    ExistingPlans = Array.Empty<LessonPlan>(),
                    UserDetail = string.Empty,
                    IdPass = string.Empty
                };

                return View(emptyModel);
            }

            bool userExists = await PopulateCreatePlanLookupsAsync(lessonPlanViewModel);
            if (!userExists)
            {
                ModelState.AddModelError(nameof(lessonPlanViewModel.IdPass), "The selected user could not be found.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state invalid for IdPass: {IdPass}", lessonPlanViewModel.IdPass);
                return View(lessonPlanViewModel);
            }

            try
            {
                var plan = CreateLessonPlan(lessonPlanViewModel);

                _logger.LogDebug("Created lesson plan with Reference: {Reference}", plan.Reference);

                await AttachmentUploader(plan);

                var savedPlan = await SaveLessonPlanAsync(plan);

                _logger.LogInformation("Lesson plan saved successfully with Reference: {Reference}", savedPlan.Reference);

                TempData["success"] = "Lesson plan uploaded and saved successfully.";

                await _inAppNotificationService.SendToRoleAsync(eSysRole.Admin, $"{plan.CreatedBy} has submitted his/her Lesson Plan on {DateTimeHelper.GetCurrentSastDateTimeOffset()}",
                        actionUrl: Url.Action("OnCreatePlan", "LessonPlanConfig", new { IdPass = plan.IdPass }), iconCss: "fa fa-file-alt", createdBy: $"{plan.CreatedBy}");

                return RedirectToAction("OnCreatePlan", new { IdPass = savedPlan.IdPass });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating lesson plan for IdPass: {IdPass}", lessonPlanViewModel.IdPass);

                TempData["error"] = "Error: Unable to save lesson plan. Please try again.";

                await PopulateCreatePlanLookupsAsync(lessonPlanViewModel);
                return View(lessonPlanViewModel);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCourseModuleId(Guid CourseId)
        {
            if (CourseId == Guid.Empty)
            {
                return Json(Array.Empty<object>());
            }

            var modules = await _context.Modules.GetAllAsync(filter: m => m.CourseIdFK == CourseId && m.IsActive);

            var result = modules
                .OrderBy(m => m.ModuleName)
                .Select(m => new
                {
                    moduleId = m.ModuleId,
                    moduleName = m.ModuleName ?? string.Empty
                })
                .ToList();

            return Json(result);
        }

        /// <summary>
        /// Retrieves and prepares the lesson plan configuration for modification.
        /// </summary>
        /// <param name="lessonPlanId">The unique identifier of the lesson plan.</param>
        /// <returns>An <see cref="IActionResult"/> displaying the lesson plan details.</returns>
        [HttpGet]
        public async Task<IActionResult> OnModifyPlanConfig(Guid lessonPlanId)
        {
            if (lessonPlanId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            LessonPlan? plan = await _context.LessonPlans.GetAsync(
                filter: lp => lp.LessonPlanId == lessonPlanId,
                asNoTracking: true);

            if (plan == null)
            {
                _logger.LogWarning("Lesson plan {LessonPlanId} was not found for moderation", lessonPlanId);
                return NotFound();
            }

            await PopulateModerationLookupsAsync(plan.IsApprovedBy);

            LessonPlanViewModel viewModel = _mapper.Map<LessonPlanViewModel>(plan);

            return View(viewModel);
        }

        /// <summary>
        /// Updates the moderation fields for an existing lesson plan without replacing its creation metadata.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> OnModifyPlanConfig(LessonPlanViewModel model)
        {
            if (model.LessonPlanId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            LessonPlan? plan = await _context.LessonPlans.GetAsync(
                filter: lp => lp.LessonPlanId == model.LessonPlanId);

            if (plan == null)
            {
                _logger.LogWarning("Lesson plan {LessonPlanId} was not found during moderation update", model.LessonPlanId);
                return NotFound();
            }

            ModelState.Clear();

            string? moderatorName = await OnConvertUserToString(model.IsApprovedBy);
            if (string.IsNullOrWhiteSpace(moderatorName))
            {
                ModelState.AddModelError(nameof(model.IsApprovedBy), "Select the administrator responsible for this decision.");
            }

            if (model.Approval == eSelection.No && string.IsNullOrWhiteSpace(model.Reason))
            {
                ModelState.AddModelError(nameof(model.Reason), "Provide feedback when rejecting a lesson plan.");
            }

            if (ModelState.ErrorCount > 0)
            {
                await PopulateModerationLookupsAsync(model.IsApprovedBy);
                RestoreModerationDisplayFields(model, plan);
                return View(model);
            }

            plan.Approval = model.Approval;
            plan.IsApprovedBy = moderatorName;
            plan.Reason = model.Approval == eSelection.No ? model.Reason?.Trim() : null;
            plan.IsActive = model.IsActive;
            plan.ModifiedBy = moderatorName;
            plan.ModifiedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString("O");

            await _context.LessonPlans.UpdateLessonPlanAsync(plan);

            _logger.LogInformation(
                "Lesson plan {LessonPlanId} moderated as {Approval} by {Moderator}",
                plan.LessonPlanId,
                plan.Approval,
                moderatorName);

            TempData["success"] = $"Lesson plan {plan.Reference} was updated successfully.";

            return RedirectToAction(nameof(OnCreatePlan), new { IdPass = plan.IdPass });
        }

        /// <summary>
        /// Handles the downloading of an attachment.
        /// </summary>
        /// <param name="filename">The name of the file to be downloaded.</param>
        /// <returns>A file download response or an error message if the file is not found.</returns>
        public async Task<IActionResult> AttachmentDownload(string fileId, CancellationToken ct = default)
        {
            var download = await _fileUploadService.DownloadIfPresentAsync(fileId, ct).ConfigureAwait(false);

            if (download is null)
            {
                return NotFound();
            }

            return File(download.Value.FileStream, download.Value.ContentType ?? "application/octet-stream");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RemoveDocument(Guid lessonPlanId, CancellationToken ct = default)
        {
            LessonPlan? plan = await _context.LessonPlans.GetAsync(
                filter: candidate => candidate.LessonPlanId == lessonPlanId,
                cancellationToken: ct);

            if (plan == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(plan.UploadUrl))
            {
                await _fileUploadService.DeleteAsync(plan.UploadUrl, ct).ConfigureAwait(false);
            }

            User? currentUser = _userService.OnGetCurrentUser();
            plan.IsActive = false;
            plan.ModifiedBy = $"{currentUser?.Name} {currentUser?.LastName}".Trim();
            plan.ModifiedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString("O");

            await _context.LessonPlans.UpdateLessonPlanAsync(plan);

            TempData["success"] = $"Lesson plan {plan.Reference} was removed.";
            return RedirectToAction(nameof(OnCreatePlan), new { IdPass = plan.IdPass });
        }

        /// <summary>
        /// Uploads an attachment related to a lesson plan.
        /// </summary>
        /// <param name="attachment">The lesson plan object containing the file to be uploaded.</param>
        public async Task<string> AttachmentUploader(LessonPlan attachment)
		{
            var fileId = await _fileUploadService.UploadIfPresentAsync(
             file: attachment.DocumentFile,
             documentType: "LessonPlanConfig",
                 metadata: new Dictionary<string, string>
                 {
                     ["Entity"] = "LessonPlanConfig",
                     ["AttachmentId"] = attachment.LessonPlanId.ToString("D"),
                     ["Reference"] = attachment.Reference ?? string.Empty,
                     ["LessonPlanId"] = attachment.LessonPlanId.ToString("D")
                 },
             ct: HttpContext.RequestAborted).ConfigureAwait(false);

            attachment.Document = attachment.DocumentFile?.FileName;

            attachment.UploadUrl = fileId;

            return fileId ?? string.Empty;
        }

        #region Private

        private async Task<bool> PopulateCreatePlanLookupsAsync(LessonPlanViewModel model)
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.IDPass == model.IdPass);

            var courses = await _context.Courses.GetAllAsync();

            var existingPlans = await _context.LessonPlans.GetAllAsync(
                filter: p => p.IdPass == model.IdPass && p.IsActive);

            var courseItems = courses
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString("D"),
                    Text = $"{c.CourseName} ({c.Type} {c.NType})"
                })
                .ToList();

            model.Courses = new SelectList(
                courseItems,
                "Value",
                "Text",
                model.Course == Guid.Empty ? null : model.Course.ToString("D"));

            var moduleItems = model.Course == Guid.Empty
                ? Array.Empty<SelectListItem>()
                : (await _context.Modules.GetAllAsync(
                    filter: module => module.CourseIdFK == model.Course && module.IsActive))
                    .OrderBy(module => module.ModuleName)
                    .Select(module => new SelectListItem
                    {
                        Value = module.ModuleId.ToString("D"),
                        Text = module.ModuleName ?? string.Empty
                    })
                    .ToArray();

            model.Modules = new SelectList(
                moduleItems,
                "Value",
                "Text",
                model.Module == Guid.Empty ? null : model.Module.ToString("D"));

            model.ExistingPlans = existingPlans
                .OrderByDescending(plan => plan.TryGetCreatedOn(out DateTimeOffset createdOn)
                    ? createdOn
                    : DateTimeOffset.MinValue)
                .ToArray();
            model.UserDetail = user == null
                ? string.Empty
                : $"{user.Name} {user.LastName} ({user.StudentNumber})";

            return user != null;
        }


        /// <summary>
        /// Creates a new lesson plan based on the provided view model.
        /// </summary>
        /// <param name="model">The lesson plan view model containing necessary details.</param>
        /// <returns>A new <see cref="LessonPlan"/> object.</returns>
        private LessonPlan CreateLessonPlan(LessonPlanViewModel model)
        {
            User? currentUser = _userService.OnGetCurrentUser(); 

            return new LessonPlan
            {
                LessonPlanId = _helperService.GenerateGuid(),
                Course = model.Course,
                Module = model.Module,
                Document = model.Document,
                IdPass = model.IdPass,
                DocumentFile = model.DocumentFile,
                Funder = model.Funder,
                IsActive = true,
                Phase = model.Phase,
                Approval = eSelection.Pending,
                Reason = model.Reason,
                Reference = $"REF-LP/{_helperService.GenerateRandomString(3)}",
                CreatedBy = $"{currentUser?.Name} {currentUser?.LastName}",
                CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString("O"),
                UploadUrl = model.UploadUrl
            };
        }

        /// <summary>
        /// Saves the lesson plan asynchronously to the database.
        /// </summary>
        /// <param name="plan">The lesson plan to be saved.</param>
        /// <returns>The saved <see cref="LessonPlan"/> object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the lesson plan could not be saved.</exception>
        private async Task<LessonPlan> SaveLessonPlanAsync(LessonPlan plan)
        {
            LessonPlan addedPlan = await _context.LessonPlans.AddAsync(plan);

            int rowsAffected = await _context.SaveAsync();
            if (rowsAffected <= 0)
            {
                throw new InvalidOperationException("Failed to save lesson plan to database.");
            }
            return addedPlan;
        }

        /// <summary>
        /// Converts a user identifier to a string representation of the user's full name.
        /// </summary>
        /// <param name="idPass">The identification pass of the user.</param>
        /// <returns>A string containing the user's full name.</returns>
        private async Task<string?> OnConvertUserToString(string? idPass)
        {
            if (string.IsNullOrWhiteSpace(idPass))
            {
                return null;
            }

            User? user = await _context.Users.GetAsync(
                filter: candidate => candidate.IsActive && candidate.IDPass == idPass,
                asNoTracking: true);

            return user == null ? null : $"{user.Name} {user.LastName}".Trim();
        }

        private async Task PopulateModerationLookupsAsync(string? selectedAdmin)
        {
            var admins = await _context.Users.GetAllAsync(
                filter: user => user.IsActive
                    && (user.Role == eSysRole.Admin || user.Role == eSysRole.SuperAdmin),
                asNoTracking: true);

            string? selectedAdminId = admins
                .FirstOrDefault(user => user.IDPass == selectedAdmin
                    || $"{user.Name} {user.LastName}".Trim() == selectedAdmin)
                ?.IDPass;

            var adminItems = admins
                .OrderBy(user => user.Name)
                .ThenBy(user => user.LastName)
                .Select(user => new SelectListItem
                {
                    Value = user.IDPass,
                    Text = $"{user.Name} {user.LastName} ({user.StudentNumber})"
                })
                .ToArray();

            ViewBag.IsApprovedBy = new SelectList(adminItems, "Value", "Text", selectedAdminId);
        }

        private static void RestoreModerationDisplayFields(LessonPlanViewModel model, LessonPlan plan)
        {
            model.IdPass = plan.IdPass;
            model.Reference = plan.Reference;
            model.Course = plan.Course;
            model.Module = plan.Module;
            model.Phase = plan.Phase;
            model.Funder = plan.Funder;
            model.Document = plan.Document;
            model.UploadUrl = plan.UploadUrl;
        }

        #endregion
    }
}
