using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForekOnline.Domain.Entities;

public enum FacilitatorStudentLinkStatus { Active, Removed, Inactive, PendingApproval, Approved, Rejected, Transferred, Expired }
public enum LearningGroupStatus { Active, Inactive, Archived }
public enum AttendanceSessionStatus { Draft, Submitted, Cancelled }
public enum LearnerAttendanceStatus { NotMarked, Present, Absent, Late, Excused }
public enum LearnerMessageChannel { InApp, WhatsApp }
public enum LearnerMessageStatus { Queued, Sent, Failed, Skipped }

[Table("FacilitatorStudentLinks", Schema = "Learning")]
public class FacilitatorStudentLink : EntityBase<Guid>
{
    public Guid FacilitatorId { get; set; }
    public Guid StudentId { get; set; }
    public DateTimeOffset DateAdded { get; set; }
    public Guid AddedById { get; set; }
    public FacilitatorStudentLinkStatus Status { get; set; } = FacilitatorStudentLinkStatus.Active;
    public DateTimeOffset? DateRemoved { get; set; }
    public Guid? RemovedById { get; set; }
    [MaxLength(250)] public string? RemovalReason { get; set; }
    [MaxLength(1000)] public string? Notes { get; set; }
    public User Facilitator { get; set; } = null!;
    public StudentEntity Student { get; set; } = null!;
}

[Table("LearningGroups", Schema = "Learning")]
public class LearningGroup : EntityBase<Guid>
{
    public Guid FacilitatorId { get; set; }
    [Required, MaxLength(120)] public string GroupName { get; set; } = string.Empty;
    [Required, MaxLength(7)] public string Color { get; set; } = "#E65100";
    [MaxLength(1500)] public string? Note { get; set; }
    public LearningGroupStatus Status { get; set; } = LearningGroupStatus.Active;
    public User Facilitator { get; set; } = null!;
    public ICollection<LearningGroupStudent> Students { get; set; } = [];
    public ICollection<LearnerAttendanceSession> AttendanceSessions { get; set; } = [];
}

[Table("LearningGroupStudents", Schema = "Learning")]
public class LearningGroupStudent : EntityBase<Guid>
{
    public Guid LearningGroupId { get; set; }
    public Guid FacilitatorStudentLinkId { get; set; }
    public DateTimeOffset DateAdded { get; set; }
    public Guid AddedById { get; set; }
    public LearningGroup LearningGroup { get; set; } = null!;
    public FacilitatorStudentLink FacilitatorStudentLink { get; set; } = null!;
}

[Table("AttendanceSessions", Schema = "Learning")]
public class LearnerAttendanceSession : EntityBase<Guid>
{
    public Guid FacilitatorId { get; set; }
    public Guid LearningGroupId { get; set; }
    [DataType(DataType.Date)] public DateTime AttendanceDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    [MaxLength(250)] public string? Topic { get; set; }
    [MaxLength(1500)] public string? Note { get; set; }
    public AttendanceSessionStatus Status { get; set; } = AttendanceSessionStatus.Draft;
    public DateTimeOffset? SubmittedUtc { get; set; }
    public User Facilitator { get; set; } = null!;
    public LearningGroup LearningGroup { get; set; } = null!;
    public ICollection<LearnerAttendanceRecord> Records { get; set; } = [];
}

[Table("AttendanceRecords", Schema = "Learning")]
public class LearnerAttendanceRecord : EntityBase<Guid>
{
    public Guid AttendanceSessionId { get; set; }
    public Guid StudentId { get; set; }
    public LearnerAttendanceStatus AttendanceStatus { get; set; } = LearnerAttendanceStatus.NotMarked;
    [MaxLength(1000)] public string? Comment { get; set; }
    public Guid MarkedById { get; set; }
    public DateTimeOffset MarkedUtc { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTimeOffset? UpdatedUtc { get; set; }
    public LearnerAttendanceSession AttendanceSession { get; set; } = null!;
    public StudentEntity Student { get; set; } = null!;
}

[Table("AttendanceRecordAudits", Schema = "Learning")]
public class LearnerAttendanceRecordAudit : EntityBase<Guid>
{
    public Guid AttendanceRecordId { get; set; }
    public LearnerAttendanceStatus OriginalStatus { get; set; }
    public LearnerAttendanceStatus NewStatus { get; set; }
    [MaxLength(1000)] public string? OriginalComment { get; set; }
    [MaxLength(1000)] public string? NewComment { get; set; }
    public Guid EditedById { get; set; }
    public DateTimeOffset EditedUtc { get; set; }
    [MaxLength(500)] public string? Reason { get; set; }
    public LearnerAttendanceRecord AttendanceRecord { get; set; } = null!;
}

[Table("LearnerMessageLogs", Schema = "Learning")]
public class LearnerMessageLog : EntityBase<Guid>
{
    public Guid FacilitatorId { get; set; }
    public Guid? LearningGroupId { get; set; }
    public Guid StudentId { get; set; }
    public LearnerMessageChannel Channel { get; set; }
    public LearnerMessageStatus Status { get; set; }
    [Required, MaxLength(500)] public string Message { get; set; } = string.Empty;
    [MaxLength(100)] public string? Destination { get; set; }
    [MaxLength(1000)] public string? ProviderResponse { get; set; }
    public DateTimeOffset QueuedUtc { get; set; }
    public DateTimeOffset? SentUtc { get; set; }
    public User Facilitator { get; set; } = null!;
    public LearningGroup? LearningGroup { get; set; }
    public StudentEntity Student { get; set; } = null!;
}

[Table("FacilitatorActivityAudits", Schema = "Learning")]
public class FacilitatorActivityAudit : EntityBase<Guid>
{
    public Guid ActorUserId { get; set; }
    [Required, MaxLength(100)] public string EventType { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    [MaxLength(2000)] public string? PreviousValue { get; set; }
    [MaxLength(2000)] public string? NewValue { get; set; }
    [MaxLength(1000)] public string? Notes { get; set; }
    public DateTimeOffset EventUtc { get; set; }
    public User ActorUser { get; set; } = null!;
}
