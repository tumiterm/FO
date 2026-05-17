// <copyright file="StudentListCacheWarmupService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    23/03/2025 00:21 AM
// Purpose:         Defines the StudentListCacheWarmupService.

#region Usings
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
#endregion


namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// A background service that handles cache warmup and periodic refresh for student list data.
    /// </summary>
    public class StudentListCacheWarmupService : BackgroundService
    {
        #region Fields
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StudentListCacheWarmupService> _logger;
        private static TaskCompletionSource<bool> _cacheWarmupCompleted = new TaskCompletionSource<bool>();
        #endregion

        /// <summary>
        /// Initializes a new instance of the StudentListCacheWarmupService class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency injection.</param>
        /// <param name="logger">The logger instance for logging cache operations.</param>
        public StudentListCacheWarmupService(IServiceProvider serviceProvider, ILogger<StudentListCacheWarmupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Provides a task that completes when the initial cache warmup is finished.
        /// </summary>
        /// <returns>A task that represents the completion of the initial cache warmup.</returns>
        public static Task WaitForCacheWarmupAsync()
        {
            return _cacheWarmupCompleted.Task;
        }

        /// <summary>
        /// Executes the background service's main logic asynchronously.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token to stop the service.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WarmupCacheAsync(stoppingToken);
        }

        /// <summary>
        /// Performs the cache warmup and periodic refresh operations.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token to stop the operation.</param>
        /// <returns>A task representing the asynchronous cache operations.</returns>
        private async Task WarmupCacheAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();

                _logger.LogInformation("Starting initial cache warmup...");

                await studentService.PrepopulateCacheAsync();

                _logger.LogInformation("Initial cache warmup completed...");

                _cacheWarmupCompleted.TrySetResult(true); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during initial cache warmup...");
                _cacheWarmupCompleted.TrySetException(ex); 
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);

                    using var scope = _serviceProvider.CreateScope();

                    var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();

                    _logger.LogInformation("Refreshing cache...");

                    await studentService.PrepopulateCacheAsync();

                    _logger.LogInformation("Cache refresh completed...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing cache...");
                }
            }
        }
    }
}
