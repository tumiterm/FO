// <copyright file="ICourseService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    27/10/2025 20:06:27 PM
// Purpose:         Defines the ICourseService service interface for managing courses.

#region Usings
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines a contract for managing and retrieving course-related data and operations.
    /// </summary>
    /// <remarks>The <c>ICourseService</c> interface provides methods for adding, updating, and managing
    /// courses, as well as retrieving course information and performing analytics. It supports operations such as
    /// toggling course activity, recalculating course credits, and searching for courses based on various criteria. The
    /// interface also includes methods for cache invalidation and retrieving distinct course attributes like categories
    /// and NQF levels.</remarks>
    public interface ICourseService
    {
        #region Commands

        /// <summary>
        /// Asynchronously adds a new course based on the provided course creation request.
        /// </summary>
        /// <param name="request">The request containing the details of the course to be created. Cannot be null.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a <see cref="ValidationResponse"/>
        /// indicating the success or failure of the operation.</returns>
        Task<ValidationResponse> AddCourseAsync(CourseCreateRequest request);

        /// <summary>
        /// Updates the details of an existing course asynchronously.
        /// </summary>
        /// <param name="request">The request containing the updated course information. Cannot be null.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the success or failure of the update operation.</returns>
        Task<ValidationResponse> UpdateCourseAsync(CourseUpdateRequest request);

        /// <summary>
        /// Asynchronously updates or inserts modules for a specified course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course to which the modules belong.</param>
        /// <param name="modules">A collection of <see cref="ModuleUpsertRequest"/> objects representing the modules to be updated or
        /// inserted. Each module must have a valid identifier and associated data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="ValidationResponse"/> indicating the success or failure of the operation, including any validation
        /// errors encountered.</returns>
        Task<ValidationResponse> UpsertModulesAsync(Guid courseId, IEnumerable<ModuleUpsertRequest> modules);

        /// <summary>
        /// Toggles the active status of a course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course to update.</param>
        /// <param name="isActive">A value indicating whether the course should be set to active.          <see langword="true"/> to activate
        /// the course; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the operation,          including any validation
        /// errors encountered.</returns>
        Task<ValidationResponse> ToggleCourseActiveAsync(Guid courseId, bool isActive);

        /// <summary>
        /// Marks a course as deleted without permanently removing it from the database.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course to be soft deleted.</param>
        /// <param name="reason">The reason for the soft deletion, which is recorded for auditing purposes.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the success or failure of the operation, including any
        /// validation errors encountered.</returns>
        Task<ValidationResponse> SoftDeleteCourseAsync(Guid courseId, string reason);

        /// <summary>
        /// Recalculates the credit for a specified course asynchronously.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course for which the credit is to be recalculated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="ValidationResponse"/> indicating the success or failure of the recalculation process.</returns>
        Task<ValidationResponse> RecalculateCourseCreditAsync(Guid courseId);
        #endregion
        #region Queries

        Task<IReadOnlyList<CourseViewModel>> GetAllCoursesAsync(bool includeModules = false, bool onlyActive = true, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CourseViewModel>> GetAllCoursesAsync(bool includeModules = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves a course by its unique identifier.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course to retrieve.</param>
        /// <param name="includeModules">A boolean value indicating whether to include the course modules in the result. <see langword="true"/> to
        /// include modules; otherwise, <see langword="false"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CourseViewModel"/>
        /// representing the course with the specified identifier. Returns <see langword="null"/> if the course is not
        /// found.</returns>
        Task<CourseViewModel> GetCourseByIdAsync(Guid courseId, bool includeModules = true);

        /// <summary>
        /// Asynchronously retrieves a list of modules for a specified course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course for which to retrieve modules.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of <see
        /// cref="ModuleViewModel"/> objects representing the course modules.</returns>
        Task<IReadOnlyList<ModuleViewModel>> GetCourseModulesAsync(Guid courseId);

        /// <summary>
        /// Asynchronously retrieves a list of the most popular course modules.
        /// </summary>
        /// <remarks>The method returns the top popular course modules based on predefined criteria. The
        /// default value for <paramref name="top"/> is 12.</remarks>
        /// <param name="top">The maximum number of popular course modules to retrieve. Must be a positive integer.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of <see
        /// cref="CourseModuleViewModel"/> representing the popular course modules.</returns>
        Task<IReadOnlyList<CourseModuleViewModel>> GetPopularCoursesAsync(int top = 12);

        /// <summary>
        /// Asynchronously retrieves a list of course modules filtered by the specified course type.
        /// </summary>
        /// <param name="type">The type of courses to retrieve. This parameter determines the category of courses to be included in the
        /// result.</param>
        /// <param name="onlyActive">A boolean value indicating whether to include only active courses. If <see langword="true"/>, only active
        /// courses are returned; otherwise, all courses of the specified type are included.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of <see
        /// cref="CourseModuleViewModel"/> objects that match the specified criteria.</returns>
        Task<IReadOnlyList<CourseModuleViewModel>> GetCoursesByTypeAsync(eCourseType type, bool onlyActive = true);

        /// <summary>
        /// Asynchronously retrieves a list of distinct category names.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of distinct
        /// category names.</returns>
        Task<IReadOnlyList<string>> GetDistinctCategoriesAsync();

        /// <summary>
        /// Asynchronously retrieves a list of distinct NQF levels.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of strings, each
        /// representing a unique NQF level.</returns>
        Task<IReadOnlyList<string>> GetDistinctNqfLevelsAsync();

        /// <summary>
        /// Asynchronously retrieves a list of available cycle names.
        /// </summary>
        /// <param name="courseId">An optional identifier for the course. If specified, the method returns cycles associated with the given
        /// course; otherwise, it returns all available cycles.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of strings, each
        /// representing the name of an available cycle.</returns>
        Task<IReadOnlyList<string>> GetAvailableCyclesAsync(Guid? courseId = null);
        #endregion
        #region Search

        /// <summary>
        /// Asynchronously searches for courses based on specified criteria and returns a paged result.
        /// </summary>
        /// <param name="q">The search query string. Can be null or empty to match all courses.</param>
        /// <param name="type">The type of course to filter by. Can be null to include all types.</param>
        /// <param name="category">The category to filter courses by. Can be null to include all categories.</param>
        /// <param name="isFunded">A boolean indicating whether to filter for funded courses. Can be null to include both funded and non-funded
        /// courses.</param>
        /// <param name="nqfMin">The minimum NQF level to filter courses by. Can be null to include all levels.</param>
        /// <param name="nqfMax">The maximum NQF level to filter courses by. Can be null to include all levels.</param>
        /// <param name="page">The page number of the results to retrieve. Must be a positive integer.</param>
        /// <param name="pageSize">The number of results per page. Must be a positive integer.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a <see cref="PagedResult{T}"/> of
        /// <see cref="CourseSearchResult"/> objects matching the search criteria.</returns>
        Task<PagedResult<CourseSearchResult>> SearchCoursesAsync(string? q,eCourseType? type,string? category,bool? isFunded,eNQF? nqfMin,eNQF? nqfMax,int page,int pageSize);
        #endregion
        #region Analytics

        /// <summary>
        /// Retrieves a list of recommended courses based on a specified course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course for which recommendations are requested.</param>
        /// <param name="take">The maximum number of recommended courses to return. The default value is 6.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of recommended
        /// course search results.</returns>
        Task<IReadOnlyList<CourseSearchResult>> GetRecommendedCoursesAsync(Guid courseId, int take = 6);

        /// <summary>
        /// Asynchronously retrieves statistical data for a specified course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course for which to retrieve statistics.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a <see cref="CourseStatsResult"/>
        /// object with the course statistics.</returns>
        Task<CourseStatsResult> GetCourseStatsAsync(Guid courseId);

        /// <summary>
        /// Asynchronously retrieves a paged list of courses related to the specified course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course for which related courses are being retrieved.</param>
        /// <param name="page">The zero-based page index of the results to retrieve.</param>
        /// <param name="pageSize">The number of courses to include in each page of results. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagedResult{T}"/>
        /// of <see cref="CourseSearchResult"/> representing the related courses.</returns>
        Task<PagedResult<CourseSearchResult>> GetRelatedCoursesAsync(Guid courseId, int page, int pageSize);
        #endregion
        #region Listings

        /// <summary>
        /// Retrieves a paged list of courses based on the specified criteria.
        /// </summary>
        /// <param name="page">The zero-based page index to retrieve. Must be non-negative.</param>
        /// <param name="pageSize">The number of courses to include in each page. Must be greater than zero.</param>
        /// <param name="onlyActive">If <see langword="true"/>, only active courses are included; otherwise, all courses are included.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing a list of <see cref="CourseSearchResult"/> objects for the
        /// specified page.</returns>
        Task<PagedResult<CourseSearchResult>> GetPagedCoursesAsync(int page, int pageSize, bool onlyActive = true);

        /// <summary>
        /// Asynchronously retrieves a mapping of course IDs to their respective credit values.
        /// </summary>
        /// <param name="courseIds">A collection of course IDs for which to retrieve credit values. Cannot be null.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a read-only dictionary mapping each
        /// course ID to its corresponding credit value. If a course ID does not exist, it will not be included in the
        /// dictionary.</returns>
        Task<IReadOnlyDictionary<Guid, double>> GetCourseCreditMapAsync(IEnumerable<Guid> courseIds);
        #endregion
        #region Cache

        /// <summary>
        /// Invalidates the cache for a specific course, identified by its unique identifier.
        /// </summary>
        /// <remarks>This method should be called whenever the course data is updated to ensure that the
        /// cache reflects the latest information.</remarks>
        /// <param name="courseId">The unique identifier of the course whose cache should be invalidated.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task InvalidateCourseCacheAsync(Guid courseId);
        #endregion
    }
}
