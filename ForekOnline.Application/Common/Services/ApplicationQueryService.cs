using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ForekOnline.Application.Common.Services
{
    public sealed class ApplicationQueryService : IApplicationQueryService
    {
        private const string DateFormat = "dd/MM/yyyy";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ApplicationQueryService> _logger;

        public ApplicationQueryService(IUnitOfWork unitOfWork, ILogger<ApplicationQueryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> ConvertCourseIdToStringAsync(Guid courseId, CancellationToken ct = default)
        {
            if (courseId == Guid.Empty)
            {
                throw new ArgumentException("Invalid course ID provided.", nameof(courseId));
            }

            var course = await _unitOfWork.Courses.GetAsync(filter: c => c.CourseId == courseId).ConfigureAwait(false);
            if (course is null)
            {
                throw new InvalidOperationException("Course not found.");
            }

            return $"{course.CourseName} ({Helper.GetDisplayName(course.Type)}) {course.NType}";
        }

        public async Task<ApplyViewModel?> GetApplicationForEditAsync(Guid applicationId, CancellationToken ct = default)
        {
            if (applicationId == Guid.Empty)
            {
                return null;
            }

            var application = await _unitOfWork.Applications.GetAsync(
                    filter: a => a.ApplicationId == applicationId,
                    includeProperties: new[] { nameof(Domain.Entities.Application.ApplicantAddress), nameof(Domain.Entities.Application.ApplicantGuardian) })
                .ConfigureAwait(false);

            if (application is null)
            {
                return null;
            }

            return new ApplyViewModel
            {
                ApplicantId = application.ApplicationId,
                ApplicantIDPassFile = application.IDPassFile,
                ApplicantName = application.ApplicantName,
                ApplicantSurname = application.ApplicantSurname,
                ApplicantTitle = application.ApplicantTitle,
                Gender = application.Gender,
                IDNumber = application.IDNumber,
                IDPassDoc = application.IDPassDoc,
                ResidenceDoc = application.ResidenceDoc,

                Cellphone = application.Cellphone,
                Email = application.Email,

                CourseId = application.CourseId,
                Status = application.Status,
                Selection = application.Selection,
                ReferenceNumber = application.ReferenceNumber,
                StudyPermitCategory = application.StudyPermitCategory,
                HighestQualification = application.HighestQualification,
                HighestQualDoc = application.HighestQualDoc,

                GuardianFirstName = application.ApplicantGuardian?.FirstName,
                GuardianLastName = application.ApplicantGuardian?.LastName,
                GuardianCellphone = application.ApplicantGuardian?.Cellphone,
                GuardianId = application.ApplicantGuardian?.GuardianId ?? Guid.Empty,
                GuardianIDDoc = application.ApplicantGuardian?.IDDoc,

                Line1 = application.ApplicantAddress?.Line1,
                StreetName = application.ApplicantAddress?.StreetName,
                AddressId = application.ApplicantAddress?.AddressId ?? Guid.Empty,
                City = application.ApplicantAddress?.City,
                Province = application.ApplicantAddress?.Province,
                PostalCode = application.ApplicantAddress?.PostalCode,

                HighestQualFileUrl = application.HighestQualFileUrl,
                ResidenceFileUrl = application.ResidenceFileUrl,
                IDPassFileUrl = application.IDPassFileUrl
            };
        }

        public async Task<IReadOnlyList<ApplicationsViewModel>> GetApplicationsAsync(CancellationToken ct = default)
        {
            var list = await _unitOfWork.Applications
                .GetAllAsync(includeProperties: new[] { nameof(Domain.Entities.Application.ApplicantAddress), nameof(Domain.Entities.Application.ApplicantGuardian) })
                .ConfigureAwait(false);

            var ordered = list
                .Select(a => new { App = a, Date = TryParseAppDate(a.ReferenceNumber) })
                .OrderByDescending(x => x.Date ?? DateTime.MinValue)
                .Select(x => x.App)
                .ToList();

            var courses = await _unitOfWork.Courses.GetAllAsync().ConfigureAwait(false);
            var courseTextById = courses
                .GroupBy(c => c.CourseId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var c = g.First();
                        return $"{c.CourseName} ({Helper.GetDisplayName(c.Type)}) {c.NType}";
                    });

            var vms = new List<ApplicationsViewModel>(ordered.Count);

            foreach (var item in ordered)
            {
                vms.Add(new ApplicationsViewModel
                {
                    ApplicationId = item.ApplicationId,
                    IDNumber = item.IDNumber ?? "0000000000000",
                    Email = item.Email,
                    Status = item.Status.ToString(),
                    SubmittedDate = TryParseAppDate(item.ReferenceNumber) ?? DateTime.MinValue,
                    Cellphone = item.Cellphone,
                    Course = courseTextById.TryGetValue(item.CourseId, out var courseText)
                        ? courseText
                        : "Course not found",
                    Names = $"{item.ApplicantName} {item.ApplicantSurname}",
                    Reference = item.ReferenceNumber ?? string.Empty,
                    IDPassDoc = item.IDPassDoc ?? "Id Error",
                    QualificationDoc = item.HighestQualDoc ?? "Qualification Error",
                    ApplicantGuardian = item.ApplicantGuardian,
                    ApplicantAddress = item.ApplicantAddress
                });
            }

            return vms;
        }

        public async Task<ApplicationsDashboardViewModel> GetDashboardData()
        {
            var apps = await _unitOfWork.Applications.GetAllAsync().ConfigureAwait(false);
            var courses = await _unitOfWork.Courses.GetAllAsync().ConfigureAwait(false);

            var now = DateTime.UtcNow;
            var withDate = apps
                .Select(a => TryParseAppDate(a.ReferenceNumber))
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .ToList();

            var totalForMonth = withDate.Count(d => d.Month == now.Month && d.Year == now.Year);

            var genderCounts = apps.Aggregate(
                new Dictionary<string, int> { ["Male"] = 0, ["Female"] = 0 },
                (acc, a) =>
                {
                    if (a.ApplicantTitle == ForekOnline.Domain.Enums.EnumRegistry.eTitle.Mr)
                    {
                        acc["Male"]++;
                    }
                    else
                    {
                        acc["Female"]++;
                    }

                    return acc;
                });
            var trend = withDate
               .GroupBy(d => d.Month)
               .OrderBy(g => g.Key)
               .ToDictionary(
                   g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                   g => g.Count());

            var topCourseIds = apps
                .GroupBy(a => a.CourseId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            var courseNameById = courses
                .GroupBy(c => c.CourseId)
                .ToDictionary(g => g.Key, g => g.First().CourseName);

            var topNames = topCourseIds
                .Select(id => courseNameById.TryGetValue(id, out var name) ? name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            return new ApplicationsDashboardViewModel
            {
                TotalApplicationsForCurrentMonth = totalForMonth,
                GenderDistribution = genderCounts,
                MonthlyApplicationsTrend = trend,
                Top5ProgramsAppliedFor = topNames,
                TotalMales = genderCounts.TryGetValue("Male", out var m) ? m : 0,
                TotalFemales = genderCounts.TryGetValue("Female", out var f) ? f : 0,
                MostAppliedProgram = GetMostAppliedProgramName(apps, courseNameById)
            };
        }

        public async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync(CancellationToken ct = default)
        {
            var apps = await _unitOfWork.Applications.GetAllAsync().ConfigureAwait(false);

            var today = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime.Date;

            var withDate = apps
                .Select(a => TryParseAppDate(a.ReferenceNumber))
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .ToList();

            int totalToday = withDate.Count(d => d.Date == today);
            int recent = withDate.Count(d => (today - d.Date).TotalDays <= 2);

            var topCourseId = apps
                .GroupBy(a => a.CourseId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            string top;
            try
            {
                top = topCourseId == Guid.Empty
                    ? "N/A"
                    : await ConvertCourseIdToStringAsync(topCourseId, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve top course name for dashboard summary.");
                top = "N/A";
            }

            return new DashboardSummaryViewModel
            {
                Total = totalToday,
                Recent = recent,
                Top = top
            };

        }

        #region Private Methods

        private static DateTime? TryParseAppDate(string? referenceNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(referenceNumber))
                {
                    return null;
                }

                string extracted = Helper.ExtractDateFromReference(referenceNumber);
                if (string.IsNullOrWhiteSpace(extracted) || extracted == "Invalid Date")
                {
                    return null;
                }

                return DateTime.TryParseExact(extracted, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
                    ? parsed
                    : null;
            }
            catch
            {
                return null;
            }
        }

        private static string GetMostAppliedProgramName(IReadOnlyList<Domain.Entities.Application> apps, IReadOnlyDictionary<Guid, string> courseNameById)
        {
            var topCourseId = apps
                .GroupBy(a => a.CourseId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return topCourseId != Guid.Empty && courseNameById.TryGetValue(topCourseId, out var name)
                ? name
                : "N/A";
        }

        #endregion
    }
}
