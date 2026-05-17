// <copyright file="LoginHistoryService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/04/2026 10:00 AM
// Purpose:         Implements the LoginHistoryService

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Logging;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides login history operations including KPI calculation, session management,
    /// and data export. All logic is centralised here to keep controllers thin.
    /// </summary>
    public class LoginHistoryService : ILoginHistoryService
    {
        #region Privates
        private readonly ILogger<LoginHistoryService> _logger;
        private readonly IUnitOfWork _context;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHistoryService"/> class.
        /// </summary>
        public LoginHistoryService(ILogger<LoginHistoryService> logger, IUnitOfWork context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Public

        /// <inheritdoc/>
        public async Task<LoginHistoryDashboardViewModel> GetDashboardAsync(int take = 200, string filter = "all", DateOnly? from = null, DateOnly? to = null,  CancellationToken cancellationToken = default)
        {
            try
            {
                take = take <= 0 ? 200 : Math.Min(take, 2000);
                filter = (filter ?? "all").Trim().ToLowerInvariant();

                var allSessions = await _context.UserLoginHistories.GetAllAsync(
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                var now = DateTimeHelper.GetCurrentSastDateTimeOffset();
                var todayDate = DateOnly.FromDateTime(now.DateTime);
                var yesterdayDate = todayDate.AddDays(-1);

                int totalSessions = allSessions.Count;
                int activeNow = allSessions.Count(s => !s.LogoutTimeUtc.HasValue);
                int closedCount = allSessions.Count(s => s.LogoutTimeUtc.HasValue);
                int abnormalOrForced = allSessions.Count(s =>
                    s.ForceLogoutPerformed == true ||
                    string.Equals(s.LogoutReason, "AbnormalTermination", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(s.LogoutReason, "ForceLogoutAdmin", StringComparison.OrdinalIgnoreCase));

                var todaySessions = allSessions
                    .Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) == todayDate)
                    .ToList();

                var yesterdaySessions = allSessions
                    .Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) == yesterdayDate)
                    .ToList();

                var avgDurationToday = ComputeAverageDuration(todaySessions);
                var avgDurationYesterday = ComputeAverageDuration(yesterdaySessions);

                double avgChangePercent = avgDurationYesterday.TotalMinutes > 0
                    ? ((avgDurationToday.TotalMinutes - avgDurationYesterday.TotalMinutes) / avgDurationYesterday.TotalMinutes) * 100
                    : 0;

                int peakConcurrentToday = ComputePeakConcurrent(todaySessions);

                int recentLogins = allSessions.Count(s =>
                    (DateTimeOffset.UtcNow - s.LoginTimeUtc).TotalMinutes <= 5);

                IEnumerable<UserLoginHistory> filtered = allSessions;

                switch (filter)
                {
                    case "active":
                        filtered = filtered.Where(s => !s.LogoutTimeUtc.HasValue);
                        break;
                    case "abnormal":
                        filtered = filtered.Where(s =>
                            s.ForceLogoutPerformed == true ||
                            string.Equals(s.LogoutReason, "AbnormalTermination", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(s.LogoutReason, "ForceLogoutAdmin", StringComparison.OrdinalIgnoreCase));
                        break;
                    case "thisweek":
                        var startOfWeek = todayDate.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                        filtered = filtered.Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) >= startOfWeek);
                        break;
                    case "last7":
                        filtered = filtered.Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) >= todayDate.AddDays(-7));
                        break;
                    case "last30":
                        filtered = filtered.Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) >= todayDate.AddDays(-30));
                        break;
                }

                if (from.HasValue)
                {
                    filtered = filtered.Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) >= from.Value);
                }

                if (to.HasValue)
                {
                    filtered = filtered.Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) <= to.Value);
                }

                var orderedSessions = filtered
                    .OrderBy(h => h.LogoutTimeUtc.HasValue)
                    .ThenByDescending(h => h.LoginTimeUtc)
                    .Take(take)
                    .ToList();

                var rows = await MapSessionsToRowsAsync(orderedSessions, cancellationToken);

                var dashboard = new LoginHistoryDashboardViewModel
                {
                    TotalSessions = totalSessions,
                    ActiveNow = activeNow,
                    ClosedCount = closedCount,
                    AbnormalOrForcedCount = abnormalOrForced,
                    AvgDurationToday = avgDurationToday,
                    AvgDurationYesterday = avgDurationYesterday,
                    AvgDurationChangePercent = Math.Round(avgChangePercent, 1),
                    PeakConcurrentToday = peakConcurrentToday,
                    RecentLoginsLast5Min = recentLogins,
                    Take = take,
                    ActiveFilter = filter,
                    From = from,
                    To = to,
                    Rows = rows
                };

                _logger.LogInformation(
                    "Login history dashboard built: {Total} total, {Active} active, {Rows} rows returned at {Time}",
                    totalSessions, activeNow, rows.Count, DateTime.UtcNow);

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while building the login history dashboard at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<MyLoginHistoryViewModel> GetMyLoginHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            try
            {
                var now = DateTimeHelper.GetCurrentSastDateTimeOffset();
                var cutoff90 = DateTimeOffset.UtcNow.AddDays(-90);
                var cutoff30 = DateTimeOffset.UtcNow.AddDays(-30);

                var userSessions = await _context.UserLoginHistories.GetAllAsync(
                    filter: h => h.UserId == userId && h.LoginTimeUtc >= cutoff90,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                var user = await _context.Users.GetAsync(
                    filter: u => u.Id == userId,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                var displayName = user is null ? "Unknown User" : $"{user.Name} {user.LastName}";

                int activeSessions = userSessions.Count(s => !s.LogoutTimeUtc.HasValue);
                int totalLogins30 = userSessions.Count(s => s.LoginTimeUtc >= cutoff30);

                var completedSessions30 = userSessions
                    .Where(s => s.LogoutTimeUtc.HasValue && s.LoginTimeUtc >= cutoff30)
                    .ToList();

                var avgDuration = ComputeAverageDuration(completedSessions30);

                var orderedSessions = userSessions
                    .OrderBy(h => h.LogoutTimeUtc.HasValue)
                    .ThenByDescending(h => h.LoginTimeUtc)
                    .ToList();

                var rows = orderedSessions.Select(s => new LoginHistoryRowViewModel
                {
                    Session = s,
                    UserId = userId,
                    DisplayName = displayName,
                    Email = user?.Username,
                    Role = user?.Role
                }).ToList().AsReadOnly();

                _logger.LogInformation(
                    "My Login History built for user {UserId}: {Count} sessions at {Time}",
                    userId, rows.Count, DateTime.UtcNow);

                return new MyLoginHistoryViewModel
                {
                    ActiveSessionCount = activeSessions,
                    TotalLogins30Days = totalLogins30,
                    AvgDuration = avgDuration,
                    DisplayName = displayName,
                    Rows = rows
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building My Login History for user {UserId} at {Time}", userId, DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ForceLogoutSessionAsync(string sessionKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                throw new ArgumentException("Session key cannot be null or empty.", nameof(sessionKey));
            }

            try
            {
                var row = await _context.UserLoginHistories.GetAsync(
                    h => h.SessionKey == sessionKey && h.LogoutTimeUtc == null,
                    asNoTracking: false,
                    cancellationToken: cancellationToken);

                if (row is null)
                {
                    _logger.LogWarning("Force-logout attempted for session {SessionKey} but session not found or already closed at {Time}",
                        sessionKey, DateTime.UtcNow);
                    return false;
                }

                var now = DateTimeHelper.GetCurrentSastDateTimeOffset();

                row.LogoutTimeUtc = now;
                row.LastActivityUtc = now;
                row.IsCurrentSession = false;
                row.ForceLogoutPerformed = true;
                row.LogoutReason = "ForceLogoutAdmin";

                await _context.SaveAsync();

                _logger.LogInformation(
                    "Session {SessionKey} for user {UserId} force-closed at {Time}",
                    sessionKey, row.UserId, DateTime.UtcNow);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error force-closing session {SessionKey} at {Time}", sessionKey, DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> BulkForceLogoutAsync(IEnumerable<string> sessionKeys, CancellationToken cancellationToken = default)
        {
            if (sessionKeys is null)
            {
                throw new ArgumentNullException(nameof(sessionKeys));
            }

            var keySet = sessionKeys.Where(k => !string.IsNullOrWhiteSpace(k)).ToHashSet();

            if (keySet.Count == 0)
            {
                return 0;
            }

            try
            {
                var activeSessions = await _context.UserLoginHistories.GetAllAsync(
                    filter: h => keySet.Contains(h.SessionKey!) && h.LogoutTimeUtc == null,
                    asNoTracking: false,
                    cancellationToken: cancellationToken);

                var now = DateTimeHelper.GetCurrentSastDateTimeOffset();
                int closedCount = 0;

                foreach (var row in activeSessions)
                {
                    row.LogoutTimeUtc = now;
                    row.LastActivityUtc = now;
                    row.IsCurrentSession = false;
                    row.ForceLogoutPerformed = true;
                    row.LogoutReason = "ForceLogoutAdmin";
                    closedCount++;
                }

                if (closedCount > 0)
                {
                    await _context.SaveAsync();
                }

                _logger.LogInformation(
                    "Bulk force-logout completed: {Closed}/{Requested} sessions closed at {Time}",
                    closedCount, keySet.Count, DateTime.UtcNow);

                return closedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk force-logout at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetRecentLoginCountAsync(int minutes = 5, CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = DateTimeOffset.UtcNow.AddMinutes(-minutes);

                var recent = await _context.UserLoginHistories.GetAllAsync(
                    filter: h => h.LoginTimeUtc >= cutoff,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                return recent.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent login count at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<LoginHistoryRowViewModel>> GetExportDataAsync(DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var allSessions = await _context.UserLoginHistories.GetAllAsync(
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                IEnumerable<UserLoginHistory> filtered = allSessions;

                if (from.HasValue)
                {
                    filtered = filtered.Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) >= from.Value);
                }

                if (to.HasValue)
                {
                    filtered = filtered.Where(s => DateOnly.FromDateTime(s.LoginTimeUtc.UtcDateTime) <= to.Value);
                }

                var ordered = filtered
                    .OrderByDescending(h => h.LoginTimeUtc)
                    .ToList();

                var rows = await MapSessionsToRowsAsync(ordered, cancellationToken);

                _logger.LogInformation("Export data prepared: {Count} rows at {Time}", rows.Count, DateTime.UtcNow);

                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing export data at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Maps a list of <see cref="UserLoginHistory"/> entities to <see cref="LoginHistoryRowViewModel"/> rows,
        /// resolving user details in a single batched query.
        /// </summary>
        private async Task<IReadOnlyList<LoginHistoryRowViewModel>> MapSessionsToRowsAsync(IList<UserLoginHistory> sessions, CancellationToken cancellationToken)
        {
            var userIds = sessions
                .Select(s => s.UserId)
                .Distinct()
                .ToHashSet();

            var users = await _context.Users.GetAllAsync(
                filter: u => userIds.Contains(u.Id),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var userMap = users.ToDictionary(u => u.Id);

            return sessions
                .Select(s =>
                {
                    userMap.TryGetValue(s.UserId, out var user);
                    return new LoginHistoryRowViewModel
                    {
                        Session = s,
                        UserId = s.UserId,
                        DisplayName = user is null ? "Unknown User" : $"{user.Name} {user.LastName}",
                        Email = user?.Username,
                        Role = user?.Role
                    };
                })
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Computes the average session duration for a collection of sessions.
        /// Only completed sessions (with a logout time) are considered.
        /// </summary>
        private static TimeSpan ComputeAverageDuration(IList<UserLoginHistory> sessions)
        {
            var completed = sessions.Where(s => s.LogoutTimeUtc.HasValue).ToList();

            if (completed.Count == 0)
            {
                return TimeSpan.Zero;
            }

            var totalTicks = completed.Sum(s => s.SessionDuration.Ticks);
            return TimeSpan.FromTicks(totalTicks / completed.Count);
        }

        /// <summary>
        /// Computes the peak number of concurrent sessions for a given day.
        /// Uses an event-based sweep-line algorithm: +1 at login, -1 at logout/now.
        /// </summary>
        private static int ComputePeakConcurrent(IList<UserLoginHistory> sessions)
        {
            if (sessions.Count == 0)
            {
                return 0;
            }

            var events = new List<(DateTimeOffset Time, int Delta)>();

            foreach (var s in sessions)
            {
                events.Add((s.LoginTimeUtc, +1));
                events.Add((s.LogoutTimeUtc ?? DateTimeOffset.UtcNow, -1));
            }

            events.Sort((a, b) => a.Time.CompareTo(b.Time));

            int current = 0;
            int peak = 0;

            foreach (var (_, delta) in events)
            {
                current += delta;
                if (current > peak) peak = current;
            }

            return peak;
        }

        #endregion
    }
}