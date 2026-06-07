using ForekOnline.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.ViewModels;

public class MyStudentsViewModel
{
    public string? Search { get; set; }
    public Guid? GroupId { get; set; }
    public bool UngroupedOnly { get; set; }
    public IReadOnlyList<LearningGroup> Groups { get; set; } = [];
    public IReadOnlyList<MyStudentRowViewModel> Students { get; set; } = [];
}

public class MyStudentRowViewModel
{
    public Guid LinkId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string? Course { get; set; }
    public string? ContactNumber { get; set; }
    public string? EnrollmentStatus { get; set; }
    public IReadOnlyList<GroupBadgeViewModel> Groups { get; set; } = [];
    public string AttendanceSummary { get; set; } = "No attendance";
    public DateTime? LastAttendanceDate { get; set; }
    public DateTimeOffset? LastCommunicationDate { get; set; }
}

public class GroupBadgeViewModel { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public string Color { get; set; } = string.Empty; }

public class AddStudentsViewModel
{
    public string? Search { get; set; }
    public string? EnrollmentStatus { get; set; }
    public List<Guid> StudentIds { get; set; } = [];
    public IReadOnlyList<SelectableStudentViewModel> Students { get; set; } = [];
}

public class SelectableStudentViewModel
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string? IdentityNumber { get; set; }
    public string? Course { get; set; }
    public string? EnrollmentStatus { get; set; }
    public int OtherFacilitatorCount { get; set; }
    public bool AlreadyMine { get; set; }
}

public class CreateLearningGroupViewModel
{
    [Required, MaxLength(120)] public string GroupName { get; set; } = string.Empty;
    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")] public string Color { get; set; } = "#E65100";
    [MaxLength(1500)] public string? Note { get; set; }
}

public class GroupsViewModel
{
    public IReadOnlyList<GroupSummaryViewModel> Groups { get; set; } = [];
    public int UngroupedStudentCount { get; set; }
    public CreateLearningGroupViewModel Create { get; set; } = new();
}

public class GroupSummaryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Note { get; set; }
    public LearningGroupStatus Status { get; set; }
    public int StudentCount { get; set; }
    public int AttendanceSessionCount { get; set; }
    public DateTimeOffset Created { get; set; }
}

public class GroupDetailsViewModel
{
    public LearningGroup Group { get; set; } = null!;
    public IReadOnlyList<MyStudentRowViewModel> Members { get; set; } = [];
    public IReadOnlyList<MyStudentRowViewModel> AvailableStudents { get; set; } = [];
    public IReadOnlyList<LearnerAttendanceSession> RecentSessions { get; set; } = [];
}

public class AttendanceEntryViewModel
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public LearnerAttendanceStatus Status { get; set; }
    [MaxLength(1000)] public string? Comment { get; set; }
}

public class TakeAttendanceViewModel
{
    public Guid? SessionId { get; set; }
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string GroupColor { get; set; } = string.Empty;
    [DataType(DataType.Date)] public DateTime AttendanceDate { get; set; } = DateTime.Today;
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    [MaxLength(250)] public string? Topic { get; set; }
    [MaxLength(1500)] public string? Note { get; set; }
    [MaxLength(500)] public string? EditReason { get; set; }
    public bool Submit { get; set; }
    public List<AttendanceEntryViewModel> Entries { get; set; } = [];
}

public class SendLearnerMessageViewModel
{
    public Guid? GroupId { get; set; }
    public bool AllMyStudents { get; set; }
    public List<Guid> StudentIds { get; set; } = [];
    [Required, MaxLength(500)] public string Message { get; set; } = string.Empty;
    public bool SendInApp { get; set; } = true;
    public bool QueueWhatsApp { get; set; }
    public IReadOnlyList<LearningGroup> Groups { get; set; } = [];
    public IReadOnlyList<MyStudentRowViewModel> Students { get; set; } = [];
}

public class AdminLearnerManagementViewModel
{
    public IReadOnlyList<FacilitatorCoverageViewModel> Facilitators { get; set; } = [];
    public IReadOnlyList<StudentEntity> UnassignedStudents { get; set; } = [];
    public IReadOnlyList<MultiFacilitatorStudentViewModel> MultipleFacilitatorStudents { get; set; } = [];
    public IReadOnlyList<GroupAdminRowViewModel> Groups { get; set; } = [];
    public IReadOnlyList<LearnerAttendanceSession> RecentAttendance { get; set; } = [];
    public IReadOnlyList<LearnerMessageLog> RecentMessages { get; set; } = [];
    public IReadOnlyList<FacilitatorActivityAudit> RecentActivity { get; set; } = [];
}

public class FacilitatorCoverageViewModel
{
    public Guid FacilitatorId { get; set; }
    public string FacilitatorName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public int ActiveGroupCount { get; set; }
    public int AttendanceSessionCount { get; set; }
    public DateTimeOffset? LastActivityUtc { get; set; }
}

public class MultiFacilitatorStudentViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public IReadOnlyList<string> Facilitators { get; set; } = [];
}

public class GroupAdminRowViewModel
{
    public string FacilitatorName { get; set; } = string.Empty;
    public GroupSummaryViewModel Group { get; set; } = new();
}
