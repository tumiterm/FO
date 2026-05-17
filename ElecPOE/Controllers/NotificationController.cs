// <copyright file="NotificationController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    24/10/2025 12:31 PM
// Purpose:         Defines the NotificationController

#region Using Directives
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// MVC (non-API) controller for managing notification events and their content blocks.
    /// Provides full CRUD, activation/deactivation, reordering and preview capabilities.
    /// </summary>
    public class NotificationController : Controller
    {
        #region Private
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;
        private readonly IMemoryCache _cache;

        // Cache keys
        private const string ActiveCacheKey = "notifications_active";
        private static readonly TimeSpan ActiveCacheDuration = TimeSpan.FromMinutes(2);
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="notificationService">The service responsible for handling notification-related operations.</param>
        /// <param name="logger">The logger used to log diagnostic and operational information.</param>
        /// <param name="cache">The memory cache used to store and retrieve temporary data.</param>
        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger, IMemoryCache cache)
        {
            _notificationService = notificationService;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Displays a list of active notification events for the current user, ordered by display priority.
        /// </summary>
        /// <remarks>This method retrieves active notification events based on the user's role and caches
        /// the results to improve performance. If the user is not authenticated, a null role is used to fetch
        /// notifications. The notifications are ordered by their display order before being passed to the
        /// view.</remarks>
        /// <returns>An <see cref="IActionResult"/> that renders the "Index" view with a model containing the list of active
        /// notification summaries.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var role = User?.Identity?.IsAuthenticated == true
                ? User.Claims.FirstOrDefault(c => c.Type == "role")?.Value
                : null;

            var cacheKey = $"{ActiveCacheKey}:{role ?? "__anon"}";

            if (!_cache.TryGetValue(cacheKey, out IReadOnlyList<NotificationEvent> events))
            {
                events = await _notificationService.GetActiveAsync(role, DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);
                _cache.Set(cacheKey, events, ActiveCacheDuration);
            }

            var model = events
                .OrderBy(e => e.DisplayOrder)
                .Select(MapToSummary)
                .ToList();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());

            return new JsonResult(model, jsonOptions);
        }

       /// <summary>
       /// Retrieves the details of a notification by its unique identifier.
       /// </summary>
       /// <param name="id">The unique identifier of the notification to retrieve.</param>
       /// <returns>An <see cref="IActionResult"/> that renders the "Details" view with the notification details  if found;
       /// otherwise, a <see cref="NotFoundResult"/> if the notification does not exist.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var ev = await _notificationService.GetByIdAsync(id);

            if (ev == null) return NotFound();

            return View("Details", MapToSummary(ev));
        }

        /// <summary>
        /// Displays the notification event creation view with a pre-populated model.
        /// </summary>
        /// <remarks>The returned view model includes default values for the notification event, such as 
        /// the current UTC time as the start date, one week later as the end date, and an active status.</remarks>
        /// <returns>An <see cref="IActionResult"/> that renders the "Create" view with a pre-configured  <see
        /// cref="NotificationEventCreateViewModel"/> instance.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new NotificationEventCreateViewModel
            {
                StartUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                EndUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime.AddDays(7),
                IsActive = true,
                Blocks = new List<NotificationContentBlockViewModel>()
            };
            return View("Create", vm);
        }

        /// <summary>
        /// Creates a new notification based on the provided model and redirects to the edit page upon success.
        /// </summary>
        /// <remarks>This method validates the provided model and attempts to create a new notification. 
        /// If the model is invalid, the create view is re-displayed with validation errors.  If an <see
        /// cref="ArgumentException"/> is thrown during creation, the error message is added to the model state.  For
        /// unexpected errors, an error message is displayed, and the create view is re-displayed.</remarks>
        /// <param name="model">The view model containing the data required to create a new notification.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation.  Returns a redirect to the edit
        /// page if the creation is successful, or re-displays the create view with validation errors if the model is
        /// invalid or an error occurs.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificationEventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var (entity, blocks) = MapFromCreateDto(model);

            try
            {
                var created = await _notificationService.CreateAsync(entity, blocks);
                InvalidateCache();
                TempData["success"] = "Notification created.";
                return RedirectToAction(nameof(Edit), new { id = created.Id });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Create validation failed.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Create", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating notification.");
                TempData["error"] = "An unexpected error occurred.";
                return View("Create", model);
            }
        }

        /// <summary>
        /// Displays the edit view for a notification event with the specified identifier.
        /// </summary>
        /// <remarks>The method retrieves the notification event by its identifier and maps its details to
        /// a view model. The view model is then passed to the "Edit" view for rendering.</remarks>
        /// <param name="id">The unique identifier of the notification event to edit.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the edit view with the notification event's details. Returns
        /// <see cref="NotFoundResult"/> if the notification event is not found.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var ev = await _notificationService.GetByIdAsync(id);
            if (ev == null) return NotFound();

            var vm = new NotificationEventUpdateViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                HeaderIconCss = ev.HeaderIconCss,
                HeaderGradientCss = ev.HeaderGradientCss,
                HeaderTextColor = ev.HeaderTextColor,
                ModalSize = ev.ModalSize,
                ImageUrl = ev.ImageUrl,
                StartUtc = ev.StartUtc,
                EndUtc = ev.EndUtc,
                DisplayOrder = ev.DisplayOrder,
                IsActive = ev.IsActive,
                AudienceRole = ev.AudienceRole,
                CarouselGroupKey = ev.CarouselGroupKey,
                Blocks = ev.Blocks.Select(b => new NotificationContentBlockViewModel
                {
                    Id = b.Id,
                    Type = b.Type,
                    Text = b.Text,
                    ListItems = b.ListItems,
                    TableJson = b.TableJson,
                    ImageUrl = b.ImageUrl,
                    AltText = b.AltText
                }).ToList()
            };

            return View("Edit", vm);
        }

        /// <summary>
        /// Updates an existing notification with the specified data.
        /// </summary>
        /// <remarks>This method validates the input model and ensures the route ID matches the model's
        /// ID. If validation fails,  the user is returned to the <c>Edit</c> view with appropriate error messages. If
        /// the update operation succeeds,  the cache is invalidated, and the user is redirected to the <c>Edit</c> view
        /// for the updated notification.</remarks>
        /// <param name="id">The unique identifier of the notification to update.</param>
        /// <param name="model">The data used to update the notification, including its properties and associated blocks.</param>
        /// <returns>A task that represents the asynchronous operation. The result is an <see cref="IActionResult">: <list
        /// type="bullet"> <item><description>Redirects to the <c>Edit</c> view with the updated notification if the
        /// operation succeeds.</description></item> <item><description>Redirects to the <c>Index</c> view if the
        /// notification is not found.</description></item> <item><description>Returns the <c>Edit</c> view with
        /// validation errors if the input is invalid or an error occurs.</description></item> </list></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, NotificationEventUpdateViewModel model)
        {
            if (id != model.Id)
            {
                ModelState.AddModelError(string.Empty, "Route id mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            var (entity, blocks) = MapFromUpdateDto(model);

            try
            {
                var updated = await _notificationService.UpdateAsync(entity, blocks);
                if (updated == null)
                {
                    TempData["error"] = "Notification not found.";
                    return RedirectToAction(nameof(Index));
                }

                InvalidateCache();
                TempData["success"] = "Notification updated.";
                return RedirectToAction(nameof(Edit), new { id = updated.Id });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Edit validation failed.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Edit", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating notification {Id}", id);
                TempData["error"] = "An unexpected error occurred.";
                return View("Edit", model);
            }
        }

        /// <summary>
        /// Deactivates a notification with the specified identifier.
        /// </summary>
        /// <remarks>If the notification is not found or is already inactive, an error message is added to
        /// <see cref="TempData"/>. Otherwise, the notification is deactivated, the cache is invalidated,  and a success
        /// message is added to <see cref="TempData"/>.</remarks>
        /// <param name="id">The unique identifier of the notification to deactivate.</param>
        /// <returns>A task that represents the asynchronous operation. The result is a redirect to the  <see cref="Index"/>
        /// action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var ok = await _notificationService.DeactivateAsync(id);

            if (!ok)
            {
                TempData["error"] = "Notification not found or already inactive.";
            }
            else
            {
                InvalidateCache();
                TempData["success"] = "Notification deactivated.";
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Activates a notification with the specified identifier.
        /// </summary>
        /// <remarks>If the notification is not found, an error message is added to <c>TempData</c>, and
        /// the user is redirected to the <c>Index</c> action. If the notification is already active, no changes are
        /// made. Otherwise, the notification is activated, the cache is invalidated,  and a success message is added to
        /// <c>TempData</c>.</remarks>
        /// <param name="id">The unique identifier of the notification to activate.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> that represents the asynchronous operation.  Redirects to the
        /// <c>Index</c> action upon completion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(Guid id)
        {
            var ev = await _notificationService.GetByIdAsync(id);
            if (ev == null)
            {
                TempData["error"] = "Notification not found.";
                return RedirectToAction(nameof(Index));
            }

            if (!ev.IsActive)
            {
                ev.IsActive = true;
                // Reuse update without changes to blocks (pass existing)
                await _notificationService.UpdateAsync(ev, ev.Blocks);
                InvalidateCache();
                TempData["success"] = "Notification activated.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Updates the display order of notifications based on the provided mapping of notification IDs to their new
        /// order values.
        /// </summary>
        /// <remarks>This method processes the provided order mapping to update the display order of
        /// notifications.  Notifications that are not found or do not require a change in order are skipped.  If any
        /// changes are made, the cache is invalidated to ensure the updated order is reflected.</remarks>
        /// <param name="orderMap">A dictionary where the key is the unique identifier of a notification, and the value is the new display
        /// order for that notification. The dictionary must not be null or empty.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the <c>Index</c> action. If the operation succeeds, a
        /// success message is added to <c>TempData</c>. If no changes are applied, an informational message is added.
        /// If the input is invalid, an error message is added.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromForm] Dictionary<Guid, int> orderMap)
        {

            if (orderMap == null || orderMap.Count == 0)
            {
                TempData["error"] = "No ordering data provided.";
                return RedirectToAction(nameof(Index));
            }

            int changed = 0;
            foreach (var kv in orderMap)
            {
                var ev = await _notificationService.GetByIdAsync(kv.Key);
                if (ev == null) continue;
                if (ev.DisplayOrder != kv.Value)
                {
                    ev.DisplayOrder = kv.Value;
                    await _notificationService.UpdateAsync(ev, ev.Blocks);
                    changed++;
                }
            }

            if (changed > 0)
            {
                InvalidateCache();
                TempData["success"] = $"Reordered {changed} notifications.";
            }
            else
            {
                TempData["info"] = "No changes applied.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Adds a new content block to the specified notification event.
        /// </summary>
        /// <remarks>This method validates the provided block data and adds it to the specified
        /// notification event. If the data is invalid, the user is redirected back to the edit page with an error
        /// message. The method also updates the notification event and invalidates any related cached data.</remarks>
        /// <param name="eventId">The unique identifier of the notification event to which the block will be added.</param>
        /// <param name="blockVm">The view model containing the data for the new content block.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the edit page of the notification event if the operation
        /// succeeds, or returns a <see cref="NotFoundResult"/> if the event does not exist.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBlock(Guid eventId, NotificationContentBlockViewModel blockVm)
        {
            var ev = await _notificationService.GetByIdAsync(eventId);
            if (ev == null) return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid block data.";
                return RedirectToAction(nameof(Edit), new { id = eventId });
            }

            var nextOrder = ev.Blocks.Any() ? ev.Blocks.Max(x => x.Order) + 1 : 0;

            var newBlock = new NotificationContentBlock
            {
                Id = Guid.NewGuid(),
                NotificationEventId = ev.Id,
                Type = blockVm.Type,
                Text = blockVm.Text,
                ListItems = NormalizeListItems(blockVm.ListItems),
                TableJson = NormalizeTableJson(blockVm.TableJson),
                ImageUrl = NormalizeImageUrl(blockVm.ImageUrl),
                AltText = blockVm.AltText,
                Order = nextOrder
            };

            ev.Blocks.Add(newBlock);
            await _notificationService.UpdateAsync(ev, ev.Blocks);
            InvalidateCache();

            TempData["success"] = "Block added.";
            return RedirectToAction(nameof(Edit), new { id = eventId });
        }

        /// <summary>
        /// Updates the order of blocks within a specified event based on the provided mapping of block IDs to their new
        /// order values.
        /// </summary>
        /// <remarks>If the event is not found, a 404 Not Found result is returned. If no changes are made
        /// to the block order, an informational message is displayed. When changes are made, the event is updated, the
        /// cache is invalidated, and a success message is displayed.</remarks>
        /// <param name="eventId">The unique identifier of the event whose blocks are to be reordered.</param>
        /// <param name="orderMap">A dictionary mapping block IDs to their new order values. Each key represents the ID of a block, and the
        /// corresponding value represents its new order.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> that represents the asynchronous operation. The result is a redirect to
        /// the edit view of the event if the operation succeeds.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderBlocks(Guid eventId, Dictionary<Guid, int> orderMap)
        {
            var ev = await _notificationService.GetByIdAsync(eventId);
            if (ev == null) return NotFound();

            bool changed = false;
            foreach (var kv in orderMap)
            {
                var blk = ev.Blocks.FirstOrDefault(b => b.Id == kv.Key);
                if (blk == null) continue;
                if (blk.Order != kv.Value)
                {
                    blk.Order = kv.Value;
                    changed = true;
                }
            }

            if (changed)
            {
                await _notificationService.UpdateAsync(ev, ev.Blocks);
                InvalidateCache();
                TempData["success"] = "Blocks reordered.";
            }
            else
            {
                TempData["info"] = "No block order changes.";
            }

            return RedirectToAction(nameof(Edit), new { id = eventId });
        }

        /// <summary>
        /// Removes a block from the specified event.
        /// </summary>
        /// <remarks>If the specified block is not found within the event, the user is redirected to the
        /// edit page of the event  with an error message. Upon successful removal, a success message is
        /// displayed.</remarks>
        /// <param name="eventId">The unique identifier of the event from which the block will be removed.</param>
        /// <param name="blockId">The unique identifier of the block to be removed.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the edit page of the event if the operation is successful, 
        /// or returns a <see cref="NotFoundResult"/> if the event does not exist.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBlock(Guid eventId, Guid blockId)
        {
            var ev = await _notificationService.GetByIdAsync(eventId);
            if (ev == null) return NotFound();

            var blk = ev.Blocks.FirstOrDefault(b => b.Id == blockId);
            if (blk == null)
            {
                TempData["error"] = "Block not found.";
                return RedirectToAction(nameof(Edit), new { id = eventId });
            }

            ev.Blocks.Remove(blk);
            await _notificationService.UpdateAsync(ev, ev.Blocks);
            InvalidateCache();

            TempData["success"] = "Block removed.";
            return RedirectToAction(nameof(Edit), new { id = eventId });
        }

        /// <summary>
        /// Updates the content of a notification block within a specified event.
        /// </summary>
        /// <remarks>This method validates the provided block data and updates the corresponding block in
        /// the event.  If the event or block is not found, or if the provided data is invalid, the method returns an
        /// error response.</remarks>
        /// <param name="eventId">The unique identifier of the event containing the block to update.</param>
        /// <param name="blockId">The unique identifier of the block to update.</param>
        /// <param name="blockVm">The view model containing the updated content for the block.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the edit page of the event if the update is successful, or
        /// an appropriate error response if the operation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBlock(Guid eventId, Guid blockId, NotificationContentBlockViewModel blockVm)
        {
            var ev = await _notificationService.GetByIdAsync(eventId);
            if (ev == null) return NotFound();

            var blk = ev.Blocks.FirstOrDefault(b => b.Id == blockId);
            if (blk == null)
            {
                TempData["error"] = "Block not found.";
                return RedirectToAction(nameof(Edit), new { id = eventId });
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid block data.";
                return RedirectToAction(nameof(Edit), new { id = eventId });
            }

            blk.Type = blockVm.Type;
            blk.Text = blockVm.Text;
            blk.ListItems = NormalizeListItems(blockVm.ListItems);
            blk.TableJson = NormalizeTableJson(blockVm.TableJson);
            blk.ImageUrl = NormalizeImageUrl(blockVm.ImageUrl);
            blk.AltText = blockVm.AltText;

            await _notificationService.UpdateAsync(ev, ev.Blocks);
            InvalidateCache();

            TempData["success"] = "Block updated.";
            return RedirectToAction(nameof(Edit), new { id = eventId });
        }

        public async Task<IActionResult> Notifications()
        {
            var role = User?.Identity?.IsAuthenticated == true
                ? User.Claims.FirstOrDefault(c => c.Type == "role")?.Value
                : null;

            if (!_cache.TryGetValue((ActiveCacheKey, role), out IReadOnlyList<NotificationEvent> events))
            {
                events = await _notificationService.GetActiveAsync(role, DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);
                _cache.Set((ActiveCacheKey, role), events, ActiveCacheDuration);
            }

            var model = events
                .OrderBy(e => e.DisplayOrder)
                .Select(MapToSummary)
                .ToList();

            return View("Notifications", model);
        }

        #region Private Methods

        /// <summary>
        /// Maps a <see cref="NotificationEvent"/> instance to a <see cref="NotificationEventSummaryViewModel"/>.
        /// </summary>
        /// <param name="e">The <see cref="NotificationEvent"/> to map. This parameter cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="NotificationEventSummaryViewModel"/> containing the mapped summary data from the specified <see
        /// cref="NotificationEvent"/>.</returns>
        private NotificationEventSummaryViewModel MapToSummary(NotificationEvent e) =>
           new()
           {
               Id = e.Id,
               Title = e.Title,
               HeaderIconCss = e.HeaderIconCss,
               HeaderGradientCss = e.HeaderGradientCss,
               HeaderTextColor = e.HeaderTextColor,
               SizeClass = MapSize(e.ModalSize),
               ImageUrl = e.ImageUrl,
               StartUtc = e.StartUtc,
               EndUtc = e.EndUtc,
               IsActive = e.IsActive,
               DisplayOrder = e.DisplayOrder,
               Blocks = e.Blocks.Select(b => new NotificationContentBlockViewModel
               {
                   Id = b.Id,
                   Type = b.Type,
                   Text = b.Text,
                   ListItems = b.ListItems,
                   TableJson = b.TableJson,
                   ImageUrl = b.ImageUrl,
                   AltText = b.AltText,
                   Order = b.Order
               })
           };

        /// <summary>
        /// Maps the specified <see cref="eNotificationModalSize"/> value to its corresponding CSS class name.
        /// </summary>
        /// <param name="size">The size of the notification modal to map. Must be a valid <see cref="eNotificationModalSize"/> value.</param>
        /// <returns>A string representing the CSS class name for the specified modal size.  Returns "modal-sm" for <see
        /// cref="eNotificationModalSize.Small"/>, "modal-lg" for <see cref="eNotificationModalSize.Large"/>, 
        /// "modal-xl" for <see cref="eNotificationModalSize.ExtraLarge"/>, and an empty string for <see
        /// cref="eNotificationModalSize.Default"/>  or unrecognized values.</returns>
        private string MapSize(eNotificationModalSize size) => size switch
        {
            eNotificationModalSize.Small => "modal-sm",
            eNotificationModalSize.Default => "",
            eNotificationModalSize.Large => "modal-lg",
            eNotificationModalSize.ExtraLarge => "modal-xl",
            _ => ""
        };

        /// <summary>
        /// Maps a <see cref="NotificationEventCreateViewModel"/> to a <see cref="NotificationEvent"/>  and a collection
        /// of <see cref="NotificationContentBlock"/> instances.
        /// </summary>
        /// <remarks>This method creates a new <see cref="NotificationEvent"/> based on the properties of
        /// the provided DTO.  It also maps the content blocks from the DTO to a collection of <see
        /// cref="NotificationContentBlock"/> objects,  generating new IDs for blocks that do not already have one. The
        /// method ensures that the table JSON in each block  is normalized before returning the result.</remarks>
        /// <param name="dto">The data transfer object containing the details for creating a notification event and its associated content
        /// blocks.</param>
        /// <returns>A tuple containing the mapped <see cref="NotificationEvent"/> and an <see cref="IEnumerable{T}"/> of  <see
        /// cref="NotificationContentBlock"/> instances.</returns>
        private (NotificationEvent ev, IEnumerable<NotificationContentBlock> blocks) MapFromCreateDto(NotificationEventCreateViewModel dto)
        {
            var ev = new NotificationEvent
            {
                Title = dto.Title,
                HeaderIconCss = dto.HeaderIconCss,
                HeaderGradientCss = dto.HeaderGradientCss,
                HeaderTextColor = dto.HeaderTextColor,
                ModalSize = dto.ModalSize,
                ImageUrl = dto.ImageUrl,
                StartUtc = dto.StartUtc,
                EndUtc = dto.EndUtc,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                AudienceRole = dto.AudienceRole,
                CarouselGroupKey = dto.CarouselGroupKey,
                Blocks = new List<NotificationContentBlock>()
            };

            var blocks = dto.Blocks.Select((b, idx) => new NotificationContentBlock
            {
                Id = b.Id ?? Guid.NewGuid(),
                NotificationEventId = ev.Id,
                Type = b.Type,
                Text = b.Text,
                ListItems = NormalizeListItems(b.ListItems),
                TableJson = NormalizeTableJson(b.TableJson),
                ImageUrl = NormalizeImageUrl(b.ImageUrl),
                AltText = b.AltText,
                Order = b.Order <= 0 ? idx : b.Order
            }).ToList();
            ev.ImageUrl = NormalizeImageUrl(ev.ImageUrl);

            return (ev, blocks);
        }

        /// <summary>
        /// Maps a <see cref="NotificationEventUpdateViewModel"/> to a <see cref="NotificationEvent"/>  and a collection
        /// of <see cref="NotificationContentBlock"/> instances.
        /// </summary>
        /// <remarks>This method creates a new <see cref="NotificationEvent"/> based on the properties of
        /// the provided  <paramref name="dto"/> and maps its associated content blocks to a collection of  <see
        /// cref="NotificationContentBlock"/> objects. If a content block's <c>Id</c> is not provided,  a new <see
        /// cref="Guid"/> is generated for it.</remarks>
        /// <param name="dto">The data transfer object containing the updated notification event details and associated content blocks.</param>
        /// <returns>A tuple containing the mapped <see cref="NotificationEvent"/> and an <see cref="IEnumerable{T}"/>  of <see
        /// cref="NotificationContentBlock"/> instances.</returns>
        private (NotificationEvent ev, IEnumerable<NotificationContentBlock> blocks) MapFromUpdateDto(NotificationEventUpdateViewModel dto)
        {
            var ev = new NotificationEvent
            {
                Id = dto.Id,
                Title = dto.Title,
                HeaderIconCss = dto.HeaderIconCss,
                HeaderGradientCss = dto.HeaderGradientCss,
                HeaderTextColor = dto.HeaderTextColor,
                ModalSize = dto.ModalSize,
                ImageUrl = dto.ImageUrl,
                StartUtc = dto.StartUtc,
                EndUtc = dto.EndUtc,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                AudienceRole = dto.AudienceRole,
                CarouselGroupKey = dto.CarouselGroupKey,
                Blocks = new List<NotificationContentBlock>()
            };

            var blocks = dto.Blocks.Select((b, idx) => new NotificationContentBlock
            {
                Id = b.Id ?? Guid.NewGuid(),
                NotificationEventId = ev.Id,
                Type = b.Type,
                Text = b.Text,
                ListItems = NormalizeListItems(b.ListItems),
                TableJson = NormalizeTableJson(b.TableJson),
                ImageUrl = NormalizeImageUrl(b.ImageUrl),
                AltText = b.AltText,
                Order = b.Order < 0 ? idx : b.Order
            }).ToList();
            ev.ImageUrl = NormalizeImageUrl(ev.ImageUrl);

            return (ev, blocks);
        }

        /// <summary>
        /// Normalizes a JSON string by parsing it and re-serializing it into a standardized format.
        /// </summary>
        /// <remarks>This method ensures that the JSON string is formatted in a consistent manner by
        /// parsing and re-serializing it. If the input is not valid JSON, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="raw">The raw JSON string to normalize. Can be null or whitespace.</param>
        /// <returns>A normalized JSON string if the input is valid JSON; otherwise, <see langword="null"/>. Returns <see
        /// langword="null"/> if the input is null, whitespace, or invalid JSON.</returns>
        private string? NormalizeTableJson(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            try
            {
                using var doc = JsonDocument.Parse(raw);
                return JsonSerializer.Serialize(doc.RootElement);
            }
            catch
            {
                return null; 
            }
        }
        private static string?[]? NormalizeListItems(string?[]? source)
        {
            if (source == null) return null;
            if (source.Length == 1 && !string.IsNullOrWhiteSpace(source[0]) && source[0]!.Contains(','))
            {
                return source[0]!
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .ToArray();
            }
            return source
                .Select(s => string.IsNullOrWhiteSpace(s) ? null : s!.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }
        private static string? NormalizeImageUrl(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var url = raw.Trim();
            if (url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return null;

            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("//") ||
                url.StartsWith("/"))
                return url;

            if (url.Contains('.') && !url.Contains(' '))
                return "https://" + url;

            return url;
        }

        /// <summary>
        /// Invalidates the cached data associated with the active cache key.
        /// </summary>
        /// <remarks>This method clears the cache for the active role-based data. It ensures that any
        /// stale or outdated information is removed, allowing fresh data to be loaded when needed. If role-based
        /// caching is extended in the future, this method may need to account for additional role-specific
        /// keys.</remarks>
        private void InvalidateCache()
        {
            // Clear all role variants by removing pattern (simplified)
            // If role-based caching becomes complex, maintain explicit keys.
            // For now remove everything by enumerating common roles if needed.
            _cache.Remove((ActiveCacheKey));
            // Could extend to roles: Admin, Facilitator, Student etc.
        }
        #endregion
    }

}
