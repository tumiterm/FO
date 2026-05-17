// <copyright file="IBackgroundJobQueue.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    07-03-2026 20:41 PM
// Purpose:         Defines the IBackgroundJobQueue interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IBackgroundJobQueue : IRepository<BackgroundJobQueueItem>
    {
        Task<BackgroundJobQueueItem?> TryClaimNextAsync(string queue, string workerId, TimeSpan lockDuration, CancellationToken ct = default);

        Task<BackgroundJobQueueItem> UpdateAsync(BackgroundJobQueueItem item, CancellationToken ct = default);
    }
}