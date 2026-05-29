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
    public class VisitService : IVisitService
    {
        #region Fields
        private readonly IUnitOfWork _uow;
        private readonly IStudentService _studentService;
        private readonly IFileUploadService _fileUploadService;
        #endregion

        public VisitService(IUnitOfWork uow, IStudentService studentService, IFileUploadService fileUploadService)
        {
            _uow = uow;
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
        }

        public async Task<VisitViewModel?> GetCreateViewModelAsync(Guid companyId, Guid? placementId = null, CancellationToken cancellationToken = default)
        {
            var company = await _uow.Company.GetAsync(c => c.CompanyId == companyId);
            if (company == null) return null;
            var contacts = await _uow.ContactPerson.GetAllAsync();
            var contact = contacts.FirstOrDefault(m => m.AssociativeId == company.CompanyId);
            if (contact == null) return null;

            return new VisitViewModel
            {
                Company = company.CompanyName,
                CompanyId = companyId,
                PlacementId = placementId,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Mentor = $"{contact.Name} {contact.LastName}",
                HasReport = false,
                LearnerFeedback = string.Empty
            };
        }

        public async Task<(bool Success, string Message, Visit? Visit)> CreateAsync(VisitViewModel model, User? currentUser, CancellationToken cancellationToken = default)
        {
            var visit = MapToEntity(model, currentUser, isCreate: true);
            if (visit.HasReport && visit.ReportFile != null)
            {
                await using var stream = visit.ReportFile.OpenReadStream();
                var upload = await _fileUploadService.UploadAsync(new UploadFileRequest(stream, visit.ReportFile.FileName, visit.ReportFile.ContentType, DocumentType: "Visitation"), cancellationToken);
                visit.Report = upload.FileId;
            }

            var created = await _uow.Visit.AddAsync(visit);
            if (created == null) return (false, "Error: Something went wrong!!!", null);
            var rc = await _uow.SaveAsync();
            return rc > 0 ? (true, "Visitation successfully captured", visit) : (false, "Error: Unable to save visitation details!!!", null);
        }

        public async Task<IReadOnlyCollection<VisitationViewModel>> GetVisitationListAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var all = await _uow.Visit.GetAllAsync();
            var company = await _uow.Company.GetAsync(c => c.CompanyId == companyId);
            var users = await _uow.Users.GetAllAsync();
            return all.Where(v => v.CompanyId == companyId).Select(v => new VisitationViewModel
            {
                VisitId = v.VisitId,
                CompanyId = v.CompanyId,
                Company = company?.CompanyName ?? string.Empty,
                PlacementId = v.PlacementId,
                Date = v.Date,
                DurationMinutes = v.DurationMinutes,
                HasReport = v.HasReport,
                Report = v.Report,
                Mentor = v.Mentor,
                VisitBy = users.FirstOrDefault(u => u.Id == v.VisitBy) is User user ? $"{user.Name} {user.LastName}" : string.Empty,
                VisitPurpose = v.VisitPurpose
            }).ToList();
        }

        public async Task<VisitViewModel?> GetForEditAsync(Guid visitId, CancellationToken cancellationToken = default)
        {
            var visit = await _uow.Visit.GetAsync(v => v.VisitId == visitId);
            var company = visit == null ? null : await _uow.Company.GetAsync(c => c.CompanyId == visit.CompanyId);
            if (visit == null || company == null) return null;
            return new VisitViewModel
            {
                VisitId = visit.VisitId,
                CompanyId = visit.CompanyId,
                Company = company.CompanyName,
                PlacementId = visit.PlacementId,
                Date = visit.Date.ToString("dddd, dd MMMM yyyy hh:mm tt"),
                DurationMinutes = visit.DurationMinutes,
                SelectedEmployeeIDs = visit.SelectedEmployeeIDs,
                SelectedIDArray = (visit.SelectedEmployeeIDs ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries),
                HasReport = visit.HasReport,
                VisitBy = visit.VisitBy,
                Mentor = visit.Mentor,
                VisitPurpose = visit.VisitPurpose,
                Report = visit.Report
            };
        }

        public async Task<(bool Success, string Message)> UpdateAsync(VisitViewModel model, User? currentUser, CancellationToken cancellationToken = default)
        {
            var visit = MapToEntity(model, currentUser, isCreate: false);
            var updated = await _uow.Visit.UpdateVisitAsync(visit);
            return updated != null ? (true, "Visitation details saved") : (false, "Error: Unable to save visit!!!");
        }

        public Task<DownloadFileResponse?> DownloadAttachmentAsync(string fileId, CancellationToken cancellationToken = default)
            => _fileUploadService.DownloadAsync(fileId, cancellationToken)!;

        public async Task<IReadOnlyCollection<StudentLookupItem>> SearchStudentsAsync(string? term, int max = 20, CancellationToken cancellationToken = default)
        {
            var students = await _studentService.GetStudentListAsync();
            term = term?.Trim() ?? string.Empty;
            return students
                .Where(s => string.IsNullOrWhiteSpace(term) || $"{s.FirstName} {s.LastName} {s.StudentNumber}".Contains(term, StringComparison.OrdinalIgnoreCase))
                .Take(max)
                .Select(s => new StudentLookupItem($"{s.FirstName} {s.LastName}", s.Email ?? string.Empty, s.StudentNumber ?? string.Empty))
                .ToList();
        }

        public async Task<(IReadOnlyCollection<SelectListItem> Students, IReadOnlyCollection<SelectListItem> Visitors)> GetLookupOptionsAsync(CancellationToken cancellationToken = default)
        {
            var students = await _studentService.GetStudentListAsync();
            var users = await _uow.Users.GetAllAsync();
            var studentItems = students.Where(s => !string.IsNullOrEmpty(s.StudentNumber)).Select(m => new SelectListItem { Value = m.StudentId.ToString(), Text = $"{m.FirstName} {m.LastName} ({m.StudentNumber})" }).ToList();
            var visitorItems = users.Where(u => u.Role != eSysRole.Student).Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.Name} {u.LastName} ({u.StudentNumber})" }).ToList();
            return (studentItems, visitorItems);
        }

        private Visit MapToEntity(VisitViewModel model, User? currentUser, bool isCreate)
        {
            return new Visit
            {
                VisitId = isCreate ? Helper.GenerateGuid() : model.VisitId,
                CompanyId = model.CompanyId,
                PlacementId = model.PlacementId,
                Date = DateTime.Parse(model.Date),
                DurationMinutes = model.DurationMinutes,
                SelectedEmployeeIDs = string.Join(",", model.SelectedIDArray ?? Array.Empty<string>()),
                SelectedIDArray = model.SelectedIDArray,
                HasReport = model.HasReport,
                VisitBy = model.VisitBy,
                Mentor = model.Mentor,
                VisitPurpose = model.VisitPurpose,
                LearnerFeedback = model.LearnerFeedback,
                Report = model.Report,
                ReportFile = model.ReportFile,
                IsActive = true,
                CreatedBy = isCreate ? $"{currentUser?.Name} {currentUser?.LastName}" : null,
                CreatedOn = isCreate ? DateTimeHelper.GetCurrentSastDateTimeOffset().ToString() : null,
                ModifiedBy = !isCreate ? $"{currentUser?.Name} {currentUser?.LastName}" : null,
                ModifiedOn = !isCreate ? Helper.OnGetCurrentDateTime() : null
            };
        }
    }
}
