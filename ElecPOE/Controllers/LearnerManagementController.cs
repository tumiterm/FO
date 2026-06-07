// <copyright file="LearnerManagementController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/06/2026 10:48 AM
// Purpose:         Defines the LearnerManagementController

#region Usings
using ElecPOE.ViewModels;
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
#endregion

namespace ElecPOE.Controllers;

[Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
public class LearnerManagementController : Controller
{
    #region Fields
    private readonly ApplicationDbContext _db;
    private readonly IInAppNotificationService _notifications;
    #endregion
    public LearnerManagementController(ApplicationDbContext db, IInAppNotificationService notifications)
    {
        _db = db;
        _notifications = notifications;
    }

    public IActionResult Index() => User.IsInRole("Facilitator") ? RedirectToAction(nameof(MyStudents)) : RedirectToAction(nameof(Admin));

    [Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> MyStudents(string? search, Guid? groupId, bool ungroupedOnly = false)
    {
        var facilitatorId = CurrentUserId();
        var links = await ActiveLinks(facilitatorId)
            .Include(x => x.Student).ThenInclude(x => x.Enrollments)
            .ToListAsync();
        var linkIds = links.Select(x => x.Id).ToList();
        var memberships = await _db.LearningGroupStudents
            .Where(x => linkIds.Contains(x.FacilitatorStudentLinkId) && !x.IsDeleted)
            .Include(x => x.LearningGroup).ToListAsync();

        if (groupId.HasValue)
        {
            var groupedIds = memberships.Where(x => x.LearningGroupId == groupId).Select(x => x.FacilitatorStudentLinkId).ToHashSet();
            links = links.Where(x => groupedIds.Contains(x.Id)).ToList();
        }
        if (ungroupedOnly)
        {
            var groupedIds = memberships.Select(x => x.FacilitatorStudentLinkId).ToHashSet();
            links = links.Where(x => !groupedIds.Contains(x.Id)).ToList();
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            links = links.Where(x => x.Student.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
                || x.Student.StudentNumber.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var rows = await BuildStudentRows(links, memberships);
        return View(new MyStudentsViewModel
        {
            Search = search,
            GroupId = groupId,
            UngroupedOnly = ungroupedOnly,
            Students = rows,
            Groups = await OwnedGroups(facilitatorId).OrderBy(x => x.GroupName).ToListAsync()
        });
    }

    [Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> AddStudents(string? search, string? enrollmentStatus)
    {
        var facilitatorId = CurrentUserId();
        var query = _db.Students.Include(x => x.Enrollments).Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.StudentNumber.Contains(search) || x.Name.Contains(search) || x.LastName.Contains(search)
                || (x.IDNumber != null && x.IDNumber.Contains(search)) || (x.PassportNumber != null && x.PassportNumber.Contains(search))
                || x.Enrollments.Any(e => e.CourseTitle != null && e.CourseTitle.Contains(search)));
        if (!string.IsNullOrWhiteSpace(enrollmentStatus))
            query = query.Where(x => x.Enrollments.Any(e => e.EnrollmentStatus == enrollmentStatus));

        var students = await query.OrderBy(x => x.LastName).ThenBy(x => x.Name).Take(250).ToListAsync();
        var links = await _db.FacilitatorStudentLinks.Where(x => students.Select(s => s.Id).Contains(x.StudentId) && x.Status == FacilitatorStudentLinkStatus.Active).ToListAsync();
        return View(new AddStudentsViewModel
        {
            Search = search,
            EnrollmentStatus = enrollmentStatus,
            Students = students.Select(s => new SelectableStudentViewModel
            {
                Id = s.Id,
                StudentName = s.FullName,
                StudentNumber = s.StudentNumber,
                IdentityNumber = s.IDNumber ?? s.PassportNumber,
                Course = s.Enrollments.FirstOrDefault(e => e.IsActive)?.CourseTitle,
                EnrollmentStatus = s.Enrollments.FirstOrDefault(e => e.IsActive)?.EnrollmentStatus,
                AlreadyMine = links.Any(x => x.StudentId == s.Id && x.FacilitatorId == facilitatorId),
                OtherFacilitatorCount = links.Count(x => x.StudentId == s.Id && x.FacilitatorId != facilitatorId)
            }).ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> AddStudents(AddStudentsViewModel model)
    {
        var facilitatorId = CurrentUserId();
        model.StudentIds ??= [];
        var existing = await ActiveLinks(facilitatorId).Where(x => model.StudentIds.Contains(x.StudentId)).Select(x => x.StudentId).ToListAsync();
        var validIds = await _db.Students.Where(x => model.StudentIds.Contains(x.Id) && !x.IsDeleted).Select(x => x.Id).ToListAsync();
        foreach (var studentId in validIds.Except(existing))
        {
            var link = new FacilitatorStudentLink
            {
                Id = Guid.NewGuid(), FacilitatorId = facilitatorId, StudentId = studentId, AddedById = facilitatorId,
                DateAdded = DateTimeHelper.GetCurrentSastDateTimeOffset(), DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(), UserCreated = User.Identity?.Name
            };
            _db.FacilitatorStudentLinks.Add(link);
            AddAudit(facilitatorId, "StudentAddedToFacilitator", nameof(FacilitatorStudentLink), link.Id, null, new { studentId });
        }
        await _db.SaveChangesAsync();
        TempData["success"] = $"{validIds.Except(existing).Count()} student(s) added to My Students.";
        return RedirectToAction(nameof(MyStudents));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> RemoveStudent(Guid linkId, string? reason)
    {
        var facilitatorId = CurrentUserId();
        var link = await ActiveLinks(facilitatorId).FirstOrDefaultAsync(x => x.Id == linkId);
        if (link == null) return NotFound();
        var memberships = await _db.LearningGroupStudents.Where(x => x.FacilitatorStudentLinkId == link.Id && !x.IsDeleted).ToListAsync();
        foreach (var membership in memberships) { membership.IsDeleted = true; membership.DateDeleted = DateTimeHelper.GetCurrentSastDateTimeOffset(); }
        link.Status = FacilitatorStudentLinkStatus.Removed;
        link.DateRemoved = DateTimeHelper.GetCurrentSastDateTimeOffset();
        link.RemovedById = facilitatorId;
        link.RemovalReason = reason;
        AddAudit(facilitatorId, "StudentRemovedFromFacilitator", nameof(FacilitatorStudentLink), link.Id, new { status = "Active" }, new { status = "Removed", reason });
        await _db.SaveChangesAsync();
        TempData["success"] = "Student removed from your list. Historical records were retained.";
        return RedirectToAction(nameof(MyStudents));
    }

    [Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> Groups()
    {
        var facilitatorId = CurrentUserId();
        var groups = await OwnedGroups(facilitatorId).Include(x => x.Students).Include(x => x.AttendanceSessions).OrderBy(x => x.GroupName).ToListAsync();
        var activeLinkIds = await ActiveLinks(facilitatorId).Select(x => x.Id).ToListAsync();
        var groupedIds = await _db.LearningGroupStudents.Where(x => activeLinkIds.Contains(x.FacilitatorStudentLinkId) && !x.IsDeleted).Select(x => x.FacilitatorStudentLinkId).Distinct().ToListAsync();
        return View(new GroupsViewModel
        {
            UngroupedStudentCount = activeLinkIds.Except(groupedIds).Count(),
            Groups = groups.Select(ToGroupSummary).ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> CreateGroup([Bind(Prefix = "Create")] CreateLearningGroupViewModel model)
    {
        if (!ModelState.IsValid) return await Groups();
        var facilitatorId = CurrentUserId();
        var group = new LearningGroup
        {
            Id = Guid.NewGuid(), FacilitatorId = facilitatorId, GroupName = model.GroupName.Trim(), Name = model.GroupName.Trim(),
            Color = model.Color, Note = model.Note, DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(), UserCreated = User.Identity?.Name
        };
        _db.LearningGroups.Add(group);
        AddAudit(facilitatorId, "GroupCreated", nameof(LearningGroup), group.Id, null, new { group.GroupName, group.Color, group.Note });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Group), new { id = group.Id });
    }

    [Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> Group(Guid id)
    {
        var facilitatorId = CurrentUserId();
        var group = await OwnedGroups(facilitatorId).FirstOrDefaultAsync(x => x.Id == id);
        if (group == null) return NotFound();
        var links = await ActiveLinks(facilitatorId).Include(x => x.Student).ThenInclude(x => x.Enrollments).ToListAsync();
        var memberships = await _db.LearningGroupStudents.Where(x => x.LearningGroupId == id && !x.IsDeleted).Include(x => x.LearningGroup).ToListAsync();
        var memberIds = memberships.Select(x => x.FacilitatorStudentLinkId).ToHashSet();
        return View(new GroupDetailsViewModel
        {
            Group = group,
            Members = await BuildStudentRows(links.Where(x => memberIds.Contains(x.Id)).ToList(), memberships),
            AvailableStudents = await BuildStudentRows(links.Where(x => !memberIds.Contains(x.Id)).ToList(), []),
            RecentSessions = await _db.LearnerAttendanceSessions.Where(x => x.LearningGroupId == id).OrderByDescending(x => x.AttendanceDate).Take(10).ToListAsync()
        });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> EditGroup(Guid id, CreateLearningGroupViewModel model)
    {
        if (!ModelState.IsValid) { TempData["error"] = "Please provide a valid group name and color."; return RedirectToAction(nameof(Group), new { id }); }
        var facilitatorId = CurrentUserId();
        var group = await OwnedGroups(facilitatorId).FirstOrDefaultAsync(x => x.Id == id);
        if (group == null) return NotFound();

        var previous = new { group.GroupName, group.Color, group.Note };
        group.GroupName = model.GroupName.Trim();
        group.Name = group.GroupName;
        group.Color = model.Color;
        group.Note = model.Note;
        group.DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset();
        group.UserModified = User.Identity?.Name;

        AddAudit(facilitatorId, "GroupUpdated", nameof(LearningGroup), group.Id, previous, new { group.GroupName, group.Color, group.Note });
        await _db.SaveChangesAsync();
        TempData["success"] = "Group details updated.";
        return RedirectToAction(nameof(Group), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> AddGroupStudents(Guid groupId, List<Guid> linkIds)
    {
        var facilitatorId = CurrentUserId();

        if (!await OwnedGroups(facilitatorId).AnyAsync(x => x.Id == groupId)) return Forbid();
        var validLinks = await ActiveLinks(facilitatorId).Where(x => linkIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();
        var existing = await _db.LearningGroupStudents.Where(x => x.LearningGroupId == groupId && validLinks.Contains(x.FacilitatorStudentLinkId) && !x.IsDeleted).Select(x => x.FacilitatorStudentLinkId).ToListAsync();
        foreach (var linkId in validLinks.Except(existing))
        {
            var membership = new LearningGroupStudent { Id = Guid.NewGuid(), LearningGroupId = groupId, FacilitatorStudentLinkId = linkId, AddedById = facilitatorId, DateAdded = DateTimeHelper.GetCurrentSastDateTimeOffset(), DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset() };
            _db.LearningGroupStudents.Add(membership);
            AddAudit(facilitatorId, "StudentAddedToGroup", nameof(LearningGroupStudent), membership.Id, null, new { groupId, linkId });
        }
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Group), new { id = groupId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> RemoveGroupStudent(Guid groupId, Guid linkId)
    {
        var facilitatorId = CurrentUserId();
        if (!await OwnedGroups(facilitatorId).AnyAsync(x => x.Id == groupId)) return Forbid();
        var membership = await _db.LearningGroupStudents.FirstOrDefaultAsync(x => x.LearningGroupId == groupId && x.FacilitatorStudentLinkId == linkId && !x.IsDeleted);
        if (membership != null) {
            membership.IsDeleted = true;
            membership.DateDeleted = DateTimeHelper.GetCurrentSastDateTimeOffset();
            AddAudit(facilitatorId, "StudentRemovedFromGroup", nameof(LearningGroupStudent), membership.Id, null, new { groupId, linkId });
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Group), new { id = groupId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> ArchiveGroup(Guid id)
    {
        var facilitatorId = CurrentUserId();
        var group = await OwnedGroups(facilitatorId).FirstOrDefaultAsync(x => x.Id == id);
        if (group == null) return NotFound();
        var old = group.Status; group.Status = LearningGroupStatus.Archived;
        group.DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset();
        AddAudit(facilitatorId, "GroupArchived", nameof(LearningGroup), group.Id, new { status = old }, new { status = group.Status });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Groups));
    }

    [Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> TakeAttendance(Guid groupId, Guid? sessionId)
    {
        var facilitatorId = CurrentUserId();

        var group = await OwnedGroups(facilitatorId).FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null) return NotFound();

        var memberStudents = await _db.LearningGroupStudents.Where(x => x.LearningGroupId == groupId && !x.IsDeleted && x.FacilitatorStudentLink.Status == FacilitatorStudentLinkStatus.Active)
            .Select(x => x.FacilitatorStudentLink.Student).OrderBy(x => x.LastName).ThenBy(x => x.Name).ToListAsync();
        LearnerAttendanceSession? session = null;
        if (sessionId.HasValue) session = await _db.LearnerAttendanceSessions.Include(x => x.Records).FirstOrDefaultAsync(x => x.Id == sessionId && x.FacilitatorId == facilitatorId && x.LearningGroupId == groupId);
        return View(new TakeAttendanceViewModel
        {
            SessionId = session?.Id, GroupId = group.Id, GroupName = group.GroupName, GroupColor = group.Color,
            AttendanceDate = session?.AttendanceDate ?? DateTime.Today, StartTime = session?.StartTime, EndTime = session?.EndTime, Topic = session?.Topic, Note = session?.Note,
            Entries = memberStudents.Select(s => { var record = session?.Records.FirstOrDefault(r => r.StudentId == s.Id); return new AttendanceEntryViewModel { StudentId = s.Id, StudentName = s.FullName, StudentNumber = s.StudentNumber, Status = record?.AttendanceStatus ?? LearnerAttendanceStatus.NotMarked, Comment = record?.Comment }; }).ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> SaveAttendance(TakeAttendanceViewModel model)
    {
        var facilitatorId = CurrentUserId();

        var group = await OwnedGroups(facilitatorId).FirstOrDefaultAsync(x => x.Id == model.GroupId);
        if (group == null) return Forbid();
        var allowedStudents = await _db.LearningGroupStudents.Where(x => x.LearningGroupId == model.GroupId && !x.IsDeleted).Select(x => x.FacilitatorStudentLink.StudentId).ToListAsync();
        LearnerAttendanceSession? session = null;
        if (model.SessionId.HasValue) session = await _db.LearnerAttendanceSessions.Include(x => x.Records).FirstOrDefaultAsync(x => x.Id == model.SessionId && x.FacilitatorId == facilitatorId && x.LearningGroupId == model.GroupId);
        var isNew = session == null;
        var wasSubmitted = session?.Status == AttendanceSessionStatus.Submitted;
        session ??= new LearnerAttendanceSession 
        { 
            Id = Guid.NewGuid(),
            FacilitatorId = facilitatorId,
            LearningGroupId = model.GroupId,
            DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(),
            UserCreated = User.Identity?.Name
        };

        session.AttendanceDate = model.AttendanceDate.Date;
        session.StartTime = model.StartTime;
        session.EndTime = model.EndTime;
        session.Topic = model.Topic;
        session.Note = model.Note;
        session.Status = model.Submit ? AttendanceSessionStatus.Submitted : AttendanceSessionStatus.Draft;

        if (model.Submit) session.SubmittedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset();
        if (isNew) { _db.LearnerAttendanceSessions.Add(session); AddAudit(facilitatorId, "AttendanceSessionCreated", nameof(LearnerAttendanceSession), session.Id, null, new { model.GroupId, model.AttendanceDate }); }
        foreach (var entry in model.Entries.Where(x => allowedStudents.Contains(x.StudentId)))
        {
            var record = session.Records.FirstOrDefault(x => x.StudentId == entry.StudentId);
            if (record == null)
            {
                record = new LearnerAttendanceRecord 
                { 
                    Id = Guid.NewGuid(),
                    AttendanceSessionId = session.Id,
                    StudentId = entry.StudentId,
                    MarkedById = facilitatorId,
                    MarkedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                    DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset() 
                };
                session.Records.Add(record);
            }
            else if (record.AttendanceStatus != entry.Status || record.Comment != entry.Comment)
            {
                _db.LearnerAttendanceRecordAudits.Add(new LearnerAttendanceRecordAudit 
                { 
                    Id = Guid.NewGuid(),
                    AttendanceRecordId = record.Id,
                    OriginalStatus = record.AttendanceStatus,
                    NewStatus = entry.Status,
                    OriginalComment = record.Comment,
                    NewComment = entry.Comment,
                    EditedById = facilitatorId,
                    EditedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                    Reason = model.EditReason,
                    DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset()
                });
                record.UpdatedById = facilitatorId;
                record.UpdatedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset();
            }
            record.AttendanceStatus = entry.Status; record.Comment = entry.Comment;
        }
        var attendanceEvent = wasSubmitted ? "AttendanceEdited" : model.Submit ? "AttendanceSubmitted" : "AttendanceDraftSaved";
        AddAudit(facilitatorId, attendanceEvent, nameof(LearnerAttendanceSession), session.Id, null, new { session.Status, count = model.Entries.Count, model.EditReason });
        await _db.SaveChangesAsync();
        if (model.Submit)
        {
            var absentStudentNumbers = await _db.LearnerAttendanceRecords.Where(x => x.AttendanceSessionId == session.Id && x.AttendanceStatus == LearnerAttendanceStatus.Absent).Select(x => x.Student.StudentNumber).ToListAsync();
            var absentStudents = await _db.Students.Where(x => absentStudentNumbers.Contains(x.StudentNumber)).ToListAsync();
            var users = await _db.Users.Where(x => x.StudentNumber != null && absentStudentNumbers.Contains(x.StudentNumber)).ToListAsync();
            var absenceMessage = $"You were marked absent for {group.GroupName} on {session.AttendanceDate:dd MMM yyyy}.";
            if (users.Count > 0) await _notifications.SendToManyAsync(users.Select(x => x.Id), absenceMessage, null, "fa fa-calendar-times", User.Identity?.Name);
            foreach (var student in absentStudents)
                AddMessageLog(facilitatorId, group.Id, student, LearnerMessageChannel.InApp, users.Any(u => u.StudentNumber == student.StudentNumber) ? LearnerMessageStatus.Sent : LearnerMessageStatus.Skipped, absenceMessage, student.StudentNumber, "Attendance absence notification.");
            await _db.SaveChangesAsync();
        }
        TempData["success"] = model.Submit ? "Attendance submitted." : "Attendance saved as draft.";
        return RedirectToAction(nameof(Group), new { id = model.GroupId });
    }

    [Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> SendMessage(Guid? groupId)
    {
        var facilitatorId = CurrentUserId();
        var links = await ActiveLinks(facilitatorId).Include(x => x.Student).ThenInclude(x => x.Enrollments).ToListAsync();
        return View(new SendLearnerMessageViewModel { GroupId = groupId, Groups = await OwnedGroups(facilitatorId).OrderBy(x => x.GroupName).ToListAsync(), Students = await BuildStudentRows(links, []) });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> SendMessage(SendLearnerMessageViewModel model)
    {
        var facilitatorId = CurrentUserId();
        if (!ModelState.IsValid) return await SendMessage(model.GroupId);
        model.StudentIds ??= [];
        var studentIds = new HashSet<Guid>();
        if (model.AllMyStudents) studentIds.UnionWith(await ActiveLinks(facilitatorId).Select(x => x.StudentId).ToListAsync());
        if (model.GroupId.HasValue)
        {
            if (!await OwnedGroups(facilitatorId).AnyAsync(x => x.Id == model.GroupId)) return Forbid();
            studentIds.UnionWith(await _db.LearningGroupStudents.Where(x => x.LearningGroupId == model.GroupId && !x.IsDeleted).Select(x => x.FacilitatorStudentLink.StudentId).ToListAsync());
        }
        studentIds.UnionWith(await ActiveLinks(facilitatorId).Where(x => model.StudentIds.Contains(x.StudentId)).Select(x => x.StudentId).ToListAsync());
        var students = await _db.Students.Where(x => studentIds.Contains(x.Id)).ToListAsync();
        if (model.SendInApp)
        {
            var numbers = students.Select(x => x.StudentNumber).ToList();
            var users = await _db.Users.Where(x => x.StudentNumber != null && numbers.Contains(x.StudentNumber)).ToListAsync();
            await _notifications.SendToManyAsync(users.Select(x => x.Id), model.Message, null, "fa fa-bullhorn", User.Identity?.Name);
            foreach (var student in students) AddMessageLog(facilitatorId, model.GroupId, student, LearnerMessageChannel.InApp, users.Any(u => u.StudentNumber == student.StudentNumber) ? LearnerMessageStatus.Sent : LearnerMessageStatus.Skipped, model.Message, student.StudentNumber, users.Any(u => u.StudentNumber == student.StudentNumber) ? null : "No linked student user account.");
        }
        if (model.QueueWhatsApp)
            foreach (var student in students) AddMessageLog(facilitatorId, model.GroupId, student, LearnerMessageChannel.WhatsApp, string.IsNullOrWhiteSpace(student.Cellphone) ? LearnerMessageStatus.Skipped : LearnerMessageStatus.Queued, model.Message, student.Cellphone, string.IsNullOrWhiteSpace(student.Cellphone) ? "No cellphone number." : "Queued for a future WhatsApp provider integration.");
        AddAudit(facilitatorId, "NotificationSent", nameof(LearnerMessageLog), null, null, new { model.GroupId, recipients = students.Count, model.SendInApp, model.QueueWhatsApp });
        await _db.SaveChangesAsync();
        TempData["success"] = $"Message processed for {students.Count} student(s).";
        return RedirectToAction(nameof(SendMessage), new { groupId = model.GroupId });
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Admin()
    {
        var facilitators = await _db.Users.Where(x => x.IsActive && x.Role == ForekOnline.Domain.Enums.EnumRegistry.eSysRole.Facilitator).OrderBy(x => x.Name).ToListAsync();
        var activeLinks = await _db.FacilitatorStudentLinks.Where(x => x.Status == FacilitatorStudentLinkStatus.Active).Include(x => x.Student).Include(x => x.Facilitator).ToListAsync();
        var groups = await _db.LearningGroups.Include(x => x.Facilitator).Include(x => x.Students).Include(x => x.AttendanceSessions).OrderByDescending(x => x.DateCreated).ToListAsync();
        var activity = await _db.FacilitatorActivityAudits.Include(x => x.ActorUser).OrderByDescending(x => x.EventUtc).Take(100).ToListAsync();
        var assignedIds = activeLinks.Select(x => x.StudentId).Distinct().ToList();
        return View(new AdminLearnerManagementViewModel
        {
            Facilitators = facilitators.Select(f => new FacilitatorCoverageViewModel 
            { 
                FacilitatorId = f.Id,
                FacilitatorName = $"{f.Name} {f.LastName}",
                StudentCount = activeLinks.Count(x => x.FacilitatorId == f.Id),
                ActiveGroupCount = groups.Count(x => x.FacilitatorId == f.Id && x.Status == LearningGroupStatus.Active),
                AttendanceSessionCount = groups.Where(x => x.FacilitatorId == f.Id).Sum(x => x.AttendanceSessions.Count),
                LastActivityUtc = activity.FirstOrDefault(x => x.ActorUserId == f.Id)?.EventUtc }).ToList(),

            UnassignedStudents = await _db.Students.Where(x => !x.IsDeleted && !assignedIds.Contains(x.Id)).
                                 OrderBy(x => x.LastName).Take(100).
                                 ToListAsync(),

            MultipleFacilitatorStudents = activeLinks.GroupBy(x => x.StudentId)
                                        .Where(x => x.Select(l => l.FacilitatorId)
                                        .Distinct().Count() > 1)
                                        .Select(x => new MultiFacilitatorStudentViewModel 
                                         { 
                                            StudentName = x.First().Student.FullName,
                                            StudentNumber = x.First().Student.StudentNumber,
                                            Facilitators = x.Select(l => $"{l.Facilitator.Name} {l.Facilitator.LastName}").Distinct().ToList() }).ToList(),

            Groups = groups.Select(x => new GroupAdminRowViewModel 
             { 
                FacilitatorName = $"{x.Facilitator.Name} {x.Facilitator.LastName}",
                Group = ToGroupSummary(x) }).
                ToList(),

            RecentAttendance = await _db.LearnerAttendanceSessions.Include(x => x.Facilitator)
                                     .Include(x => x.LearningGroup)
                                     .Include(x => x.Records)
                                     .OrderByDescending(x => x.DateCreated)
                                     .Take(50)
                                     .ToListAsync(),

            RecentMessages = await _db.LearnerMessageLogs.Include(x => x.Facilitator)
                            .Include(x => x.Student)
                            .OrderByDescending(x => x.QueuedUtc)
                            .Take(50)
                            .ToListAsync(),
            RecentActivity = activity
        });
    }

    #region Private Helpers
    private Guid CurrentUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException();
    private IQueryable<FacilitatorStudentLink> ActiveLinks(Guid facilitatorId) 
        => _db.FacilitatorStudentLinks.Where(x => x.FacilitatorId == facilitatorId && x.Status == FacilitatorStudentLinkStatus.Active && !x.IsDeleted);
    private IQueryable<LearningGroup> OwnedGroups(Guid facilitatorId) 
        => _db.LearningGroups.Where(x => x.FacilitatorId == facilitatorId && x.Status != LearningGroupStatus.Archived && !x.IsDeleted);

    private async Task<IReadOnlyList<MyStudentRowViewModel>> BuildStudentRows(IReadOnlyList<FacilitatorStudentLink> links, IReadOnlyList<LearningGroupStudent> memberships)
    {
        var studentIds = links.Select(x => x.StudentId).ToList();

        var attendance = await _db.LearnerAttendanceRecords.Where(x => studentIds.Contains(x.StudentId)
                         && x.AttendanceSession.Status == AttendanceSessionStatus.Submitted)
                         .Select(x => new 
                           { 
                             x.StudentId,
                             x.AttendanceStatus,
                             x.AttendanceSession.AttendanceDate })
                             .ToListAsync();

        var messages = await _db.LearnerMessageLogs.Where(x => studentIds.Contains(x.StudentId))
                       .GroupBy(x => x.StudentId)
                       .Select(x => new 
                        { 
                           StudentId = x.Key,
                           Last = x.Max(m => m.QueuedUtc) }).
                           ToListAsync();

        return links.Select(link =>
        {
            var records = attendance.Where(x => x.StudentId == link.StudentId).ToList();
            var present = records.Count(x => x.AttendanceStatus == LearnerAttendanceStatus.Present);

            return new MyStudentRowViewModel
            {
                LinkId = link.Id,
                StudentId = link.StudentId,
                StudentName = link.Student.FullName,
                StudentNumber = link.Student.StudentNumber,
                Course = link.Student.Enrollments?.FirstOrDefault(x => x.IsActive)?.CourseTitle,
                ContactNumber = link.Student.Cellphone,
                EnrollmentStatus = link.Student.Enrollments?.FirstOrDefault(x => x.IsActive)?.EnrollmentStatus,
                Groups = memberships.Where(x => x.FacilitatorStudentLinkId == link.Id)
                        .Select(x => new GroupBadgeViewModel 
                            { 
                                Id = x.LearningGroupId,
                                Name = x.LearningGroup.GroupName,
                                Color = x.LearningGroup.Color }).
                                ToList(),
                AttendanceSummary = records.Count == 0 ? "No attendance" : $"{present}/{records.Count} present ({present * 100 / records.Count}%)",
                LastAttendanceDate = records.OrderByDescending(x => x.AttendanceDate).FirstOrDefault()?.AttendanceDate,
                LastCommunicationDate = messages.FirstOrDefault(x => x.StudentId == link.StudentId)?.Last
            };
        }).OrderBy(x => x.StudentName).ToList();
    }

    private static GroupSummaryViewModel ToGroupSummary(LearningGroup x) 
        => new() 
        { 
            Id = x.Id,
            Name = x.GroupName,
            Color = x.Color,
            Note = x.Note,
            Status = x.Status,
            StudentCount = x.Students.Count(s => !s.IsDeleted),
            AttendanceSessionCount = x.AttendanceSessions.Count,
            Created = x.DateCreated
        };
    private void AddAudit(Guid actorId, string eventType, string entityType, Guid? entityId, object? previous, object? next, string? notes = null) 
            => _db.FacilitatorActivityAudits.Add(new FacilitatorActivityAudit 
                { 
                    Id = Guid.NewGuid(),
                    ActorUserId = actorId,
                    EventType = eventType,
                    EntityType = entityType,
                    EntityId = entityId,
                    PreviousValue = previous == null ? null : JsonSerializer.Serialize(previous),
                    NewValue = next == null ? null : JsonSerializer.Serialize(next),
                    Notes = notes,
                    EventUtc = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                    DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                    UserCreated = User.Identity?.Name 
            });
    private void AddMessageLog(Guid facilitatorId, Guid? groupId, StudentEntity student, LearnerMessageChannel channel, LearnerMessageStatus status, string message, string? destination, string? response)
        => _db.LearnerMessageLogs.Add(new LearnerMessageLog 
            { Id = Guid.NewGuid(),
            FacilitatorId = facilitatorId,
            LearningGroupId = groupId,
            StudentId = student.Id,
            Channel = channel,
            Status = status,
            Message = message,
            Destination = destination,
            ProviderResponse = response,
            QueuedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset(),
            SentUtc = status == LearnerMessageStatus.Sent ? DateTimeHelper.GetCurrentSastDateTimeOffset() : null,
            DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(),
            UserCreated = User.Identity?.Name });

    #endregion
}
