// <copyright file="NotificationsController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/10/2025 15:29 PM
// Purpose:         Defines the NotificationsController

#region Usings
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace Notification.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing and retrieving notifications.
    /// </summary>
    /// <remarks>This controller is responsible for handling notification-related operations, such as
    /// retrieving active notifications for the current user. It uses dependency-injected services to perform its
    /// operations, including an <see cref="INotificationService"/> for notification management and an <see
    /// cref="ILogger{TCategoryName}"/> for logging.</remarks>
    [ApiController]
    [Route("api/[controller]")]
    public sealed class NotificationsController : Controller
    {
        #region Fields
        private readonly INotificationService _notificationsService;
        private readonly ILogger<NotificationService> _logger;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsController"/> class.
        /// </summary>
        /// <param name="notificationsService">The service used to manage and send notifications. This parameter cannot be <see langword="null"/>.</param>
        /// <param name="logger">The logger instance used to log messages for the <see cref="NotificationsController"/>. This parameter
        /// cannot be <see langword="null"/>.</param>
        public NotificationsController(INotificationService notificationsService, ILogger<NotificationService> logger)
        {
            _notificationsService = notificationsService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a collection of active notification events based on the user's role and the current date and time.
        /// </summary>
        /// <remarks>This method returns a filtered list of notification events that are currently active.
        /// If the user is authenticated, their role is used to determine which events are relevant.  The response
        /// includes event details and associated content blocks in a structured format.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing an HTTP 200 OK response with a collection of notification events. 
        /// Each event includes its metadata, styling information, and content blocks.  If no events are active, the
        /// collection will be empty.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var role = User?.Identity?.IsAuthenticated == true
                ? User.Claims.FirstOrDefault(c => c.Type == "role")?.Value
                : null;

            var events = await _notificationsService.GetActiveAsync(role, DateTime.UtcNow);

            var dto = events.Select(e => new {
                e.Id,
                e.Title,
                e.HeaderIconCss,
                e.HeaderGradientCss,
                e.HeaderTextColor,
                SizeClass = MapSize(e.ModalSize),
                e.ImageUrl,
                Blocks = e.Blocks.Select(b => new {
                    b.Type,
                    b.Text,
                    b.ListItems,
                    b.TableJson,
                    b.ImageUrl,
                    b.AltText
                })
            });

            return Ok(dto);
        }

        /// <summary>
        /// Retrieves a notification by its unique identifier.
        /// </summary>
        /// <remarks>This method returns an HTTP 200 OK response with the notification details if the
        /// notification is found,  or an HTTP 404 Not Found response if the notification does not exist.</remarks>
        /// <param name="id">The unique identifier of the notification to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the notification details if found;  otherwise, a <see
        /// cref="NotFoundResult"/> if no notification exists with the specified identifier.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ev = await _notificationsService.GetByIdAsync(id);

            if (ev == null) return NotFound();

            return Ok(MapToSummary(ev));
        }

        /// <summary>
        /// Creates a new notification event and its associated blocks.
        /// </summary>
        /// <param name="model">The data required to create the notification event, including its properties and associated blocks.</param>
        /// <returns>A <see cref="CreatedAtActionResult"/> containing the details of the created notification event if the
        /// operation succeeds. Returns a <see cref="BadRequestObjectResult"/> if the input model is invalid or an error
        /// occurs during creation. Returns a <see cref="ValidationProblemResult"/> if the model state is invalid.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationEventCreateViewModel model)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var (entity, blocks) = MapFromCreateDto(model);
            try
            {
                var created = await _notificationsService.CreateAsync(entity, blocks);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToSummary(created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing notification with the specified identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NotificationEventUpdateViewModel model)
        {
            if (id != model.Id) return BadRequest(new { error = "Route id mismatch." });
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var (entity, blocks) = MapFromUpdateDto(model);
            try
            {
                var updated = await _notificationsService.UpdateAsync(entity, blocks);
                if (updated == null) return NotFound();
                return Ok(MapToSummary(updated));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivates a notification with the specified identifier.
        /// </summary>
        /// <remarks>This method performs a partial update to deactivate the specified notification. 
        /// Ensure the <paramref name="id"/> corresponds to an existing notification.</remarks>
        /// <param name="id">The unique identifier of the notification to deactivate.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.  The result is an <see
        /// cref="IActionResult"/> indicating the outcome of the operation: <list type="bullet"> <item><description><see
        /// cref="NotFoundResult"/> if the notification with the specified identifier does not
        /// exist.</description></item> <item><description><see cref="NoContentResult"/> if the notification was
        /// successfully deactivated.</description></item> </list></returns>
        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var ok = await _notificationsService.DeactivateAsync(id);

            if (!ok) return NotFound();

            return NoContent();
        }

        #region Private
        /// <summary>
        /// Maps the specified <see cref="eNotificationModalSize"/> value to its corresponding CSS class name.
        /// </summary>
        /// <param name="size">The size of the notification modal to map.</param>
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
        /// Maps a <see cref="NotificationEvent"/> to a <see cref="NotificationEventSummaryViewModel"/>.
        /// </summary>
        /// <param name="e">The <see cref="NotificationEvent"/> instance to map. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="NotificationEventSummaryViewModel"/> containing the mapped data from the specified <see
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
                   AltText = b.AltText
               })
           };

        /// <summary>
        /// Maps a <see cref="NotificationEventCreateViewModel"/> to a <see cref="NotificationEvent"/>  and its
        /// associated collection of <see cref="NotificationContentBlock"/> instances.
        /// </summary>
        /// <remarks>The method initializes a <see cref="NotificationEvent"/> using the properties of the
        /// provided  <paramref name="dto"/>. It also maps the content blocks from the <paramref name="dto"/> to  <see
        /// cref="NotificationContentBlock"/> instances, ensuring that each block is associated with  the created
        /// event.</remarks>
        /// <param name="dto">The data transfer object containing the details for creating a <see cref="NotificationEvent"/>  and its
        /// associated content blocks.</param>
        /// <returns>A tuple containing the mapped <see cref="NotificationEvent"/> and an enumerable collection  of <see
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
                CarouselGroupKey = dto.CarouselGroupKey
            };

            var blocks = dto.Blocks.Select(b => new NotificationContentBlock
            {
                Id = b.Id ?? Guid.Empty,
                NotificationEventId = ev.Id, // will be assigned after ev.Id set
                Type = b.Type,
                Text = b.Text,
                ListItems = b.ListItems,
                TableJson = b.TableJson,
                ImageUrl = b.ImageUrl,
                AltText = b.AltText
            });

            return (ev, blocks);
        }

        /// <summary>
        /// Maps a <see cref="NotificationEventUpdateViewModel"/> to a <see cref="NotificationEvent"/>  and a collection
        /// of <see cref="NotificationContentBlock"/> instances.
        /// </summary>
        /// <remarks>The method creates a new <see cref="NotificationEvent"/> based on the properties of
        /// the provided  <paramref name="dto"/>. It also maps the content blocks from the DTO to a collection of  <see
        /// cref="NotificationContentBlock"/> objects, associating them with the notification event.</remarks>
        /// <param name="dto">The data transfer object containing the updated notification event details and content blocks.</param>
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
                CarouselGroupKey = dto.CarouselGroupKey
            };

            var blocks = dto.Blocks.Select(b => new NotificationContentBlock
            {
                Id = b.Id ?? Guid.Empty,
                NotificationEventId = ev.Id,
                Type = b.Type,
                Text = b.Text,
                ListItems = b.ListItems,
                TableJson = b.TableJson,
                ImageUrl = b.ImageUrl,
                AltText = b.AltText
            });

            return (ev, blocks);
        }
        #endregion
    }
}
