// <copyright file="NotificationService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/10/2025 15:29 PM
// Purpose:         Defines the NotificationService interface implementation.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Logging;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality for managing and retrieving notification events.
    /// </summary>
    /// <remarks>This service is responsible for retrieving active notification events based on the specified
    /// user role  and the current time. It interacts with the underlying data store and ensures that only relevant 
    /// notifications are returned. The service is designed to be used in scenarios where notifications need  to be
    /// filtered and displayed to users based on their roles and the current time.</remarks>
    public class NotificationService : INotificationService
    {
        #region Fields
        private readonly IUnitOfWork _context;
        private readonly ILogger<NotificationService> _logger;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="context">The unit of work used to interact with the data layer.</param>
        /// <param name="logger">The logger instance used for logging operations and diagnostics.</param>
        public NotificationService(IUnitOfWork context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new notification event and persists it to the database.
        /// </summary>
        /// <remarks>This method validates the provided notification event before persisting it. The event
        /// and its associated content blocks are saved to the database, and a log entry is created upon successful
        /// completion.</remarks>
        /// <param name="ev">The <see cref="NotificationEvent"/> to create. The event's <see cref="NotificationEvent.Id"/> will be
        /// assigned a new GUID if it is empty.</param>
        /// <param name="blocks">A collection of <see cref="NotificationContentBlock"/> objects to associate with the notification event.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while performing the operation.</param>
        /// <returns>The created <see cref="NotificationEvent"/> with its associated content blocks.</returns>
        public async Task<NotificationEvent> CreateAsync(NotificationEvent ev, IEnumerable<NotificationContentBlock> blocks, CancellationToken ct = default)
        {
            Validate(ev);
            ev.Id = ev.Id == Guid.Empty ? Guid.NewGuid() : ev.Id;
            ev.Blocks = blocks.ToList();

            await _context.Notification.AddAsync(ev);
            await _context.SaveAsync();
            _logger.LogInformation("NotificationEvent created {Id}", ev.Id);
            return ev;
        }

        /// <summary>
        /// Retrieves a list of active notification events based on the specified user role and the current UTC time.
        /// </summary>
        /// <remarks>The returned list is ordered by the display order of the notification
        /// events.</remarks>
        /// <param name="userRole">The role of the user for whom the notification events are being retrieved.  If <see langword="null"/>,
        /// events applicable to all roles are included.</param>
        /// <param name="utcNow">The current UTC time used to determine which notification events are active. Events are considered active if
        /// their start time is on or before this value  and their end time is on or after this value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="NotificationEvent"/> objects that are currently active and match the specified criteria.</returns>
        public async Task<IReadOnlyList<NotificationEvent>> GetActiveAsync(string? userRole, DateTime utcNow)
        {
            var events = await _context.Notification.GetAllAsync(filter: n => n.IsActive && n.StartUtc <= utcNow
                                                     && n.EndUtc >= utcNow && (n.AudienceRole == null || n.AudienceRole == userRole),  
                                                     includeProperties: new[] {nameof(NotificationEvent.Blocks) });

            return events.OrderBy(e => e.DisplayOrder).ToList();


            /* Your code
            return await _db.NotificationEvents
                .Include(e => e.Blocks)
                .Where(e => e.IsActive
                            && e.StartUtc <= utcNow
                            && e.EndUtc >= utcNow
                            && (e.AudienceRole == null || e.AudienceRole == userRole))
                .OrderBy(e => e.DisplayOrder)
                .ToListAsync(); */
        }

        /// <summary>
        /// Retrieves a notification event by its unique identifier.
        /// </summary>
        /// <remarks>The method queries the data source for a notification event with the specified
        /// identifier. The result includes related blocks as part of the returned notification event.</remarks>
        /// <param name="id">The unique identifier of the notification event to retrieve.</param>
        /// <returns>A <see cref="NotificationEvent"/> object if a matching event is found; otherwise, <see langword="null"/>.</returns>
        public async Task<NotificationEvent?> GetByIdAsync(Guid id)
        {
            var list = await _context.Notification.GetAllAsync(
             filter: n => n.Id == id,
             includeProperties: new[] { nameof(NotificationEvent.Blocks) });

            return list.FirstOrDefault();
        }

        /// <summary>
        /// Updates an existing <see cref="NotificationEvent"/> with new values and associated content blocks.
        /// </summary>
        /// <remarks>This method retrieves the existing event by its identifier, updates its properties,
        /// and replaces its associated content blocks with the provided collection. If the event does not exist, the
        /// method returns <see langword="null"/>. The operation is performed asynchronously.</remarks>
        /// <param name="ev">The <see cref="NotificationEvent"/> containing the updated values. The event must already exist in the
        /// system.</param>
        /// <param name="blocks">A collection of <see cref="NotificationContentBlock"/> objects to associate with the event. Existing blocks
        /// will be replaced with these.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while performing the operation.</param>
        /// <returns>The updated <see cref="NotificationEvent"/> if the event exists; otherwise, <see langword="null"/>.</returns>
        public async Task<NotificationEvent?> UpdateAsync(NotificationEvent ev, IEnumerable<NotificationContentBlock> blocks, CancellationToken ct = default)
        {
            var existing = await GetByIdAsync(ev.Id);
            if (existing == null) return null;

            Validate(ev);

            existing.Title = ev.Title;
            existing.HeaderIconCss = ev.HeaderIconCss;
            existing.HeaderGradientCss = ev.HeaderGradientCss;
            existing.HeaderTextColor = ev.HeaderTextColor;
            existing.ModalSize = ev.ModalSize;
            existing.ImageUrl = ev.ImageUrl;
            existing.StartUtc = ev.StartUtc;
            existing.EndUtc = ev.EndUtc;
            existing.DisplayOrder = ev.DisplayOrder;
            existing.IsActive = ev.IsActive;
            existing.AudienceRole = ev.AudienceRole;
            existing.CarouselGroupKey = ev.CarouselGroupKey;

            var blockRepo = _context.NotificationContentBlock;

            foreach (var b in existing.Blocks.ToList())
                await blockRepo.RemoveAsync(b);

            existing.Blocks.Clear();

            foreach (var b in blocks)
            {
                b.Id = b.Id == Guid.Empty ? Guid.NewGuid() : b.Id;
                b.NotificationEventId = existing.Id;
                await blockRepo.AddAsync(b);
                existing.Blocks.Add(b);
            }

            await _context.Notification.UpdateNotificationEventAsync(existing);
            await _context.SaveAsync();

            _logger.LogInformation("NotificationEvent updated {Id}", existing.Id);

            return existing;
        }

        /// <summary>
        /// Deactivates a notification event by its unique identifier.
        /// </summary>
        /// <remarks>This method sets the <c>IsActive</c> property of the notification event to <see
        /// langword="false"/>  and persists the change to the database. A log entry is created upon successful
        /// deactivation.</remarks>
        /// <param name="id">The unique identifier of the notification event to deactivate.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns><see langword="true"/> if the notification event was successfully deactivated; otherwise, <see
        /// langword="false"/> if the event does not exist.</returns>
        public async Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await GetByIdAsync(id);

            if (existing == null) return false;
            existing.IsActive = false;

            await _context.Notification.UpdateNotificationEventAsync(existing);
            await _context.SaveAsync();

            _logger.LogInformation("NotificationEvent deactivated {Id}", id);
            return true;
        }

        /// <summary>
        /// Validates the specified <see cref="NotificationEvent"/> instance to ensure it meets required conditions.
        /// </summary>
        /// <param name="ev">The <see cref="NotificationEvent"/> instance to validate.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="ev"/> has a <see cref="NotificationEvent.StartUtc"/> value that is not earlier
        /// than  <see cref="NotificationEvent.EndUtc"/>, or if <see cref="NotificationEvent.Title"/> is null, empty, or
        /// consists only of whitespace.</exception>
        private static void Validate(NotificationEvent ev)
        {
            if (ev.StartUtc >= ev.EndUtc)
                throw new ArgumentException("StartUtc must be earlier than EndUtc.");
            if (string.IsNullOrWhiteSpace(ev.Title))
                throw new ArgumentException("Title is required.");
        }

    }
}

   


