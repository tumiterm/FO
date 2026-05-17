// <copyright file="IStudentService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/03/2025 20:27 PM
// Purpose:         Defines the IStudentService interface

#region Usings
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides methods for retrieving and managing student information and related data.
    /// </summary>
    /// <remarks>This interface defines a set of asynchronous methods for accessing student details, including
    /// retrieving individual student information, lists of students, and related statistics. It supports operations
    /// such as filtering students by various criteria, fetching course-related data, and prepopulating cache for
    /// performance optimization.</remarks>
    public interface IStudentService
    {
        /// <summary>
        /// Retrieves a student's details using their student number.
        /// </summary>
        /// <param name="studentNumber">Unique identifier for the student.</param>
        /// <returns>Returns a <see cref="Student"/> object if found; otherwise, null.</returns>
        Task<Student> GetStudentAsync(string studentNumber);

        /// <summary>
        /// Retrieves a student by their email address.
        /// </summary>
        /// <remarks>
        /// The lookup is case-insensitive. If multiple students share the same email address,
        /// active students are prioritised, then the most recently admitted is returned.
        /// Results are cached with a sliding expiration to reduce repeated list scans.
        /// </remarks>
        /// <param name="email">
        /// The email address to search for. Cannot be null, empty, or whitespace.
        /// </param>
        /// <returns>
        /// The matching <see cref="Student"/> if found; otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="email"/> is null, empty, whitespace, or not a valid email format.
        /// </exception>
        Task<Student?> GetStudentByEmailAsync(string email);

        /// <summary>
        /// Retrieves a student's details using their student number and optional specified properties.
        /// </summary>
        /// <param name="studentNumber">Unique identifier for the student.</param>
        /// <param name="studentProperties">
        /// A colon-delimited string specifying which properties of the student to retrieve (e.g., "FullName:LastName:IDNumber").
        /// If null or empty, all student properties will be fetched.
        /// </param>
        /// <returns>Returns a dictionary containing the requested student properties and their values.</returns>
        Task<Dictionary<string, object>> GetStudentWithPropertiesAsync(string studentNumber, string studentProperties = null);

        /// <summary>
        /// Gets a list of students asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of students.</returns>
        Task<List<Student>> GetStudentListAsync();

        /// <summary>
        /// Retrieve a list of students based on dynamic criteria (e.g., name, enrollment status, course) instead of just fetching all students.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<Student>> GetStudentsByFilterAsync(Dictionary<string, string> filters = null, int page = 1, int pageSize = 10);

        /// <summary>
        /// Retrieve a lightweight summary of a student (e.g., ID, name, enrollment status) instead of the full object, useful for dashboards or lists.
        /// </summary>
        /// <param name="studentNumber"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>> GetStudentSummaryAsync(string studentNumber);

        /// <summary>
        /// Retrieve a student along with specific related data (e.g., EnrollmentHistory, Courses, Grades) in one call, API NOT SUPPORTING THIS CURRENTLY.
        /// </summary>
        /// <param name="studentNumber"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<StudentRelatedDataViewModel> GetStudentRelatedDataAsync(string studentNumber, string[] include = null);

        /// <summary>
        /// Provide aggregated statistics about a student (e.g., number of enrollments, average grade), useful for reporting.
        /// </summary>
        /// <param name="studentNumber"></param>
        /// <returns>Returns a dictionary containing the requested student properties and their values.</returns>
        Task<Dictionary<string, object>> GetStudentStatisticsAsync(string studentNumber);

        /// <summary>
        /// Retrieve all students enrolled in a specific course by passing a courseId
        /// </summary>
        /// <param name="courseId">Takes a courseId parameter to identify the course</param>
        /// <param name="onlyActive">Optionally takes a onlyActive parameter to filter for active enrollments.</param>
        /// <returns>Returns a list of students enrolled in the specified course.</returns>
        Task<List<Student>> GetStudentsByCourseAsync(Guid courseId, bool onlyActive = true);

        /// <summary>
        /// Retrieves course statistics, including the total number of enrolled students and those who have dropped out.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <returns>A dictionary containing course statistics, such as total enrolled students and dropped-out students.</returns>
        /// <exception cref="ArgumentException">Thrown when the courseId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the API request fails or returns invalid data.</exception>
        Task<Dictionary<string, int>> GetCourseStatisticsAsync(Guid courseId);

        /// <summary>
        /// Asynchronously prepopulates the cache with student list data.
        /// </summary>
        /// <returns>A task representing the asynchronous cache prepopulation operation.</returns>
        Task PrepopulateCacheAsync();

        /// <summary>
        /// Retrieves a dummy student details using their dummy student number.
        /// </summary>
        /// <param name="studentNumber">Unique identifier for the student.</param>
        /// <returns>Returns a <see cref="Student"/> object if found; otherwise, null.</returns>
        Task<Student> GetDummyStudentAsync(string studentNumber);

        /// <summary>
        /// Asynchronously retrieves a list of students who are currently online in the Forek system.
        /// </summary>
        /// <param name="apiStudents">An optional collection of <see cref="Student"/> objects to filter the online students. If provided, only
        /// students present in this collection will be considered.</param>
        /// <param name="idOrPassportNumbers">An optional collection of student identifiers or passport numbers to filter the online students. If
        /// provided, only students with matching identifiers will be considered.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with a read-only list of
        /// online <see cref="Student"/> objects and the total count of these students.</returns>
        Task<(IReadOnlyList<Student> Students, int Count)> GetForekOnlineStudentsAsync(IEnumerable<Student> apiStudents = null, IEnumerable<string> idOrPassportNumbers = null);

        /// <summary>
        /// Performs an asynchronous search for students based on course title, course type, year, and pagination
        /// parameters.
        /// </summary>
        /// <param name="courseTitle">The title of the course to filter students by. Specify null to ignore this filter.</param>
        /// <param name="courseType">The type of the course to filter students by. Specify null to ignore this filter.</param>
        /// <param name="year">The academic year to filter students by. Specify null to ignore this filter.</param>
        /// <param name="page">The page number of results to retrieve. Must be greater than or equal to 1.</param>
        /// <param name="pageSize">The maximum number of results to return per page. Must be greater than or equal to 1.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of student
        /// search results matching the specified criteria. The list may be empty if no students are found.</returns>
        Task<IReadOnlyList<AdvancedStudentSearchResultViewModel>> AdvancedStudentSearchAsync(string? courseTitle, string? courseType, int? year, int page = 1, int pageSize = 20);

        /// <summary>
        /// Asynchronously retrieves placement details for a student, including print information if available.
        /// </summary>
        /// <param name="studentNumber">The unique identifier for the student whose placement details are to be retrieved. Cannot be null or empty.</param>
        /// <param name="includePlacement">Specifies whether to include placement information in the result. Set to <see langword="true"/> to include
        /// placement details; otherwise, only print information is returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="StudentPrintDetailsViewModel"/> with the student's placement and print details, or <see
        /// langword="null"/> if no details are found for the specified student.</returns>
        Task<StudentPrintDetailsViewModel?> GetStudentPlacementDetailsAsync(string studentNumber, bool includePlacement = true);
    }
}
