// <copyright file="UserSessionCleanupHostedService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    26/01/2026 21:02:27 PM
// Purpose:         Defines the UserSessionCleanupHostedService class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
#endregion

namespace ForekOnline.Infrastructure.HostedServices
{
    /// <summary>
    /// Provides a hosted background service that periodically cleans up inactive user sessions by marking them as
    /// terminated after a defined period of inactivity.
    /// </summary>
    /// <remarks>This service runs in the background as part of the application's hosting environment and
    /// executes cleanup operations at regular intervals. It is intended to help maintain accurate session state by
    /// automatically closing sessions that have not recorded activity within the configured timeout. The cleanup
    /// process is resilient to errors and logs any failures or actions performed. This service is typically registered
    /// as a singleton in the application's dependency injection container.</remarks>
    public sealed class UserSessionCleanupHostedService : BackgroundService
    {
        #region Fields
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan InactivityTimeout = TimeSpan.FromMinutes(20);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UserSessionCleanupHostedService> _logger;

        #endregion

        /// <summary>
        /// Initializes a new instance of the UserSessionCleanupHostedService class with the specified service scope
        /// factory and logger.
        /// </summary>
        /// <param name="scopeFactory">The factory used to create service scopes for dependency resolution during session cleanup operations.
        /// Cannot be null.</param>
        /// <param name="logger">The logger used to record diagnostic and operational information for the hosted service. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="scopeFactory"/> or <paramref name="logger"/> is null.</exception>
        public UserSessionCleanupHostedService(IServiceScopeFactory scopeFactory, ILogger<UserSessionCleanupHostedService> logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executes the background user session cleanup operation until cancellation is requested.
        /// </summary>
        /// <remarks>This method runs continuously in the background, periodically invoking session
        /// cleanup until the provided cancellation token signals a request to stop. Errors encountered during cleanup
        /// are logged but do not interrupt the execution loop.</remarks>
        /// <param name="stoppingToken">A cancellation token that can be used to request termination of the background operation.</param>
        /// <returns>A task that represents the asynchronous execution of the cleanup operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupAsync(stoppingToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "User session cleanup failed.");
                }

                await Task.Delay(CleanupInterval, stoppingToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously closes stale user sessions that have been inactive beyond the configured timeout period.
        /// </summary>
        /// <remarks>Stale sessions are identified as those with no recorded logout time and a last
        /// activity timestamp older than the inactivity timeout. The method updates session records to reflect abnormal
        /// termination and logs the number of sessions closed.</remarks>
        /// <param name="ct">A cancellation token that can be used to cancel the cleanup operation.</param>
        /// <returns>A task that represents the asynchronous cleanup operation.</returns>
        private async Task CleanupAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var cutoff = DateTimeOffset.UtcNow.Subtract(InactivityTimeout);

            var stale = await uow.UserLoginHistories.GetAllAsync(
                filter: h => h.LogoutTimeUtc == null && (h.LastActivityUtc == null || h.LastActivityUtc < cutoff),
                asNoTracking: false,
                cancellationToken: ct);

            if (stale.Count == 0)
            {
                return;
            }

            foreach (UserLoginHistory row in stale)
            {
                row.LogoutTimeUtc = DateTimeOffset.UtcNow;
                row.IsCurrentSession = false;
                row.LogoutReason = "AbnormalTermination";
                row.ForceLogoutPerformed = false;
            }

            await uow.SaveAsync().ConfigureAwait(false);

            _logger.LogInformation("User session cleanup closed {Count} stale session(s).", stale.Count);
        }
    }
}
