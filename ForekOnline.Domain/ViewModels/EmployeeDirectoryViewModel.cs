namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the employee department directory displayed in the employee portal.
    /// </summary>
    public sealed class EmployeeDirectoryViewModel
    {
        public IReadOnlyList<EmployeeDepartmentSummaryViewModel> Departments { get; init; } =
            Array.Empty<EmployeeDepartmentSummaryViewModel>();

        public string? ErrorMessage { get; init; }
    }

    /// <summary>
    /// Represents a department and its employees.
    /// </summary>
    public sealed class EmployeeDepartmentSummaryViewModel
    {
        public Guid DepartmentId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Code { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string? HeadOfDepartmentName { get; init; }

        public int EmployeeCount { get; init; }

        public bool IsActive { get; init; }

        public string IconClass { get; init; } = "fa-building";

        public string Gradient { get; init; } = "linear-gradient(135deg, #FF6B35 0%, #D62828 100%)";

        public IReadOnlyList<EmployeeDirectoryMemberViewModel> Staff { get; init; } =
            Array.Empty<EmployeeDirectoryMemberViewModel>();
    }

    /// <summary>
    /// Represents an employee shown on a department card.
    /// </summary>
    public sealed class EmployeeDirectoryMemberViewModel
    {
        public Guid EmployeeId { get; init; }

        public string DisplayName { get; init; } = string.Empty;

        public string JobTitle { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public bool IsHeadOfDepartment { get; init; }
    }
}
