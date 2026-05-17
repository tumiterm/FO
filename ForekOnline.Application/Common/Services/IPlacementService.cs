// <copyright file="IPlacementService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

#region Usings
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines operations for managing learner placements.
    /// </summary>
    public interface IPlacementService
    {
        /// <summary>
        /// Retrieves all active placements, projected into <see cref="PlacementViewModel"/>.
        /// </summary>
        Task<IReadOnlyList<PlacementViewModel>> GetActivePlacementsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns <see langword="true"/> when the student is already placed at the given company.
        /// </summary>
        Task<bool> IsStudentPlacedAsync(Guid companyId, string studentFullName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists a new placement record.
        /// </summary>
        /// <returns>The saved <see cref="Placement"/>, or <see langword="null"/> on failure.</returns>
        Task<Placement?> CreatePlacementAsync(Placement placement, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing placement record.
        /// </summary>
        Task<Placement?> UpdatePlacementAsync(Placement placement, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a single placement by its identifier.
        /// </summary>
        Task<Placement?> GetPlacementByIdAsync(Guid placementId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the placement details for a specific student by name.
        /// </summary>
        Task<Placement?> GetPlacementByStudentNameAsync(string studentFullName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the display name of the company.
        /// </summary>
        Task<string> GetCompanyNameAsync(Guid companyId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Builds the <see cref="SelectList"/> of facilitators and admins.
        /// </summary>
        Task<SelectList> GetUserSelectListAsync(Guid? selectedValue = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Builds the <see cref="SelectList"/> of companies.
        /// </summary>
        Task<SelectList> GetCompanySelectListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Builds the <see cref="SelectList"/> of courses.
        /// </summary>
        Task<SelectList> GetCourseSelectListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Builds the <see cref="SelectList"/> of modules belonging to the given course.
        /// </summary>
        Task<SelectList> GetModuleSelectListAsync(Guid courseId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a status-count dictionary used for dashboard widgets.
        /// </summary>
        IDictionary<object, int> GetStatusCounts(IReadOnlyList<PlacementViewModel> placements);
    }
}