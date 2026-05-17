// <copyright file="NotificationBellController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/04/2026 21:19 PM
// Purpose:         Defines the NotificationBellController

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Handles in-app notification bell actions.
    /// </summary>
    [Authorize]
    public class NotificationBellController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationBellController"/> class.
        /// </summary>
        public NotificationBellController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Gets the unread notification count and latest notifications for the current user.
        /// Called via AJAX from the bell icon.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var user = _userService.OnGetCurrentUser();
            if (user == null) return Json(new { count = 0, notifications = Array.Empty<object>() });

            var unreadCount = await _unitOfWork.InAppNotifications.GetUnreadCountAsync(user.Id);

            var notifications = await _unitOfWork.InAppNotifications.GetAllAsync(
                filter: n => n.RecipientUserId == user.Id && !n.IsRead,
                orderBy: q => q.OrderByDescending(n => n.DateCreated),
                take: 10,
                asNoTracking: true
            );

            var result = notifications.Select(n => new
            {
                id = n.Id,
                message = n.Message,
                actionUrl = n.ActionUrl,
                iconCss = n.IconCss,
                createdBy = n.UserCreated,
                timeAgo = GetTimeAgo(n.DateCreated)
            });

            return Json(new { count = unreadCount, notifications = result });
        }

        /// <summary>
        /// Marks a notification as read and redirects to the action URL.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id, string? redirectUrl)
        {
            await _unitOfWork.InAppNotifications.MarkAsReadAsync(id);

            if (!string.IsNullOrWhiteSpace(redirectUrl))
                return Redirect(redirectUrl);

            return Ok();
        }

        /// <summary>
        /// Marks all notifications as read for the current user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = _userService.OnGetCurrentUser();
            if (user == null) return Unauthorized();

            await _unitOfWork.InAppNotifications.MarkAllAsReadAsync(user.Id);
            return Ok();
        }

        /// <summary>
        /// Converts a UTC datetime to a human-readable "time ago" string.
        /// </summary>
        private static string GetTimeAgo(DateTimeOffset createdUtc)
        {
            var span = DateTimeOffset.UtcNow - createdUtc;
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return createdUtc.ToString("dd MMM yyyy");
        }
    }
}