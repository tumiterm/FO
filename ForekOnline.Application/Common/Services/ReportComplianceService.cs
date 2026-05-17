// <copyright file="ReportComplianceService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    18/04/2026
// Purpose:         Implements report submission checks and compliance calculation

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Handles report submission eligibility checks and compliance calculations.
    /// </summary>
    public sealed class ReportComplianceService : IReportComplianceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHelperService _helperService;
        private readonly ILogger<ReportComplianceService> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The late-report threshold percentage. If compliance drops below this, the user is non-compliant.
        /// </summary>
        private const double NonCompliantThreshold = 70.0;

        /// <summary>
        /// The partially-compliant threshold. Between this and 100% = partially compliant.
        /// </summary>
        private const double PartiallyCompliantThreshold = 90.0;

        public ReportComplianceService(
            IUnitOfWork unitOfWork,
            IHelperService helperService,
            ILogger<ReportComplianceService> logger,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Gets the number of grace weeks at the start of the year (school hasn't started yet).
        /// Configurable via "ReportCompliance:GraceWeeks" in appsettings. Defaults to 2.
        /// </summary>
        private int GraceWeeks => _configuration.GetValue("ReportCompliance:GraceWeeks", 2);

        /// <summary>
        /// Returns the effective compliance start date for the given year,
        /// accounting for the grace period (e.g. school starts week 2).
        /// </summary>
        private DateTime GetComplianceStartDate(int year)
        {
            var yearStart = new DateTime(year, 1, 1);
            return yearStart.AddDays(GraceWeeks * 7);
        }

        /// <inheritdoc />
        public async Task<ReportSubmissionCheckResult> CheckSubmissionAsync(string idPass, ReportType reportType, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(idPass, nameof(idPass));

            var now = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            var currentPeriod = GetPeriodBounds(reportType, now);

            var allReports = await _unitOfWork.Reports.GetAllAsync(
                r => r.IdPass == idPass && r.ReportType == reportType,
                cancellationToken: ct);

            var reportDates = allReports
                .Select(r => new
                {
                    Report = r,
                    ParsedDate = _helperService.ConvertToDateTimeNoReference(r.CreatedOn)
                })
                .Where(x => x.ParsedDate.HasValue)
                .Select(x => new
                {
                    x.Report,
                    Date = x.ParsedDate!.Value,
                    PeriodStart = x.Report.IntendedPeriodStart ?? GetPeriodBounds(reportType, x.ParsedDate!.Value).Start
                })
                .OrderByDescending(x => x.Date)
                .ToList();

            bool currentPeriodCovered = reportDates.Any(x =>
                x.PeriodStart >= currentPeriod.Start && x.PeriodStart < currentPeriod.End);

            int lateCount = allReports.Count(r => r.IsLateSubmission);

            var coveredPeriodStarts = reportDates
                .Select(x => x.PeriodStart)
                .ToHashSet();

            var missedPeriods = FindMissedPeriods(reportType, coveredPeriodStarts, now);

            if (!currentPeriodCovered)
            {
                return new ReportSubmissionCheckResult
                {
                    IsAllowed = true,
                    CurrentPeriodAlreadyCovered = false,
                    WouldBeLate = false,
                    HasMissedPeriods = missedPeriods.Count > 0,
                    IntendedPeriodStart = currentPeriod.Start,
                    IntendedPeriodEnd = currentPeriod.End,
                    IntendedPeriodLabel = FormatPeriodLabel(reportType, currentPeriod.Start),
                    ReportType = reportType,
                    Message = missedPeriods.Count > 0
                        ? $"You are submitting your {reportType} report for {FormatPeriodLabel(reportType, currentPeriod.Start)}. " +
                          $"You also have {missedPeriods.Count} missed period(s) — you can choose to submit for a missed period instead."
                        : $"You are submitting your {reportType} report for {FormatPeriodLabel(reportType, currentPeriod.Start)}.",
                    ExistingLateCount = lateCount,
                    MissedPeriods = missedPeriods
                };
            }

            if (missedPeriods.Count == 0)
            {
                return new ReportSubmissionCheckResult
                {
                    IsAllowed = true,
                    CurrentPeriodAlreadyCovered = true,
                    WouldBeLate = true,
                    HasMissedPeriods = false,
                    IntendedPeriodStart = currentPeriod.Start,
                    IntendedPeriodEnd = currentPeriod.End,
                    IntendedPeriodLabel = FormatPeriodLabel(reportType, currentPeriod.Start),
                    ReportType = reportType,
                    Message = $"You have already submitted a {reportType} report for {FormatPeriodLabel(reportType, currentPeriod.Start)}, " +
                              "and all previous periods are also covered. This will be flagged as an additional late submission.",
                    ExistingLateCount = lateCount,
                    MissedPeriods = missedPeriods
                };
            }

            var targetMissed = missedPeriods[0];

            return new ReportSubmissionCheckResult
            {
                IsAllowed = true,
                CurrentPeriodAlreadyCovered = true,
                WouldBeLate = true,
                HasMissedPeriods = true,
                IntendedPeriodStart = targetMissed.PeriodStart,
                IntendedPeriodEnd = targetMissed.PeriodEnd,
                IntendedPeriodLabel = targetMissed.Label,
                ReportType = reportType,
                Message = $"You have already submitted a {reportType} report for the current period. " +
                          $"This submission will be recorded as a late report for {targetMissed.Label} " +
                          "and flagged accordingly.",
                ExistingLateCount = lateCount,
                MissedPeriods = missedPeriods
            };
        }

        /// <inheritdoc />
        public async Task<UserReportCompliance> CalculateComplianceAsync(string idPass, string userName, int lookbackMonths = 12, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(idPass, nameof(idPass));

            var now = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            var complianceStart = GetComplianceStartDate(now.Year);

            if (now < complianceStart)
            {
                return new UserReportCompliance
                {
                    IdPass = idPass,
                    UserDisplayName = userName,
                    Status = eComplianceStatus.Compliant,
                    CompliancePercentage = 100.0,
                    OnTimeCount = 0,
                    LateCount = 0,
                    MissedCount = 0,
                    TotalExpected = 0,
                    TotalSubmitted = 0,
                    ByType = [],
                    ComplianceYear = now.Year,
                    GraceWeeks = GraceWeeks,
                    ComplianceStartDate = complianceStart,
                    MissedPeriodsByType = new Dictionary<ReportType, IReadOnlyList<MissedPeriod>>()
                };
            }

            var allReports = await _unitOfWork.Reports.GetAllAsync(
                r => r.IdPass == idPass,
                cancellationToken: ct);

            var reportsByType = allReports
                .GroupBy(r => r.ReportType)
                .ToDictionary(g => g.Key, g => g.ToList());

            var typeBreakdowns = new List<ReportTypeCompliance>();
            var missedPeriodsByType = new Dictionary<ReportType, IReadOnlyList<MissedPeriod>>();

            int totalExpected = 0;
            int totalOnTime = 0;
            int totalLate = 0;
            int totalMissed = 0;

            foreach (var reportType in new[] { ReportType.Weekly, ReportType.Monthly })
            {
                var reportsOfType = reportsByType.GetValueOrDefault(reportType, []);

                int expected = CountExpectedPeriods(reportType, complianceStart, now);

                var reportsInYear = reportsOfType
                    .Where(r =>
                    {
                        var parsed = _helperService.ConvertToDateTimeNoReference(r.CreatedOn);
                        return parsed.HasValue && parsed.Value >= complianceStart;
                    })
                    .ToList();

                int onTimeInYear = reportsInYear.Count(r => !r.IsLateSubmission);
                int lateInYear = reportsInYear.Count(r => r.IsLateSubmission);
                int submittedInYear = reportsInYear.Count;

                var coveredPeriodStarts = reportsInYear
                    .Select(r => r.IntendedPeriodStart ?? GetParsedPeriodStart(r, reportType))
                    .Where(d => d >= complianceStart)
                    .ToHashSet();

                var missedPeriodsForType = FindMissedPeriodsInYear(reportType, coveredPeriodStarts, complianceStart, now);
                missedPeriodsByType[reportType] = missedPeriodsForType;

                typeBreakdowns.Add(new ReportTypeCompliance
                {
                    ReportType = reportType,
                    OnTime = onTimeInYear,
                    Late = lateInYear,
                    Missed = missedPeriodsForType.Count,
                    Expected = expected
                });

                totalExpected += expected;
                totalOnTime += onTimeInYear;
                totalLate += lateInYear;
                totalMissed += missedPeriodsForType.Count;
            }

            if (reportsByType.TryGetValue(ReportType.Annual, out var annualReports))
            {
                var annualInYear = annualReports
                    .Where(r =>
                    {
                        var parsed = _helperService.ConvertToDateTimeNoReference(r.CreatedOn);
                        return parsed.HasValue && parsed.Value >= complianceStart;
                    })
                    .ToList();

                typeBreakdowns.Add(new ReportTypeCompliance
                {
                    ReportType = ReportType.Annual,
                    OnTime = annualInYear.Count(r => !r.IsLateSubmission),
                    Late = annualInYear.Count(r => r.IsLateSubmission),
                    Missed = 0,
                    Expected = annualInYear.Count > 0 ? 1 : 0
                });
            }

            int totalSubmitted = totalOnTime + totalLate;
            double compliancePct = totalExpected == 0
                ? 100.0
                : Math.Round((totalOnTime / (double)totalExpected) * 100, 1);

            var status = compliancePct >= PartiallyCompliantThreshold
                ? eComplianceStatus.Compliant
                : compliancePct >= NonCompliantThreshold
                    ? eComplianceStatus.PartiallyCompliant
                    : eComplianceStatus.NonCompliant;

            return new UserReportCompliance
            {
                IdPass = idPass,
                UserDisplayName = userName,
                Status = status,
                CompliancePercentage = compliancePct,
                OnTimeCount = totalOnTime,
                LateCount = totalLate,
                MissedCount = totalMissed,
                TotalExpected = totalExpected,
                TotalSubmitted = totalSubmitted,
                ByType = typeBreakdowns,
                ComplianceYear = now.Year,
                GraceWeeks = GraceWeeks,
                ComplianceStartDate = complianceStart,
                MissedPeriodsByType = missedPeriodsByType
            };
        }

        /// <inheritdoc />
        public void ApplyLateSubmissionFields(Report report, ReportSubmissionCheckResult checkResult)
        {
            ArgumentNullException.ThrowIfNull(report, nameof(report));
            ArgumentNullException.ThrowIfNull(checkResult, nameof(checkResult));

            report.IntendedPeriodStart = checkResult.IntendedPeriodStart;
            report.IntendedPeriodEnd = checkResult.IntendedPeriodEnd;
            report.IntendedPeriodLabel = checkResult.IntendedPeriodLabel;

            if (checkResult.WouldBeLate)
            {
                report.IsLateSubmission = true;
                report.LateAcknowledgedUtc = DateTime.UtcNow;
            }
            else
            {
                report.IsLateSubmission = false;
                report.LateAcknowledgedUtc = null;
            }
        }

        #region Period Calculation Helpers

        private DateTime GetParsedPeriodStart(Report report, ReportType reportType)
        {
            var parsed = _helperService.ConvertToDateTimeNoReference(report.CreatedOn);
            return parsed.HasValue
                ? GetPeriodBounds(reportType, parsed.Value).Start
                : DateTime.MinValue;
        }

        private static List<MissedPeriod> FindMissedPeriodsInYear(ReportType reportType, ICollection<DateTime> coveredPeriodStarts, DateTime yearStart, DateTime now)
        {
            var missed = new List<MissedPeriod>();
            var cursor = yearStart;

            while (cursor < now)
            {
                var bounds = GetPeriodBounds(reportType, cursor);

                if (bounds.End > now)
                    break;

                bool isCovered = coveredPeriodStarts.Any(s => s >= bounds.Start && s < bounds.End);

                if (!isCovered)
                {
                    missed.Add(new MissedPeriod
                    {
                        PeriodStart = bounds.Start,
                        PeriodEnd = bounds.End,
                        Label = FormatPeriodLabel(reportType, bounds.Start)
                    });
                }

                cursor = reportType switch
                {
                    ReportType.Weekly => bounds.Start.AddDays(7),
                    ReportType.Monthly => bounds.Start.AddMonths(1),
                    ReportType.Annual => bounds.Start.AddYears(1),
                    _ => bounds.Start.AddMonths(1)
                };
            }

            return missed;
        }

        private static (DateTime Start, DateTime End) GetPeriodBounds(ReportType reportType, DateTime date)
        {
            return reportType switch
            {
                ReportType.Weekly => GetIsoWeekBounds(date),
                ReportType.Monthly => (
                    new DateTime(date.Year, date.Month, 1),
                    new DateTime(date.Year, date.Month, 1).AddMonths(1)),
                ReportType.Annual => (
                    new DateTime(date.Year, 1, 1),
                    new DateTime(date.Year + 1, 1, 1)),
                _ => throw new ArgumentOutOfRangeException(nameof(reportType))
            };
        }

        private static (DateTime Start, DateTime End) GetIsoWeekBounds(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            var weekStart = date.Date.AddDays(-diff);
            return (weekStart, weekStart.AddDays(7));
        }

        private static int GetIsoWeekNumber(DateTime date)
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private static string FormatPeriodLabel(ReportType reportType, DateTime periodStart)
        {
            return reportType switch
            {
                ReportType.Weekly => $"Week {GetIsoWeekNumber(periodStart)} of {periodStart.Year}",
                ReportType.Monthly => periodStart.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                ReportType.Annual => periodStart.Year.ToString(),
                _ => periodStart.ToString("dd MMM yyyy")
            };
        }

        private List<MissedPeriod> FindMissedPeriods(ReportType reportType, ICollection<DateTime> coveredPeriodStarts, DateTime now, int maxLookback = 12)
        {
            var missed = new List<MissedPeriod>();
            var current = GetPreviousPeriodStart(reportType, now);

            for (int i = 0; i < maxLookback && current >= GetEarliestLookback(reportType, now, maxLookback); i++)
            {
                var bounds = GetPeriodBounds(reportType, current);

                bool isCovered = coveredPeriodStarts.Any(s => s >= bounds.Start && s < bounds.End);

                if (!isCovered)
                {
                    missed.Add(new MissedPeriod
                    {
                        PeriodStart = bounds.Start,
                        PeriodEnd = bounds.End,
                        Label = FormatPeriodLabel(reportType, bounds.Start)
                    });
                }

                current = GetPreviousPeriodStart(reportType, current.AddDays(-1));
            }

            return missed;
        }

        private static DateTime GetPreviousPeriodStart(ReportType reportType, DateTime date)
        {
            var bounds = GetPeriodBounds(reportType, date);
            return reportType switch
            {
                ReportType.Weekly => bounds.Start.AddDays(-7),
                ReportType.Monthly => bounds.Start.AddMonths(-1),
                ReportType.Annual => bounds.Start.AddYears(-1),
                _ => bounds.Start.AddMonths(-1)
            };
        }

        private static DateTime GetEarliestLookback(ReportType reportType, DateTime now, int maxPeriods)
        {
            return reportType switch
            {
                ReportType.Weekly => now.AddDays(-7 * maxPeriods),
                ReportType.Monthly => now.AddMonths(-maxPeriods),
                ReportType.Annual => now.AddYears(-maxPeriods),
                _ => now.AddMonths(-maxPeriods)
            };
        }

        private static int CountExpectedPeriods(ReportType reportType, DateTime from, DateTime to)
        {
            return reportType switch
            {
                ReportType.Weekly => (int)Math.Ceiling((to - from).TotalDays / 7),
                ReportType.Monthly => ((to.Year - from.Year) * 12) + to.Month - from.Month,
                ReportType.Annual => to.Year - from.Year,
                _ => 0
            };
        }

        #endregion
    }
}