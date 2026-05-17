using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace ForekOnline.Application.Common.Jobs
{
    public sealed class BackgroundJobQueueDrainJob
    {
        private readonly ILogger<BackgroundJobQueueDrainJob> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IReadOnlyDictionary<string, IBackgroundJobHandler> _handlersByType;

        public BackgroundJobQueueDrainJob(ILogger<BackgroundJobQueueDrainJob> logger, IUnitOfWork uow, IEnumerable<IBackgroundJobHandler> handlers)
        {
            _logger = logger;
            _uow = uow;
            _handlersByType = handlers
                .GroupBy(h => h.JobType, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        }

        [Queue("onlineapps")]
        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 10, 60, 300, 1800, 7200 })]
        public async Task DrainAsync(string queue, CancellationToken ct = default)
        {
            var workerId = Environment.MachineName;
            _logger.LogInformation("Drain started. Queue={Queue} Worker={Worker}", queue, workerId);

            while (!ct.IsCancellationRequested)
            {
                BackgroundJobQueueItem? item;

                try
                {
                    item = await _uow.BackgroundJobQueue
                        .TryClaimNextAsync(queue, workerId, lockDuration: TimeSpan.FromMinutes(5), ct: ct)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "TryClaimNextAsync failed. Queue={Queue}", queue);
                    throw;
                }

                if (item is null)
                {
                    _logger.LogDebug("No work found. Queue={Queue}", queue);
                    return;
                }

                _logger.LogInformation("Claimed item {Id} JobType={JobType} Status={Status}", item.Id, item.JobType, item.Status);

                try
                {
                    if (!_handlersByType.TryGetValue(item.JobType, out var handler))
                    {
                        throw new InvalidOperationException($"No handler registered for JobType '{item.JobType}'.");
                    }

                    await handler.HandleAsync(item.PayloadJson, ct).ConfigureAwait(false);

                    item.Status = "Completed";
                    item.ProcessedUtc = DateTimeOffset.UtcNow;
                    item.LockedBy = null;
                    item.LockedUntilUtc = null;
                    item.LastError = null;
                    item.DateModified = DateTimeOffset.UtcNow;

                    await _uow.BackgroundJobQueue.UpdateAsync(item, ct).ConfigureAwait(false);
                    await _uow.SaveAsync().ConfigureAwait(false);

                    _logger.LogInformation("Completed item {Id}", item.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Handler failed for item {Id} JobType={JobType}", item.Id, item.JobType);

                    item.Attempts++;
                    item.Status = item.Attempts >= 10 ? "Failed" : "Pending";
                    item.LastError = ex.Message;
                    item.LockedBy = null;
                    item.LockedUntilUtc = null;
                    item.DateModified = DateTimeOffset.UtcNow;

                    await _uow.BackgroundJobQueue.UpdateAsync(item, ct).ConfigureAwait(false);
                    await _uow.SaveAsync().ConfigureAwait(false);

                    throw;
                }
            }
        }
    }
}
