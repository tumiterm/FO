using System.Net.Http.Json;
using ForekOnline.Domain.ViewModels;

namespace ElecPOE.Services
{
    /// <summary>
    /// Retrieves and combines employee department and employee records from BaseAPI.
    /// </summary>
    public sealed class EmployeeDirectoryService : IEmployeeDirectoryService
    {
        private static readonly (string Icon, string Gradient)[] CardStyles =
        [
            ("fa-bolt", "linear-gradient(135deg, #FF6B35 0%, #D62828 100%)"),
            ("fa-building", "linear-gradient(135deg, #004E89 0%, #1A659E 100%)"),
            ("fa-fire-flame-curved", "linear-gradient(135deg, #8E0E14 0%, #FF6B35 100%)"),
            ("fa-seedling", "linear-gradient(135deg, #10B981 0%, #059669 100%)"),
            ("fa-shield-heart", "linear-gradient(135deg, #EF4444 0%, #DC2626 100%)"),
            ("fa-chalkboard-user", "linear-gradient(135deg, #8B5CF6 0%, #7C3AED 100%)"),
            ("fa-briefcase", "linear-gradient(135deg, #1F2937 0%, #111827 100%)"),
            ("fa-laptop-code", "linear-gradient(135deg, #3B82F6 0%, #2563EB 100%)")
        ];

        private readonly HttpClient _httpClient;
        private readonly ILogger<EmployeeDirectoryService> _logger;

        public EmployeeDirectoryService(HttpClient httpClient, ILogger<EmployeeDirectoryService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<EmployeeDirectoryViewModel> GetDirectoryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var departmentsTask = GetDataAsync<EmployeeDepartmentDto>(
                    "api/v1.0/employee-departments",
                    cancellationToken);
                var employeesTask = GetDataAsync<EmployeeDto>("api/v1.0/employees", cancellationToken);

                await Task.WhenAll(departmentsTask, employeesTask);

                var employees = await employeesTask;
                var departments = (await departmentsTask)
                    .OrderBy(department => department.Name)
                    .Select((department, index) => MapDepartment(department, employees, index))
                    .ToArray();

                return new EmployeeDirectoryViewModel { Departments = departments };
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Unable to retrieve the employee directory from BaseAPI.");

                return new EmployeeDirectoryViewModel
                {
                    ErrorMessage = "The employee directory is temporarily unavailable. Please refresh the page or try again later."
                };
            }
        }

        private async Task<IReadOnlyList<T>> GetDataAsync<T>(string requestUri, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<IReadOnlyList<T>>>(
                cancellationToken: cancellationToken);

            if (envelope is null)
            {
                throw new System.Text.Json.JsonException($"BaseAPI returned an empty response for '{requestUri}'.");
            }

            if (!envelope.Success)
            {
                throw new HttpRequestException(envelope.Message ?? $"BaseAPI request '{requestUri}' was unsuccessful.");
            }

            return envelope.Data ?? Array.Empty<T>();
        }

        private static EmployeeDepartmentSummaryViewModel MapDepartment(
            EmployeeDepartmentDto department,
            IReadOnlyList<EmployeeDto> employees,
            int index)
        {
            var staff = employees
                .Where(employee => employee.DepartmentId == department.DepartmentId)
                .OrderByDescending(employee => employee.EmployeeId == department.HeadOfDepartmentId)
                .ThenBy(employee => employee.DisplayName)
                .Select(employee => new EmployeeDirectoryMemberViewModel
                {
                    EmployeeId = employee.EmployeeId,
                    DisplayName = employee.DisplayName,
                    JobTitle = employee.EmployeeId == department.HeadOfDepartmentId
                        ? "Head of Department"
                        : employee.JobTitle,
                    IsActive = employee.IsActive,
                    IsHeadOfDepartment = employee.EmployeeId == department.HeadOfDepartmentId
                })
                .ToArray();

            var style = CardStyles[index % CardStyles.Length];

            return new EmployeeDepartmentSummaryViewModel
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                HeadOfDepartmentName = department.HeadOfDepartmentName,
                EmployeeCount = department.EmployeeCount,
                IsActive = department.IsActive,
                IconClass = style.Icon,
                Gradient = style.Gradient,
                Staff = staff
            };
        }

        private sealed class ApiEnvelope<T>
        {
            public bool Success { get; init; }

            public T? Data { get; init; }

            public string? Message { get; init; }
        }

        private sealed class EmployeeDepartmentDto
        {
            public Guid DepartmentId { get; init; }

            public string Name { get; init; } = string.Empty;

            public string Code { get; init; } = string.Empty;

            public string? Description { get; init; }

            public Guid? HeadOfDepartmentId { get; init; }

            public string? HeadOfDepartmentName { get; init; }

            public int EmployeeCount { get; init; }

            public bool IsActive { get; init; }
        }

        private sealed class EmployeeDto
        {
            public Guid EmployeeId { get; init; }

            public string DisplayName { get; init; } = string.Empty;

            public string JobTitle { get; init; } = string.Empty;

            public Guid? DepartmentId { get; init; }

            public bool IsActive { get; init; }
        }
    }
}
