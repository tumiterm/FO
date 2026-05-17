// <copyright file="PlacementService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <inheritdoc cref="IPlacementService"/>
    public sealed class PlacementService : IPlacementService
    {
        #region Private Variables
        private readonly IUnitOfWork _context;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="PlacementService"/>.
        /// </summary>
        public PlacementService(IUnitOfWork context, IUserService userService, ICourseService courseService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<PlacementViewModel>> GetActivePlacementsAsync(CancellationToken cancellationToken = default)
        {
            var placements = await _context.Placement.GetAllAsync(
                filter: p => p.IsActive == true, includeProperties: new[] { "Company"},
                cancellationToken: cancellationToken);

            var result = new List<PlacementViewModel>(placements.Count);

            foreach (var item in placements)
            {
               // string companyName = await GetCompanyNameAsync(item.CompanyId, cancellationToken);
                string placedByName = await ResolveUserNameAsync(item.PlacedBy, cancellationToken);

                result.Add(new PlacementViewModel
                {
                    PlacementId = item.PlacementId,
                    Student = item.Student,
                    CompanyId = StringExtensions.TruncateWithEllipsisSmart(item.Company?.CompanyName),
                    PlacedBy = placedByName,
                    StartDate = FormatDate(item.StartDate),
                    EndDate = FormatDate(item.EndDate),
                    Status = item.Status,
                    IsActive = item.IsActive,
                });
            }

            return result;
        }

        /// <inheritdoc/>
        public Task<bool> IsStudentPlacedAsync(Guid companyId, string studentFullName, CancellationToken cancellationToken = default)
            => _context.Placement.ExistsAsync(
                m => m.CompanyId == companyId && m.Student.Equals(studentFullName),
                cancellationToken);

        /// <inheritdoc/>
        public async Task<Placement?> CreatePlacementAsync(Placement placement, CancellationToken cancellationToken = default)
        {
            var added = await _context.Placement.AddAsync(placement, cancellationToken);
            if (added is null) return null;

            int saved = await _context.SaveAsync();
            return saved > 0 ? added : null;
        }

        /// <inheritdoc/>
        public async Task<Placement?> UpdatePlacementAsync(Placement placement, CancellationToken cancellationToken = default)
            => await _context.Placement.UpdatePlacementAsync(placement);

        /// <inheritdoc/>
        public Task<Placement?> GetPlacementByIdAsync(Guid placementId, CancellationToken cancellationToken = default)
            => _context.Placement.GetAsync(
                filter: p => p.PlacementId == placementId,
                cancellationToken: cancellationToken);

        /// <inheritdoc/>
        public async Task<Placement?> GetPlacementByStudentNameAsync(string studentFullName, CancellationToken cancellationToken = default)
        {
            var list = await _context.Placement.GetAllAsync(
                filter: p => p.Student.Equals(studentFullName),
                cancellationToken: cancellationToken);

            return list.FirstOrDefault();
        }

        /// <inheritdoc/>
        public async Task<string> GetCompanyNameAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var company = await _context.Company.GetAsync(
                filter: c => c.CompanyId == companyId,
                cancellationToken: cancellationToken);

            return company?.CompanyName ?? string.Empty;
        }

        /// <inheritdoc/>
        public async Task<SelectList> GetUserSelectListAsync(Guid? selectedValue = null, CancellationToken cancellationToken = default)
        {
            var users = await _userService.GetUsersByRoleAsync(eSysRole.Facilitator, cancellationToken);
            var admins = await _userService.GetUsersByRoleAsync(eSysRole.Admin, cancellationToken);

            var combined = users.Concat(admins)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} {u.LastName}"
                });

            return new SelectList(combined, "Value", "Text", selectedValue?.ToString());
        }

        /// <inheritdoc/>
        public async Task<SelectList> GetCompanySelectListAsync(CancellationToken cancellationToken = default)
        {
            var companies = await _context.Company.GetAllAsync(cancellationToken: cancellationToken);

            var items = companies.Select(c => new SelectListItem
            {
                Value = c.CompanyId.ToString(),
                Text = c.CompanyName
            });

            return new SelectList(items, "Value", "Text");
        }

        /// <inheritdoc/>
        public async Task<SelectList> GetCourseSelectListAsync(CancellationToken cancellationToken = default)
        {
            //var courses = await _context.Courses.GetAllAsync(cancellationToken: cancellationToken);
            var courses = await _courseService.GetAllCoursesAsync(true, true, cancellationToken);

            var items = courses.Select(c => new SelectListItem
            {
                Value = c.CourseId.ToString(),
                Text = $"{c.CourseName} ({c.Type})"
            });

            return new SelectList(items, "Value", "Text");
        }

        /// <inheritdoc/>
        public async Task<SelectList> GetModuleSelectListAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            var modules = await _context.Modules.GetAllAsync(
                filter: m => m.CourseIdFK == courseId,
                cancellationToken: cancellationToken);

            var items = modules.Select(m => new SelectListItem
            {
                Value = m.ModuleId.ToString(),
                Text = m.ModuleName
            });

            return new SelectList(items, "Value", "Text");
        }

        /// <inheritdoc/>
        public IDictionary<object, int> GetStatusCounts(IReadOnlyList<PlacementViewModel> placements)
            => placements
                .GroupBy(p => (object)p.Status)
                .ToDictionary(g => g.Key, g => g.Count());

        /// <summary>
        /// Formats a nullable <see cref="DateTime"/> to "yyyy-MM-dd", returning an empty string when null.
        /// </summary>
        private static string FormatDate(DateTime? value)
            => value.HasValue ? value.Value.ToString("yyyy-MM-dd") : string.Empty;

        private async Task<string> ResolveUserNameAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserInfoAsync(userId);
                return user is null ? string.Empty : $"{user.Name} {user.LastName}";
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}