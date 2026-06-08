// <copyright file="CourseService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    27/10/2025 20:06:27 PM
// Purpose:         Defines the CourseService class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides services for managing courses, including creating, updating, retrieving, and deleting courses.
    /// </summary>
    /// <remarks>This service handles operations related to courses, such as adding new courses, updating
    /// existing ones,  managing course modules, and retrieving course details. It also includes caching mechanisms to
    /// optimize  performance for frequently accessed data, such as distinct categories and NQF levels.</remarks>
    public class CourseService : ICourseService
    {
        #region Privates
        private readonly ILogger<CourseService> _logger;
        private readonly IUnitOfWork _context;
        private readonly IMemoryCache _cache;
        private const string CourseCachePrefix = "course:";
        private const string CategoryCacheKey = "course:distinct-categories";
        private const string NqfCacheKey = "course:distinct-nqf";
        private readonly IHelperService _helperService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used to log diagnostic and operational messages.</param>
        /// <param name="context">The unit of work instance used to manage database transactions and repositories.</param>
        /// <param name="cache">The memory cache instance used to store and retrieve cached data.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/>, <paramref name="context"/>, or <paramref name="cache"/> is <see
        /// langword="null"/>.</exception>
        public CourseService(ILogger<CourseService> logger, IUnitOfWork context, IMemoryCache cache, IHelperService helperService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        #region Commands

        /// <summary>
        /// Adds a new course to the system asynchronously.
        /// </summary>
        /// <remarks>This method performs several validations before adding the course: <list
        /// type="bullet"> <item><description>Ensures the course name is provided and is not empty or
        /// whitespace.</description></item> <item><description>Checks for duplicate course names (case-insensitive) in
        /// the system.</description></item> <item><description>Validates that all modules, if provided, have unique
        /// names and non-empty names.</description></item> </list> If the course passes validation, it is added to the
        /// database along with its associated modules, if any. The method also invalidates relevant cache entries upon
        /// successful addition.</remarks>
        /// <param name="request">The request object containing the details of the course to be added, including the course name, type, NQF
        /// level, and optional modules. The request cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the operation. If the operation is successful,
        /// the response will indicate success. Otherwise, it will contain validation errors or an error message.</returns>
        public async Task<ValidationResponse> AddCourseAsync(CourseCreateRequest request)
        {
            if (request == null)
            {
                return new ValidationResponse("Request cannot be null");
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.CourseName))
                errors.Add("Course name is required.");

            if (errors.Count > 0)
                return new ValidationResponse(string.Join(" ", errors));

            var existingCourses = await _context.Courses.GetAllAsync();

            if (existingCourses.Any(c => string.Equals(c.CourseName.Trim(), request.CourseName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResponse("A course with the same name already exists.");
            }

            if (request.Modules != null && request.Modules.Any())
            {
                var moduleNameDuplicates = request.Modules
                                        .Where(m => !string.IsNullOrWhiteSpace(m.ModuleName))
                                        .GroupBy(m => m.ModuleName.Trim().ToLowerInvariant())
                                        .Where(g => g.Count() > 1)
                                        .Select(g => g.Key)
                                        .ToList();

                if (moduleNameDuplicates.Any())
                {
                    return new ValidationResponse($"Duplicate module names: {string.Join(", ", moduleNameDuplicates)}");
                }

                if (request.Modules.Any(m => string.IsNullOrWhiteSpace(m.ModuleName)))
                {
                    return new ValidationResponse("All modules must have a name.");
                }
            }

            int totalModuleCredit = 0;
            if (request.Modules != null)
            {
                // ModuleUpsertRequest currently has only ModuleName & NQFLevel; if Credit exists in entity but not in request skip aggregation.
                // If later extended to include Credit, you can sum here.
            }

            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                CourseName = request.CourseName.Trim(),
                Type = request.Type,
                NType = request.NType,
                NQFLevel = request.NQFLevel,
                CreatedBy = request.CreatedBy,
                CreatedOn = _helperService.GetCurrentTime().ToString("0"),
                IsActive = true,
                Credit = request.Credit > 0 ? request.Credit : totalModuleCredit,
                ModifiedBy = request.ModifiedBy,
                ModifiedOn = _helperService.GetCurrentTime().ToString("0"),
                MinimumRequirement = request.MinimumRequirement, MinimumRequirementNotes = request.MinimumRequirementNotes,
                DurationValue = request.DurationValue, DurationType = request.DurationType, StudyMode = request.StudyMode, DeliveryMethod = request.DeliveryMethod,
                IsAccredited = request.IsAccredited, AccreditationBody = request.AccreditationBody, AccreditationNumber = request.AccreditationNumber,
                IsEligibleForOnlineApplications = request.IsEligibleForOnlineApplications, RequiresAptitudeTest = request.RequiresAptitudeTest,
                RequiresInterview = request.RequiresInterview, ApplicationFee = request.ApplicationFee, RegistrationFee = request.RegistrationFee,
                TuitionFee = request.TuitionFee, MaximumStudents = request.MaximumStudents, HasCourseOptions = request.HasCourseOptions,
                Module = new List<Module>(),
            };

            if (request.Modules != null)
            {
                foreach (var m in request.Modules)
                {
                    course.Module.Add(new Module
                    {
                        ModuleId = Guid.NewGuid(),
                        ModuleName = m.ModuleName.Trim(),
                        CourseIdFK = course.CourseId,
                        NQFLevel = m.NQFLevel,
                        IsActive = true,
                        Credit = m.Credit,
                    });
                }
            }

            try
            {
                await _context.Courses.AddAsync(course);
                var saved = await _context.SaveAsync();

                if (saved > 0)
                {
                    await InvalidateCourseCacheAsync(course.CourseId);

                    _cache.Remove(CategoryCacheKey);
                    _cache.Remove(NqfCacheKey);

                    return new ValidationResponse();
                }

                return new ValidationResponse("Failed to persist course.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course {CourseName}", request.CourseName);
                return new ValidationResponse("Unexpected error occurred while adding course.");
            }
        }

        /// <summary>
        /// Updates an existing course and its associated modules based on the provided update request.
        /// </summary>
        /// <remarks>This method performs the following operations: <list type="bullet"> <item>Validates
        /// the input request, ensuring it is not null and contains a valid course ID.</item> <item>Retrieves the course
        /// from the database, including its associated modules.</item> <item>Updates the course properties and modules
        /// based on the provided request.</item> <item>Adds new modules if specified in the request.</item> <item>Saves
        /// the changes to the database and invalidates the course cache upon success.</item> </list> If the course is
        /// not found, or if the request contains invalid data, the method returns a validation error. In the event of
        /// an unexpected error, the method logs the error and returns a generic failure response.</remarks>
        /// <param name="request">The <see cref="CourseUpdateRequest"/> containing the details of the course to update, including the course
        /// ID, updated course properties, and optional module updates. The request cannot be null, and the <see
        /// cref="CourseUpdateRequest.CourseId"/> must be specified.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the update operation. If the update is
        /// successful, the response will indicate success. Otherwise, it will contain an error message describing the
        /// failure.</returns>
        public async Task<ValidationResponse> UpdateCourseAsync(CourseUpdateRequest request)
        {
            if (request == null)
                return new ValidationResponse("Request cannot be null.");

            if (request.CourseId == Guid.Empty)
                return new ValidationResponse("CourseId is required.");

            var course = await _context.Courses.GetAsync(c => c.CourseId == request.CourseId, includeProperties: new[] { nameof(Course.Module) });

            if (course == null)
                return new ValidationResponse("Course not found.");

            if (!string.IsNullOrWhiteSpace(request.CourseName))
                course.CourseName = request.CourseName.Trim();

            if (request.NType.HasValue)
                course.NType = request.NType.Value;

            if (request.NQFLevel.HasValue)
                course.NQFLevel = request.NQFLevel.Value;

            course.MinimumRequirement = request.MinimumRequirement; course.MinimumRequirementNotes = request.MinimumRequirementNotes;
            course.DurationValue = request.DurationValue; course.DurationType = request.DurationType; course.StudyMode = request.StudyMode; course.DeliveryMethod = request.DeliveryMethod;
            course.IsAccredited = request.IsAccredited; course.AccreditationBody = request.AccreditationBody; course.AccreditationNumber = request.AccreditationNumber;
            course.IsEligibleForOnlineApplications = request.IsEligibleForOnlineApplications; course.RequiresAptitudeTest = request.RequiresAptitudeTest;
            course.RequiresInterview = request.RequiresInterview; course.ApplicationFee = request.ApplicationFee; course.RegistrationFee = request.RegistrationFee;
            course.TuitionFee = request.TuitionFee; course.MaximumStudents = request.MaximumStudents; course.HasCourseOptions = request.HasCourseOptions;
            course.ModifiedOn = _helperService.GetCurrentTime().ToString("0");
            course.ModifiedBy = request.ModifiedBy;

            if (request.Modules != null && request.Modules.Any())
            {
                var existingModules = course.Module.ToDictionary(m => m.ModuleId, m => m);

                foreach (var m in request.Modules)
                {
                    if (m.ModuleId.HasValue && existingModules.TryGetValue(m.ModuleId.Value, out var existing))
                    {
                        if (!string.IsNullOrWhiteSpace(m.ModuleName))
                            existing.ModuleName = m.ModuleName.Trim();
                        existing.NQFLevel = m.NQFLevel;
                    }
                    else
                    {
                        course.Module.Add(new Module
                        {
                            ModuleId = Guid.NewGuid(),
                            ModuleName = m.ModuleName?.Trim() ?? "Unnamed Module",
                            CourseIdFK = course.CourseId,
                            NQFLevel = m.NQFLevel,
                            IsActive = true,
                            Credit = m.Credit
                        });
                    }
                }
            }

            try
            {
                await _context.Courses.UpdateCourseAsync(course);
                var saved = await _context.SaveAsync();
                if (saved > 0)
                {
                    await InvalidateCourseCacheAsync(course.CourseId);
                    return new ValidationResponse();
                }
                return new ValidationResponse("No changes saved.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course {CourseId}", request.CourseId);
                return new ValidationResponse("Unexpected error during update.");
            }
        }

        /// <summary>
        /// Adds or updates modules for the specified course.
        /// </summary>
        /// <remarks>This method retrieves the specified course and its associated modules from the
        /// database.  Existing modules are updated if their identifiers match, and new modules are added if no match is
        /// found.  The method ensures that module names are not empty or whitespace.  If the operation succeeds, the
        /// course cache is invalidated to reflect the changes.</remarks>
        /// <param name="courseId">The unique identifier of the course for which modules are being upserted. Must not be an empty GUID.</param>
        /// <param name="modules">A collection of <see cref="ModuleUpsertRequest"/> objects representing the modules to be added or updated. 
        /// Each module must have a valid name. If the collection is empty or null, the operation will fail.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the operation.  Returns a success response if
        /// the modules are successfully upserted; otherwise, returns a response with an error message.</returns>
        public async Task<ValidationResponse> UpsertModulesAsync(Guid courseId, IEnumerable<ModuleUpsertRequest> modules)
        {
            if (courseId == Guid.Empty)
                return new ValidationResponse("CourseId is required.");

            var course = await _context.Courses.GetAsync(c => c.CourseId == courseId, includeProperties: new[] { nameof(Course.Module) });

            if (course == null)
                return new ValidationResponse("Course not found.");

            if (modules == null || !modules.Any())
                return new ValidationResponse("Modules collection is empty.");

            var existing = course.Module.ToDictionary(m => m.ModuleId, m => m);

            foreach (var req in modules)
            {
                if (string.IsNullOrWhiteSpace(req.ModuleName))
                    return new ValidationResponse("Module name is required.");

                if (req.ModuleId.HasValue && existing.TryGetValue(req.ModuleId.Value, out var mod))
                {
                    mod.ModuleName = req.ModuleName.Trim();
                    mod.NQFLevel = req.NQFLevel;
                }
                else
                {
                    course.Module.Add(new Module
                    {
                        ModuleId = Guid.NewGuid(),
                        ModuleName = req.ModuleName.Trim(),
                        CourseIdFK = course.CourseId,
                        NQFLevel = req.NQFLevel,
                        IsActive = true
                    });
                }
            }

            try
            {
                await _context.Courses.UpdateCourseAsync(course);
                var saved = await _context.SaveAsync();

                if (saved > 0)
                {
                    await InvalidateCourseCacheAsync(course.CourseId);
                    return new ValidationResponse();
                }
                return new ValidationResponse("Failed to upsert modules.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Module upsert failed for course {CourseId}", courseId);
                return new ValidationResponse("Unexpected error during module upsert.");
            }
        }

        /// <summary>
        /// Toggles the active state of a course.
        /// </summary>
        /// <remarks>This method updates the active state of the specified course and records the
        /// modification timestamp.  If the operation is successful, the course cache is invalidated to ensure
        /// consistency.</remarks>
        /// <param name="courseId">The unique identifier of the course to update.</param>
        /// <param name="isActive">A value indicating whether the course should be marked as active.  true to activate the course; otherwise,
        /// false.</param>
        /// <returns>A ValidationResponse indicating the result of the operation.  If the course is successfully updated, the
        /// response will indicate success.  If the course is not found, or if an error occurs, the response will
        /// contain an appropriate error message.</returns>
        public async Task<ValidationResponse> ToggleCourseActiveAsync(Guid courseId, bool isActive)
        {
            var course = await _context.Courses.GetAsync(c => c.CourseId == courseId);
            if (course == null)
                return new ValidationResponse("Course not found.");

            course.IsActive = isActive;
            course.ModifiedOn = _helperService.GetCurrentTime().ToString("0");

            try
            {
                await _context.Courses.UpdateCourseAsync(course);
                var saved = await _context.SaveAsync();

                if (saved > 0)
                {
                    await InvalidateCourseCacheAsync(course.CourseId);
                    return new ValidationResponse();
                }
                return new ValidationResponse("Unable to toggle active state.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Toggle active failed for course {CourseId}", courseId);
                return new ValidationResponse("Unexpected error toggling active state.");
            }
        }

        /// <summary>
        /// Marks a course as inactive without permanently deleting it from the database.
        /// </summary>
        /// <remarks>This method updates the course's state to inactive and modifies its timestamp to
        /// reflect the change.  It also invalidates any cached data related to the course upon a successful
        /// operation.</remarks>
        /// <param name="courseId">The unique identifier of the course to be soft deleted.</param>
        /// <param name="reason">The reason for soft deleting the course. This parameter is for logging or auditing purposes.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the operation.  Returns a success response if
        /// the course was successfully soft deleted, or an error message if the operation failed.</returns>
        public async Task<ValidationResponse> SoftDeleteCourseAsync(Guid courseId, string reason)
        {
            var course = await _context.Courses.GetAsync(c => c.CourseId == courseId);
            if (course == null)
                return new ValidationResponse("Course not found.");

            course.IsActive = false;
            course.ModifiedOn = DateTime.UtcNow.ToString("O");

            try
            {
                await _context.Courses.UpdateCourseAsync(course);
                var saved = await _context.SaveAsync();

                if (saved > 0)
                {
                    await InvalidateCourseCacheAsync(course.CourseId);
                    return new ValidationResponse();
                }
                return new ValidationResponse("Soft delete failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Soft delete failed for course {CourseId}", courseId);
                return new ValidationResponse("Unexpected error during soft delete.");
            }
        }

        /// <summary>
        /// Recalculates the total credit for the specified course based on its associated modules.
        /// </summary>
        /// <remarks>This method retrieves the course and its associated modules, calculates the total
        /// credit  based on the module credits, and updates the course's credit value. If the course is not found,  or
        /// if an error occurs during the update, an appropriate error message is returned.</remarks>
        /// <param name="courseId">The unique identifier of the course for which the credit is to be recalculated.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the operation.  Returns a successful response if
        /// the recalculation and update are completed successfully;  otherwise, returns a response with an appropriate
        /// error message.</returns>
        public async Task<ValidationResponse> RecalculateCourseCreditAsync(Guid courseId)
        {
            var course = await _context.Courses.GetAsync(c => c.CourseId == courseId, includeProperties: new[] { nameof(Course.Module) });
            if (course == null)
                return new ValidationResponse("Course not found.");

            var total = 0.0;
            if (course.Module != null && course.Module.Any())
            {
                total = course.Module.Sum(m => m.Credit) ?? 0.0;
            }

            course.Credit = total;
            course.ModifiedOn = _helperService.GetCurrentTime().ToString("0");

            try
            {
                await _context.Courses.UpdateCourseAsync(course);
                var saved = await _context.SaveAsync();
                if (saved > 0)
                {
                    await InvalidateCourseCacheAsync(course.CourseId);
                    return new ValidationResponse();
                }
                return new ValidationResponse("Credit recalculation failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Credit recalculation failed for course {CourseId}", courseId);
                return new ValidationResponse("Unexpected error recalculating credit.");
            }
        }

        #endregion

        #region Queries

        public async Task<IReadOnlyList<CourseViewModel>> GetAllCoursesAsync(bool includeModules = false, CancellationToken cancellationToken = default)
        {
            string cacheKey = CourseCachePrefix + "all" + (includeModules ? ":full" : ":lite");

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<CourseViewModel> cached))
                return cached;

            var courses = await _context.Courses.GetAllAsync(
                includeProperties: includeModules
                    ? new[] { nameof(Course.Module) }
                    : Array.Empty<string>(), cancellationToken: cancellationToken);

            if (courses == null || !courses.Any())
                return Array.Empty<CourseViewModel>();

            var result = courses.Select(course => new CourseViewModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Type = course.Type.ToString(),
                NType = course.NType.ToString(),
                NQFLevel = course.NQFLevel.ToString(),
                Credit = course.Credit,
                IsActive = course.IsActive,
                Modules = includeModules
                    ? course.Module?.Select(m => new ModuleViewModel
                    {
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        NQFLevel = m.NQFLevel,
                        Credit = m.Credit,
                        IsActive = m.IsActive
                    }).ToList()
                    : new List<ModuleViewModel>()
            }).ToList();

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<IReadOnlyList<CourseViewModel>> GetAllCoursesAsync(bool includeModules = false, bool onlyActive = true, CancellationToken cancellationToken = default)
        {
            string cacheKey = CourseCachePrefix + "all"
                + (includeModules ? ":full" : ":lite")
                + (onlyActive ? ":active" : ":all");

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<CourseViewModel> cached))
                return cached;

            var courses = await _context.Courses.GetAllAsync(
                filter: onlyActive
                    ? (Expression<Func<Course, bool>>)(c => c.IsActive)
                    : null,
                includeProperties: includeModules
                    ? new[] { nameof(Course.Module) }
                    : Array.Empty<string>(), cancellationToken: cancellationToken);

            if (courses == null || !courses.Any())
                return Array.Empty<CourseViewModel>();

            var result = courses.Select(course => new CourseViewModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Type = course.Type.ToString(),
                NType = course.NType.ToString(),
                NQFLevel = course.NQFLevel.ToString(),
                Credit = course.Credit,
                IsActive = course.IsActive,
                Modules = includeModules
                    ? course.Module?.Select(m => new ModuleViewModel
                    {
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        NQFLevel = m.NQFLevel,
                        Credit = m.Credit,
                        IsActive = m.IsActive
                    }).ToList()
                    : new List<ModuleViewModel>()
            }).ToList();

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        /// <summary>
        /// Retrieves a course by its unique identifier, with an option to include its associated modules.
        /// </summary>
        /// <remarks>The result is cached for 10 minutes to improve performance. If the course is not
        /// found in the cache, it is retrieved  from the database. The <paramref name="includeModules"/> parameter
        /// determines whether the course's modules are included  in the result.</remarks>
        /// <param name="courseId">The unique identifier of the course to retrieve.</param>
        /// <param name="includeModules">A boolean value indicating whether to include the course's associated modules in the result. If <see
        /// langword="true"/>, the modules will be included; otherwise, they will be excluded.</param>
        /// <returns>A <see cref="CourseViewModel"/> representing the course with the specified identifier, or <see
        /// langword="null"/>  if no course with the given identifier is found.</returns>
        public async Task<CourseViewModel> GetCourseByIdAsync(Guid courseId, bool includeModules = true)
        {
            string cacheKey = CourseCachePrefix + courseId + (includeModules ? ":full" : ":lite");
            if (_cache.TryGetValue(cacheKey, out CourseViewModel cached))
                return cached;

            var course = await _context.Courses.GetAsync(c => c.CourseId == courseId,
                includeProperties: includeModules ? new[] { nameof(Course.Module) } : Array.Empty<string>());

            if (course == null)
                return null;

            var vm = new CourseViewModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Type = course.Type.ToString(),
                NType = course.NType.ToString(),
                NQFLevel = course.NQFLevel.ToString(),
                Credit = course.Credit,
                IsActive = course.IsActive,
                Modules = includeModules
                    ? course.Module?.Select(m => new ModuleViewModel
                    {
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        NQFLevel = m.NQFLevel,
                        Credit = m.Credit,
                        IsActive = m.IsActive
                    }).ToList()
                    : new List<ModuleViewModel>()
            };

            _cache.Set(cacheKey, vm, TimeSpan.FromMinutes(10));
            return vm;
        }

        /// <summary>
        /// Asynchronously retrieves the list of modules associated with a specified course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course whose modules are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="ModuleViewModel"/> objects representing the modules of the specified course.  If the course does not
        /// exist or has no associated modules, an empty list is returned.</returns>
        public async Task<IReadOnlyList<ModuleViewModel>> GetCourseModulesAsync(Guid courseId)
        {
            var course = await _context.Courses.GetAsync(c => c.CourseId == courseId, includeProperties: new[] { nameof(Course.Module) });
            if (course?.Module == null)
                return Array.Empty<ModuleViewModel>();

            return course.Module.Select(m => new ModuleViewModel
            {
                ModuleId = m.ModuleId,
                ModuleName = m.ModuleName,
                NQFLevel = m.NQFLevel,
                Credit = m.Credit,
                IsActive = m.IsActive
            }).ToList();
        }

        /// <summary>
        /// Asynchronously retrieves a list of the most popular active courses, ordered by their credit value and name.
        /// </summary>
        /// <remarks>Courses are considered "popular" based on their credit value, with higher credit
        /// courses appearing first.  If multiple courses have the same credit value, they are further ordered
        /// alphabetically by their name. Only active courses are included in the result.</remarks>
        /// <param name="top">The maximum number of courses to return. Defaults to 12. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="CourseModuleViewModel"/> objects representing the most popular courses.</returns>
        public async Task<IReadOnlyList<CourseModuleViewModel>> GetPopularCoursesAsync(int top = 12)
        {
            var all = await _context.Courses.GetAllAsync(includeProperties: new[] { nameof(Course.Module) });
            return all
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Credit)
                .ThenBy(c => c.CourseName)
                .Take(top)
                .Select(c => ToCourseModuleVm(c))
                .ToList();
        }

        /// <summary>
        /// Retrieves a list of course modules filtered by course type and activity status.
        /// </summary>
        /// <remarks>The method retrieves all courses from the data source, filters them based on the
        /// specified course type and activity status,  and maps them to their corresponding view models. The result is
        /// returned as a read-only list.</remarks>
        /// <param name="type">The type of courses to retrieve. This parameter determines the category of courses to include in the result.</param>
        /// <param name="onlyActive">A boolean value indicating whether to include only active courses.  If <see langword="true"/>, only courses
        /// marked as active are included; otherwise, all courses of the specified type are included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="CourseModuleViewModel"/> objects representing the filtered course modules.</returns>
        public async Task<IReadOnlyList<CourseModuleViewModel>> GetCoursesByTypeAsync(eCourseType type, bool onlyActive = true)
        {
            var all = await _context.Courses.GetAllAsync(includeProperties: new[] { nameof(Course.Module) });
            return all
                .Where(c => c.Type == type && (!onlyActive || c.IsActive))
                .Select(c => ToCourseModuleVm(c))
                .ToList();
        }

        /// <summary>
        /// Asynchronously retrieves a distinct, alphabetically ordered list of course categories.
        /// </summary>
        /// <remarks>Categories are determined based on the <c>Type</c> property of each course. If a
        /// course does not have a  valid category, it is categorized as "Uncategorized". The result is cached for 30
        /// minutes to improve performance.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of distinct 
        /// course categories, sorted alphabetically. If no categories are found, the list will be empty.</returns>
        public async Task<IReadOnlyList<string>> GetDistinctCategoriesAsync()
        {
            if (_cache.TryGetValue(CategoryCacheKey, out IReadOnlyList<string> categoriesCached))
                return categoriesCached;

            var all = await _context.Courses.GetAllAsync();
            var categories = all
                .Select(c => c.Type.ToString() ?? "Uncategorized")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            _cache.Set(CategoryCacheKey, categories, TimeSpan.FromMinutes(30));
            return categories;
        }

        /// <summary>
        /// Retrieves a distinct, ordered list of NQF (National Qualifications Framework) levels from the available
        /// courses.
        /// </summary>
        /// <remarks>The method first attempts to retrieve the NQF levels from a cache. If the cache is
        /// empty, it queries the data source for all courses, extracts the distinct NQF levels, and orders them in
        /// ascending order. The result is then cached for subsequent calls.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of distinct NQF
        /// levels as strings, ordered in ascending order.</returns>
        public async Task<IReadOnlyList<string>> GetDistinctNqfLevelsAsync()
        {
            if (_cache.TryGetValue(NqfCacheKey, out IReadOnlyList<string> nqfCached))
                return nqfCached;

            var all = await _context.Courses.GetAllAsync();
            var levels = all
                .Where(c => c.NQFLevel.HasValue)
                .Select(c => c.NQFLevel.Value.ToString())
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            _cache.Set(NqfCacheKey, levels, TimeSpan.FromMinutes(30));
            return levels;
        }

        /// <summary>
        /// Asynchronously retrieves a list of available cycles.
        /// </summary>
        /// <param name="courseId">An optional identifier for the course. If provided, the method filters the cycles to those associated with
        /// the specified course. If <see langword="null"/>, all available cycles are retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of strings
        /// representing the available cycles.</returns>
        public async Task<IReadOnlyList<string>> GetAvailableCyclesAsync(Guid? courseId = null)
        {
            return new List<string>(); 
        }

        /// <summary>
        /// Searches for courses based on the specified criteria and returns a paginated result set.
        /// </summary>
        /// <remarks>The results are ordered alphabetically by course name. If no filters are applied, all
        /// courses are included in the result set.</remarks>
        /// <param name="q">An optional search term to filter courses by name. The search is case-insensitive and matches partial names.</param>
        /// <param name="type">An optional course type to filter the results. If specified, only courses of the given type are included.</param>
        /// <param name="category">An optional category to filter courses. If specified, only courses within the given category are included.</param>
        /// <param name="isFunded">An optional flag indicating whether to filter courses based on funding status. If <see langword="true"/>,
        /// only funded courses are included; if <see langword="false"/>, only non-funded courses are included.</param>
        /// <param name="nqfMin">An optional minimum NQF (National Qualifications Framework) level to filter courses. Only courses with an
        /// NQF level greater than or equal to this value are included.</param>
        /// <param name="nqfMax">An optional maximum NQF level to filter courses. Only courses with an NQF level less than or equal to this
        /// value are included.</param>
        /// <param name="page">The page number of the results to retrieve. Must be 1 or greater. Defaults to 1 if a value less than 1 is
        /// provided.</param>
        /// <param name="pageSize">The number of results to include per page. Must be 1 or greater. Defaults to 20 if a value less than 1 is
        /// provided.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing a collection of <see cref="CourseSearchResult"/> objects that
        /// match the search criteria, along with pagination metadata such as the total count of results.</returns>
        public async Task<PagedResult<CourseSearchResult>> SearchCoursesAsync(string? q,eCourseType? type,string? category,bool? isFunded,eNQF? nqfMin,eNQF? nqfMax,int page,int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var all = await _context.Courses.GetAllAsync(includeProperties: new[] { nameof(Course.Module) });

            IEnumerable<Course> query = all;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLowerInvariant();
                query = query.Where(c => c.CourseName.ToLower().Contains(term));
            }

            if (type.HasValue)
                query = query.Where(c => c.Type == type.Value);

            if (nqfMin.HasValue)
                query = query.Where(c => c.NQFLevel.HasValue && c.NQFLevel.Value >= nqfMin.Value);

            if (nqfMax.HasValue)
                query = query.Where(c => c.NQFLevel.HasValue && c.NQFLevel.Value <= nqfMax.Value);

            var total = query.Count();

            var items = query
                .OrderBy(c => c.CourseName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseSearchResult
                {
                    CourseId = c.CourseId,
                    Name = c.CourseName,
                    Type = c.Type,
                    NQFLevel = c.NQFLevel,
                    Credit = c.Credit,
                    ModuleCount = c.Module?.Count(m => m.IsActive) ?? 0
                })
                .ToList();

            return new PagedResult<CourseSearchResult>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        /// <summary>
        /// Retrieves a list of recommended courses based on the specified course.
        /// </summary>
        /// <remarks>Recommended courses are determined based on their similarity to the specified course,
        /// prioritizing  courses with the same NQF level, type, and higher credit values. Only active courses are
        /// considered  for recommendations.</remarks>
        /// <param name="courseId">The unique identifier of the course for which recommendations are requested.</param>
        /// <param name="take">The maximum number of recommended courses to return. The default value is 6.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="CourseSearchResult"/> objects representing the recommended courses. If no recommendations  are found,
        /// an empty list is returned.</returns>
        public async Task<IReadOnlyList<CourseSearchResult>> GetRecommendedCoursesAsync(Guid courseId, int take = 6)
        {
            var origin = await _context.Courses.GetAsync(c => c.CourseId == courseId);
            if (origin == null)
                return Array.Empty<CourseSearchResult>();

            var all = await _context.Courses.GetAllAsync(includeProperties: new[] { nameof(Course.Module) });
            var candidates = all
                .Where(c => c.CourseId != courseId && c.IsActive)
                .OrderByDescending(c => c.NQFLevel == origin.NQFLevel) // prioritize same NQF
                .ThenByDescending(c => c.Type == origin.Type)
                .ThenByDescending(c => c.Credit)
                .Take(take)
                .Select(c => new CourseSearchResult
                {
                    CourseId = c.CourseId,
                    Name = c.CourseName,
                    Type = c.Type,
                    NQFLevel = c.NQFLevel,
                    Credit = c.Credit,
                    ModuleCount = c.Module?.Count(m => m.IsActive) ?? 0
                })
                .ToList();

            return candidates;
        }

        /// <summary>
        /// Retrieves statistical information about a specific course, including total credits, active modules, and NQF
        /// level.
        /// </summary>
        /// <remarks>The method calculates the total credits by summing the credit values of all modules
        /// in the course that have a defined credit value. It also counts the number of active modules and includes the
        /// course's NQF level in the result.</remarks>
        /// <param name="courseId">The unique identifier of the course to retrieve statistics for.</param>
        /// <returns>A <see cref="CourseStatsResult"/> object containing the course statistics, or <see langword="null"/> if the
        /// course does not exist.</returns>
        public async Task<CourseStatsResult> GetCourseStatsAsync(Guid courseId)
        {
            var course = await _context.Courses.GetAsync(c => c.CourseId == courseId, includeProperties: new[] { nameof(Course.Module) });
            if (course == null) return null;

            double? totalCredits = course.Module?.Where(m => m.Credit.HasValue).Sum(m => m.Credit.Value);

            return new CourseStatsResult
            {
                CourseId = course.CourseId,
                TotalCredits = totalCredits,
                ActiveModules = course.Module?.Count(m => m.IsActive) ?? 0,
                NqfLevel = course.NQFLevel,
                LastUpdatedUtc = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Retrieves a paginated list of courses that are related to the specified course.
        /// </summary>
        /// <remarks>Related courses are determined based on the type of the specified course and are
        /// filtered to include only active courses. Results are ordered by their NQF level (descending) and then by
        /// course name (ascending).</remarks>
        /// <param name="courseId">The unique identifier of the course for which related courses are being retrieved.</param>
        /// <param name="page">The page number of the results to retrieve. Must be 1 or greater.</param>
        /// <param name="pageSize">The number of items to include per page. Must be 1 or greater.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing a collection of <see cref="CourseSearchResult"/> objects that
        /// represent the related courses. If the specified course does not exist, the result will contain an empty
        /// collection with a total count of 0.</returns>
        public async Task<PagedResult<CourseSearchResult>> GetRelatedCoursesAsync(Guid courseId, int page, int pageSize)
        {
            var origin = await _context.Courses.GetAsync(c => c.CourseId == courseId);
            if (origin == null)
            {
                return new PagedResult<CourseSearchResult>
                {
                    Items = Array.Empty<CourseSearchResult>(),
                    Page = 1,
                    PageSize = pageSize,
                    TotalCount = 0
                };
            }

            var all = await _context.Courses.GetAllAsync(includeProperties: new[] { nameof(Course.Module) });

            var related = all
                .Where(c => c.CourseId != courseId && c.IsActive && c.Type == origin.Type)
                .OrderByDescending(c => c.NQFLevel == origin.NQFLevel)
                .ThenBy(c => c.CourseName);

            var total = related.Count();
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var items = related
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseSearchResult
                {
                    CourseId = c.CourseId,
                    Name = c.CourseName,
                    Type = c.Type,
                    NQFLevel = c.NQFLevel,
                    Credit = c.Credit,
                    ModuleCount = c.Module?.Count(m => m.IsActive) ?? 0
                })
                .ToList();

            return new PagedResult<CourseSearchResult>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        /// <summary>
        /// Retrieves a paginated list of courses, optionally filtering by active status.
        /// </summary>
        /// <remarks>The courses are sorted by their name in ascending order. If the specified page
        /// exceeds the total number of pages,  the result will contain an empty collection.</remarks>
        /// <param name="page">The page number to retrieve. Must be 1 or greater. If less than 1, defaults to 1.</param>
        /// <param name="pageSize">The number of items per page. Must be 1 or greater. If less than 1, defaults to 20.</param>
        /// <param name="onlyActive">A value indicating whether to include only active courses.  <see langword="true"/> to include only active
        /// courses; otherwise, <see langword="false"/> to include all courses.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing a collection of <see cref="CourseSearchResult"/> objects for the
        /// specified page,  along with pagination metadata such as the total count of courses.</returns>
        public async Task<PagedResult<CourseSearchResult>> GetPagedCoursesAsync(int page, int pageSize, bool onlyActive = true)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var all = await _context.Courses.GetAllAsync(includeProperties: new[] { nameof(Course.Module) });
            var filtered = all.Where(c => !onlyActive || c.IsActive);

            var total = filtered.Count();

            var items = filtered
                .OrderBy(c => c.CourseName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseSearchResult
                {
                    CourseId = c.CourseId,
                    Name = c.CourseName,
                    Type = c.Type,
                    NQFLevel = c.NQFLevel,
                    Credit = c.Credit,
                    ModuleCount = c.Module?.Count(m => m.IsActive) ?? 0
                })
                .ToList();

            return new PagedResult<CourseSearchResult>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        /// <summary>
        /// Asynchronously retrieves a mapping of course IDs to their corresponding credit values.
        /// </summary>
        /// <remarks>This method filters the courses based on the provided IDs and returns only those that
        /// match. The returned dictionary is case-insensitive to duplicate course IDs in the input
        /// collection.</remarks>
        /// <param name="courseIds">A collection of course IDs for which to retrieve credit values. Duplicate IDs are ignored. If the collection
        /// is <see langword="null"/> or empty, an empty dictionary is returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a read-only dictionary where the keys
        /// are course IDs and the values are the corresponding credit values. If no matching courses are found, the
        /// dictionary will be empty.</returns>
        public async Task<IReadOnlyDictionary<Guid, double>> GetCourseCreditMapAsync(IEnumerable<Guid> courseIds)
        {
            var ids = courseIds?.Distinct().ToHashSet() ?? new HashSet<Guid>();
            if (!ids.Any())
                return new Dictionary<Guid, double>();

            var all = await _context.Courses.GetAllAsync();
            return all
                .Where(c => ids.Contains(c.CourseId))
                .ToDictionary(c => c.CourseId, c => (double)(c.Credit));
        }

        /// <summary>
        /// Invalidates the cache entries for the specified course.
        /// </summary>
        /// <remarks>This method removes all cache entries associated with the specified course, including
        /// both full and lite versions of the course data. Call this method to ensure that stale or outdated course
        /// data is no longer served from the cache.</remarks>
        /// <param name="courseId">The unique identifier of the course whose cache entries should be invalidated.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task InvalidateCourseCacheAsync(Guid courseId)
        {
            _cache.Remove(CourseCachePrefix + courseId + ":full");
            _cache.Remove(CourseCachePrefix + courseId + ":lite");
            return Task.CompletedTask;
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Converts a <see cref="Course"/> object to a <see cref="CourseModuleViewModel"/>.
        /// </summary>
        /// <param name="c">The <see cref="Course"/> object to convert. Must not be <c>null</c>.</param>
        /// <returns>A <see cref="CourseModuleViewModel"/> representing the provided <see cref="Course"/>.</returns>
        private CourseModuleViewModel ToCourseModuleVm(Course c)
        {
            return new CourseModuleViewModel
            {
                CourseId = c.CourseId,
                Name = c.CourseName,
                Type = c.Type,
                NType = c.NType,
                NQFLevel = c.NQFLevel,
                Credit = c.Credit,
                IsActive = c.IsActive,
                ModuleCount = c.Module?.Count(m => m.IsActive) ?? 0
            };
        }

        #endregion

    }
}
