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
using Newtonsoft.Json;
using System.Numerics;
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
		private IWebHostEnvironment _hostEnvironment;
		private readonly IHelperService _helperService;
		private readonly IUserService _userService;
        private readonly ILogger<LessonPlanConfig> _logger;
        private readonly IMapper _mapper;
        private readonly IBlobFileService _blobFileService;
        private readonly IFileUploadService _fileUploadService;
        private readonly string _containerName;
        private readonly IInAppNotificationService _inAppNotificationService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LessonPlanConfig"/> class.
        /// </summary>
        /// <param name="context">The unit of work for data operations.</param>
        /// <param name="userService">Service for managing user-related operations.</param>
        /// <param name="logger">Logger for capturing system events and errors.</param>
        /// <param name="hostEnvironment">Provides information about the web hosting environment.</param>
        /// <param name="helperService">Helper service for utility functions.</param>
        /// <param name="mapper">Automapper for entity mapping.</param>
        /// <exception cref="ArgumentNullException">Thrown if any required dependency is null.</exception>
        public LessonPlanConfig(IUnitOfWork context, IUserService userService, ILogger<LessonPlanConfig> logger, IInAppNotificationService inAppNotificationService,
                                IWebHostEnvironment hostEnvironment, IHelperService helperService,IMapper mapper, IBlobFileService blobFileService, IFileUploadService fileUploadService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
			_helperService = helperService;
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blobFileService = blobFileService ?? throw new ArgumentNullException(nameof(blobFileService));
            _mapper = mapper;
            _containerName = _helperService.GetConfigurationValue("AzureStorage:Containers:LessonPlans", string.Empty);
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

            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.IDPass == IdPass);

            if (user == null)
            {
                return RedirectToAction("IncorrectID", "Report");
            }

            LessonPlanViewModel viewModel = new()
            {
                IdPass = IdPass
            };

            await PopulateCreatePlanLookupsAsync(viewModel);

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

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state invalid for IdPass: {IdPass}", lessonPlanViewModel.IdPass);

                await PopulateCreatePlanLookupsAsync(lessonPlanViewModel);

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
        /// <param name="LessonPlanId">The unique identifier of the lesson plan.</param>
        /// <returns>An <see cref="IActionResult"/> displaying the lesson plan details.</returns>
        [HttpGet]
		public async Task<IActionResult> OnModifyPlanConfig(Guid LessonPlanId)
		{
			if (LessonPlanId == Guid.Empty)
			{
				return RedirectToAction("RouteNotFound", "Global");
			}

			var plans = await _context.LessonPlans.GetAsync(filter: lp => lp.LessonPlanId == LessonPlanId);
			var users = await _context.Users.GetAllAsync(filter: u => u.Role == eSysRole.Admin);
			var courses = await _context.Courses.GetAllAsync(filter: null, includeProperties: new[] {nameof(Course.Module)} );

			IEnumerable<SelectListItem> getCourses = from n in courses
													 select new SelectListItem
													 {
														 Value = n.CourseId.ToString(),
														 Text = $"{n.CourseName} ({n.Type})"
													 };


			ViewBag.CourseId = new SelectList(getCourses, "Value", "Text");

			IEnumerable<SelectListItem> getAdmins = from n in users
													select new SelectListItem
													{
														Value = n.IDPass,
														Text = $"{n.Name} {n.LastName} ({n.StudentNumber})"
													}; 


			ViewBag.IsApprovedBy = new SelectList(getAdmins, "Value", "Text");

			LessonPlanViewModel dto = new();

			dto.LessonPlanId = LessonPlanId;
			dto.Course = plans.Course;
			dto.Module = plans.Module;
			dto.Document = plans.Document;
			dto.Funder = plans.Funder;
			dto.IdPass = plans.IdPass;
			dto.Approval = plans.Approval;
			dto.Phase = plans.Phase;
			dto.IsActive = plans.IsActive;
			dto.Reason = plans.Reason;
			dto.Reference = plans.Reference;


			return View(dto);
		}

        /// <summary>
        /// Updates the lesson plan configuration with new data.
        /// </summary>
        /// <param name="model">The updated lesson plan data.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
        public async Task<IActionResult> OnModifyPlanConfig(LessonPlanViewModel model)
		{
            string isApprovedBy = await OnConvertUserToString(model.IsApprovedBy);

            var plan = _mapper.Map<LessonPlan>(model);

            plan.IsApprovedBy = isApprovedBy;

            var updatedPlan = await _context.LessonPlans.UpdateLessonPlanAsync(plan);

            if (updatedPlan != null)
            {
                TempData["success"] = "Lesson Plan successfully saved";

                string cell = await OnGetCellphone(plan.IdPass);

                if (!string.IsNullOrEmpty(cell))
                {
                    switch (plan.Approval)
                    {
                        case eSelection.No:
                            //Helper.SendSMS(
                            //    $"Hi Your Lesson Plan with reference: {plan.Reference} " +
                            //    "is NOT approved. Try re-doing it and re-submit", cell);
                            break;

                        case eSelection.Yes:
                            // SMS sending should be commented out in theoriginal code
                            // Do not Uncomment:
                            // Helper.SendSMS(
                            //     $"Hi Your Lesson Plan with reference: {plan.Reference} " +
                            //     "has been approved.", cell);
                            break;

                        case eSelection.Pending:
                            //Helper.SendSMS(
                            //    $"Hi Your Lesson Plan with reference: {plan.Reference} " +
                            //    "is still NOT approved - kindly enquire about it to make a follow up", cell);
                            break;
                    }
                }

                return RedirectToAction("OnCreatePlan", "LessonPlanConfig", new { IdPass = plan.IdPass });
            }

            TempData["error"] = "Error: Unable to save Lesson Plan!!!";

            return View();

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

        private async Task PopulateCreatePlanLookupsAsync(LessonPlanViewModel model)
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

            model.Modules = new SelectList(Array.Empty<SelectListItem>(), "Value", "Text");
            model.ExistingPlans = existingPlans;
            model.UserDetail = user == null
                ? string.Empty
                : $"{user.Name} {user.LastName} ({user.StudentNumber})";
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
                CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString(),
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
        /// Retrieves a course by its unique identifier.
        /// </summary>
        /// <param name="CourseId">The unique identifier of the course.</param>
        /// <returns>The corresponding <see cref="Course"/> object, or null if not found.</returns>
        private async Task<Course?> OnConvertCourseId(Guid CourseId)
        {
            return await _context.Courses.GetAsync(filter: c => c.CourseId == CourseId);
        }

        /// <summary>
        /// Retrieves a module by its unique identifier.
        /// </summary>
        /// <param name="ModuleId">The unique identifier of the module.</param>
        /// <returns>The corresponding <see cref="Module"/> object.</returns>
        private async Task<Module> OnConvertModuleId(Guid ModuleId)
        {
            return await _context.Modules.GetAsync(filter: c => c.ModuleId == ModuleId);
        }

        /// <summary>
        /// Converts a user identifier to a string representation of the user's full name.
        /// </summary>
        /// <param name="IdPass">The identification pass of the user.</param>
        /// <returns>A string containing the user's full name.</returns>
        private async Task<string> OnConvertUserToString(string IdPass)
        {
            var users = await _context.Users.GetAllAsync();

            var userFilter = from n in users

                             where n.IsActive == true &&

                             n.IDPass == IdPass

                             select n;

            return $"{userFilter.First().Name} {userFilter.First().LastName}";
        }

        /// <summary>
        /// Retrieves the cellphone number of a user based on their identification pass.
        /// </summary>
        /// <param name="IdPass">The identification pass of the user.</param>
        /// <returns>The cellphone number of the user.</returns>
        private async Task<string> OnGetCellphone(string IdPass)
        {
            var users = await _context.Users.GetAllAsync();

            var userFilter = from n in users

                             where n.IsActive == true &&

                             n.IDPass == IdPass

                             select n;

            return userFilter?.First().Cellphone;
        }

        #endregion
    }
}
