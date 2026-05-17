// <copyright file="CourseController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the CourseController class
#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
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
    /// Provides actions for managing courses and their associated modules.
    /// </summary>
    /// <remarks>This controller handles the creation, modification, and retrieval of courses and modules. It
    /// requires the user to be authorized with either "Admin" or "SuperAdmin" roles.</remarks>
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CourseController : Controller
    {
        #region Private
        private readonly IUnitOfWork _context;
        private readonly ILogger<CourseController> _logger;
        private IWebHostEnvironment _hostEnvironment;
        private IHelperService _helperService;
        private readonly IUserService _userService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseController"/> class.
        /// </summary>
        /// <param name="context">Repository for course-related operations.</param>
        /// <param name="logger">Logger for logging information, warnings, and errors.</param>
        public CourseController(IUnitOfWork context,IWebHostEnvironment hostEnvironment,ILogger<CourseController> logger, IHelperService helperService, IUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Serves the Course view to the user.
        /// </summary>
        /// <returns>A view for adding or managing courses.</returns>
        [HttpGet]
        public IActionResult Course()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Course view.");

                TempData["error"] = "An unexpected error occurred while loading the page.";

                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Handles the creation of a new course and its associated modules.
        /// </summary>
        /// <param name="course">The <see cref="CourseModuleViewModel"/> object containing course data.</param>
        /// <returns>A redirect to the course details view or an error view on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Course(CourseModuleViewModel course)
        {
            if (course == null)
            {
                _logger.LogWarning("Null course data was provided.");

                TempData["error"] = "Invalid course data provided.";

                return View(course);
            }

            //if (!ModelState.IsValid)
            //{
            //    _logger.LogWarning("Model state is invalid. Validation errors detected.");

            //    return View(course);
            //}

            try
            {
                var courseEntity = MapToCourseEntity(course);

                var response = await SaveCourseToDatabaseAsync(courseEntity);

                if (response.IsError)
                {
                    TempData["error"] = $"Error: {response.Message}";

                    return View(course);
                }

                TempData["success"] = $"{response.Message}";

                _logger.LogInformation($"Course {courseEntity.CourseName} added successfully (ID: {courseEntity.CourseId}).");

                return RedirectToAction(nameof(OnCourse), new { CourseId = courseEntity.CourseId });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a course.");

                TempData["error"] = "An unexpected error occurred. Please try again.";

                return View(course);
            }

        }

        /// <summary>
        /// Searches active courses by name for duplicate detection.
        /// Returns matching courses as JSON for AJAX consumption.
        /// </summary>
        /// <param name="q">The search query string to match against course names.</param>
        /// <returns>A JSON array of matching courses with id, name, type, and credit.</returns>
        [HttpGet]
        public async Task<IActionResult> SearchCoursesByName(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            try
            {
                var allCourses = await _context.Courses.GetAllAsync();

                var matches = (allCourses ?? Enumerable.Empty<Course>())
                    .Where(c => c.IsActive &&
                                !string.IsNullOrWhiteSpace(c.CourseName) &&
                                c.CourseName.Contains(q.Trim(), StringComparison.OrdinalIgnoreCase))
                    .Select(c => new
                    {
                        courseId = c.CourseId,
                        name = c.CourseName,
                        type = c.Type != null ? Helper.GetDisplayName(c.Type) : "Unknown",
                        credit = c.Credit
                    })
                    .Take(10)
                    .ToList();

                return Json(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses by name with query '{Query}'.", q);
                return Json(Array.Empty<object>());
            }
        }

        /// <summary>
        /// Handles the HTTP GET request to display the modules of a specified course.
        /// </summary>
        /// <remarks>This method retrieves the course details and its associated modules from the
        /// database. It sets various course-related data in the <see cref="ViewData"/> for display purposes.</remarks>
        /// <param name="CourseId">The unique identifier of the course. Must not be <see cref="Guid.Empty"/>.</param>
        /// <param name="ModuleId">The unique identifier of the module. This parameter is currently not used in the method.</param>
        /// <returns>An <see cref="IActionResult"/> that renders a view displaying the list of modules for the specified course.
        /// Redirects to "RouteNotFound" if the <paramref name="CourseId"/> is empty, or to "GetActiveCourses" if the
        /// course is not found. Redirects to "Error" in case of an unexpected error.</returns>
        [HttpGet]
        public async Task<IActionResult> OnCourse(Guid CourseId, Guid ModuleId)
        {
            if (CourseId == Guid.Empty)
            {
                _logger.LogWarning("Empty CourseId supplied to OnCourse.");
                return RedirectToAction("RouteNotFound", "Global");
            }

            try
            {
                var course = await _context.Courses.GetAsync(filter: c => c.CourseId == CourseId);

                if (course == null)
                {
                    _logger.LogWarning("Course not found for CourseId {CourseId}.", CourseId);
                    TempData["error"] = "Course not found.";
                    return RedirectToAction(nameof(GetActiveCourses));
                }

                string courseNameDisplay = course.Type == eCourseType.Nated
                    ? $"{course.Type}: ({course.CourseName} {course.NType})"
                    : $"{course.Type}: ({course.CourseName})";

                ViewData["CourseName"] = courseNameDisplay ?? string.Empty;
                ViewData["Credit"] = course.Credit;
                ViewData["Type"] = course.Type;
                ViewData["NQFLevel"] = course.NQFLevel;
                ViewData["CourseIdFK"] = CourseId;

                var moduleList = await OnGetCourseModules(CourseId) ?? new List<Module>();

                return View(moduleList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in OnCourse for CourseId {CourseId}.", CourseId);
                TempData["error"] = "An unexpected error occurred while loading the course modules.";
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Retrieves a list of active courses with their associated modules.
        /// </summary>
        /// <returns>A view containing the list of active courses.</returns>
        [HttpGet]
        public async Task<IActionResult> GetActiveCourses()
        {
            try
            {
                _logger.LogInformation("Fetching course data...");

                var courseEntities = await _context.Courses.GetAllAsync();

                if (courseEntities == null || !courseEntities.Any())
                {
                    _logger.LogWarning("No courses were found in the database.");

                    return View(new List<CourseViewModel>());
                }

                var courses = courseEntities
                    .Where(course => course.IsActive)
                    .Select(ConvertToCourseDto)
                    .ToList();

                _logger.LogInformation("Successfully fetched and transformed course data.");

                return View(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the courses.");

                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }

        /// <summary>
        /// Handles the HTTP POST request to process the specified course data.
        /// </summary>
        /// <param name="course">The course data to be processed. Cannot be null.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the view for the course processing result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnCourse(Course course)
        {
            return View();
        }

        /// <summary>
        /// Handles the HTTP GET request for modifying a specific course.
        /// </summary>
        /// <param name="CourseId">The unique identifier of the course to be modified.</param>
        /// <returns>A view with the course details populated for modification, or a redirection if the course ID is invalid.</returns>
        [HttpGet]
        public async Task<IActionResult> OnModifyCourse(Guid CourseId)
        {
            if (CourseId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var course = await _context.Courses.GetAsync(filter: c => c.CourseId == CourseId);

            var courseModule = course != null ? MapToCourseModuleDTO(course) : null;

            return View(courseModule);
        }

        /// <summary>
        /// Handles the HTTP POST request for modifying a course.
        /// </summary>
        /// <param name="model">The course data transfer object containing the updated course details.</param>
        /// <returns>
        /// Redirects to the list of active courses on successful save, 
        /// or reloads the modification view with error messages if saving fails.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnModifyCourse(CourseModuleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Validation errors occurred.";

                return View(model);
            }

            var course = MapToCourseEntityForUpdate(model);

            if (course == null)
            {
                TempData["error"] = "Error: Unable to map course details.";

                return View(model);
            }

            try
            {
                var updatedCourse = await _context.Courses.UpdateCourseAsync(course);

                if (updatedCourse != null)
                {
                    TempData["success"] = "Course saved successfully!";

                    return RedirectToAction(nameof(GetActiveCourses), "Course");
                }
                else
                {
                    TempData["error"] = "Error: Unable to save course!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while modifying the course.");

                TempData["error"] = "An unexpected error occurred.";
            }

            return View(model);
        }

        /// <summary>
        /// Removes a module by its unique identifier.
        /// </summary>
        /// <param name="ModuleId">The unique identifier of the module to be removed.</param>
        /// <returns>
        /// Redirects to the associated course view on successful removal, 
        /// or an error view if the module cannot be found or deletion fails.
        /// </returns>
        public async Task<IActionResult> RemoveModule(Guid ModuleId)
        {
            if (ModuleId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            try
            {
                var module = await _context.Modules.GetAsync(filter: m => m.ModuleId == ModuleId);

                if (module == null)
                {
                    return RedirectToAction("RouteNotFound", "Global");
                }

                var deleteResult = await _context.Modules.RemoveAsync(module);

                if (deleteResult)
                {
                    TempData["success"] = "Module removed successfully!";

                    return RedirectToAction("OnCourse", new { CourseId = module.CourseIdFK });
                }
                else
                {
                    TempData["error"] = "Error: Unable to remove module.";
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing a module.");

                TempData["error"] = "An unexpected error occurred.";

            }

            return View();
        }

        /// <summary>
        /// Adds a new course module based on the provided module data.
        /// </summary>
        /// <remarks>This method validates the provided module data and attempts to save it. If the data
        /// is valid and the save operation is successful, the user is redirected to the course view. If there is an
        /// error during the save operation, an error message is displayed.</remarks>
        /// <param name="model">The <see cref="ModuleViewModel"/> containing the module details to be added.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the course view if the operation is successful; otherwise,
        /// returns a view with error details if the operation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCourseModule(ModuleViewModel model)
        {
            var module = MapModule(model);

            if (ModelState.IsValid)
            {
                var results = await SaveModuleAsync(module);

                if (results.IsError)
                {
                    TempData["error"] = results.Message;

                    return View(results);
                }

                TempData["success"] = results.Message;

            }
            return RedirectToAction("OnCourse", "Course", new { CourseId = model.CourseIdFK });
        }

        #region Private Methods

        /// <summary>
        /// Retrieves the list of modules associated with a given course ID.
        /// </summary>
        /// <param name="CourseId">The unique identifier of the course whose modules are to be fetched.</param>
        /// <returns>A list of course modules filtered by the provided course ID.</returns>
        private async Task<List<Module>> OnGetCourseModules(Guid CourseId)
        {
            if (CourseId == Guid.Empty)
            {
                _logger.LogWarning("CourseId empty in OnGetCourseModules.");
                return new List<Module>();
            }

            try
            {
                var modules = await _context.Modules.GetAllAsync();

                if (modules == null)
                {
                    _logger.LogWarning("Module repository returned null for CourseId {CourseId}.", CourseId);
                    return new List<Module>();
                }

                return modules
                    .Where(m => m != null &&
                                m.CourseIdFK == CourseId &&
                                !string.IsNullOrWhiteSpace(m.ModuleName))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving modules for CourseId {CourseId}.", CourseId);
                return new List<Module>();
            }
        }

        /// <summary>
        /// Saves the course entity to the database and commits changes.
        /// </summary>
        /// <param name="course">The <see cref="Course"/> entity to save.</param>
        /// <returns>A boolean indicating success or failure.</returns>
        private async Task<ValidationResponse> SaveCourseToDatabaseAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            var normalizedCourseName = course.CourseName?.Trim();

            if (await _context.Courses.ExistsAsync(c => c.CourseName.Trim() == normalizedCourseName && c.Type == course.Type))
            {
                return new ValidationResponse("Error: Failed to save course - course already exists!");
            }

            var createdCourse = await _context.Courses.AddAsync(course);

            if (createdCourse == null)
            {
                _logger.LogWarning("Failed to save course entity to the database.");

                return new ValidationResponse("Error: Failed to save course entity to the database.");
            }

            int saveResult = await _context.SaveAsync();

            if (saveResult > 0)
            {
                return new ValidationResponse();
            }

            return new ValidationResponse("Error: Something went wrong");
        }

        /// <summary>
        /// Maps a <see cref="CourseModuleViewModel"/> to a <see cref="Course"/> entity.
        /// </summary>
        /// <param name="courseDto">The course data transfer object.</param>
        /// <returns>A <see cref="Course"/> entity.</returns>
        private Course MapToCourseEntity(CourseModuleViewModel courseDto)
        {
            var courseId = Helper.GenerateGuid();
            var user = _userService.OnGetCurrentUser();

            var course = new Course
            {
                CourseId = courseId,
                CourseName = courseDto.Name,
                NType = courseDto.NType,
                NQFLevel = courseDto.CourseNQFLevel,
                Credit = courseDto.CourseCredit,
                IsActive = true,
                CreatedBy = $"{user?.Name} {user?.LastName}",
                CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString(),
                Type = courseDto.Type,
                IsEligibleForOnlineApplications = true, //hardcoded for now
                MinimumRequirement = courseDto.MinimumRequirement,
            };

            if (string.IsNullOrEmpty(courseDto.ModuleName))
            {
                course.Module = new List<Module>();
            }
            else
            {
                course.Module = new List<Module>
            {
                new Module
                {
                    ModuleId = Helper.GenerateGuid(),
                    CourseIdFK = courseId,
                    ModuleName = courseDto.ModuleName,
                    Credit = courseDto.Credit,
                    NQFLevel = courseDto.NQFLevel,
                    IsActive = true
                }
            };
            }

            return course;
        }

        /// <summary>
        /// Converts a course entity to its corresponding DTO.
        /// </summary>
        /// <param name="courseEntity">The course entity to convert.</param>
        /// <returns>A <see cref="CourseDTO"/> representing the course.</returns>
        private CourseViewModel ConvertToCourseDto(Course courseEntity)
        {
            if (courseEntity == null) throw new ArgumentNullException(nameof(courseEntity));

            return new CourseViewModel
            {
                CourseId = courseEntity.CourseId,
                CourseName = courseEntity.CourseName.TruncateWithEllipsisSmart(),
                Credit = courseEntity.Credit,
                CreatedBy = courseEntity.CreatedBy,
                CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString(),
                IsActive = courseEntity.IsActive,
                ModifiedBy = courseEntity.ModifiedBy,
                ModifiedOn = courseEntity.ModifiedOn,
                IsEligibleForOnlineApplications = courseEntity.IsEligibleForOnlineApplications,
                MinimumRequirement = courseEntity.MinimumRequirement,
                NQFLevel = courseEntity.NQFLevel != null ? Helper.GetDisplayName(courseEntity.NQFLevel) : null,
                Type = courseEntity?.Type != null ? Helper.GetDisplayName(courseEntity.Type) : null,
                NType = courseEntity?.NType != null ? Helper.GetDisplayName(courseEntity.NType) : null,
                Modules = courseEntity?.Module?.Select(ConvertToModule).ToList()
            };
        }

        /// <summary>
        /// Converts a module entity to its corresponding DTO.
        /// </summary>
        /// <param name="moduleEntity">The module entity to convert.</param>
        /// <returns>A <see cref="ModuleDTO"/> representing the module.</returns>
        private static ModuleViewModel ConvertToModule(ForekOnline.Domain.Entities.Module moduleEntity)
        {
            if (moduleEntity == null) throw new ArgumentNullException(nameof(moduleEntity));

            return new ModuleViewModel
            {
                ModuleId = moduleEntity.ModuleId,
                ModuleName = moduleEntity.ModuleName,
                CourseIdFK = moduleEntity.CourseIdFK,
                NQFLevel = moduleEntity.NQFLevel,
                Credit = moduleEntity.Credit,
                IsActive = moduleEntity.IsActive
            };
        }

        /// <summary>
        /// Maps a CourseModuleViewModel to a Course entity for updating.
        /// </summary>
        private Course MapToCourseEntityForUpdate(CourseModuleViewModel model)
        {
            var user = _userService.OnGetCurrentUser();

            return new Course
            {
                CourseId = model.CourseId,
                CourseName = model.Name,
                Type = model.Type,
                NQFLevel = model.CourseNQFLevel,
                Credit = model.CourseCredit,
                IsActive = model.IsActive,
                ModifiedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString(),
                ModifiedBy = $"{user?.Name}   {user?.LastName}"
            };
        }

        /// <summary>
        /// Maps a Course entity to a CourseModuleViewModel.
        /// </summary>
        private CourseModuleViewModel MapToCourseModuleDTO(Course course)
        {
            return new CourseModuleViewModel
            {
                CourseId = course.CourseId,
                Name = course.CourseName,
                Type = course.Type,
                CourseNQFLevel = course.NQFLevel,
                CourseCredit = course.Credit,
                IsActive = course.IsActive,
                ModifiedOn = course.ModifiedOn,
                ModifiedBy = course.ModifiedBy,
            };
        }

        /// <summary>
        /// Maps a <see cref="ModuleViewModel"/> to a <see cref="Module"/> entity.
        /// </summary>
        /// <param name="model">The <see cref="ModuleViewModel"/> containing the data to be mapped.</param>
        /// <returns>A new <see cref="Module"/> instance populated with data from the specified <paramref name="model"/>.</returns>
        private Module MapModule(ModuleViewModel model)
        {
            return new Module()
            {
                CourseIdFK = model.CourseIdFK,
                Credit = model.Credit,
                ModuleName = model.ModuleName,
                IsActive = true,
                ModuleId = Helper.GenerateGuid(),
                NQFLevel = model.NQFLevel,
            };
        }

        /// <summary>
        /// Asynchronously saves the specified module to the database.
        /// </summary>
        /// <remarks>This method attempts to add the module to the database context and save the changes.
        /// If the operation is successful, a success response is returned. Otherwise, an error response is
        /// returned.</remarks>
        /// <param name="module">The module to be saved. Cannot be null.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the success or failure of the save operation.</returns>
        private async Task<ValidationResponse> SaveModuleAsync(Module module)
        {
            var createModule = await _context.Modules.AddAsync(module);

            if (createModule != null)
            {
                int rowsAffected = await _context.SaveAsync();

                return new ValidationResponse();
            }

            _logger.LogWarning($"Failed to create or save module for course");

            return new ValidationResponse("Failed to create or save module for course");
        }

        #endregion

    }
}


