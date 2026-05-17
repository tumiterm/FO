// <copyright file="InAppNotificationService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/04/2026 21:38 PM
// Purpose:         Defines the InAppNotificationService.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Logging;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Infrastructure.Services
{
    /// <summary>
    /// Service for creating in-app notifications.
    /// </summary>
    public class InAppNotificationService : IInAppNotificationService
    {
        #region Private Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InAppNotificationService> _logger;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppNotificationService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public InAppNotificationService(IUnitOfWork unitOfWork, ILogger<InAppNotificationService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends a notification to a specific user.
        /// </summary>
        public async Task SendAsync(Guid recipientUserId, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null)
        {
            var notification = new InAppNotification
            {
                Id = Helper.GenerateGuid(),
                RecipientUserId = recipientUserId,
                Message = message,
                ActionUrl = actionUrl,
                IconCss = iconCss,
                Code = Helper.RandomStringGenerator(8),
                Name = message.Length > 20 ? message.Substring(0, 20) + "..." : message
            };

            await _unitOfWork.InAppNotifications.AddAsync(notification);
            await _unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Sends a notification to all users with a specific role.
        /// </summary>
        public async Task SendToRoleAsync(eSysRole role, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null)
        {
            await SendToRolesAsync([role], message, actionUrl, iconCss, createdBy);
        }

        /// <inheritdoc />
        public async Task SendToManyAsync(IEnumerable<Guid> recipientUserIds, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null)
        {
            ArgumentNullException.ThrowIfNull(recipientUserIds, nameof(recipientUserIds));
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            var distinctIds = recipientUserIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (distinctIds.Count == 0)
            {
                _logger.LogWarning("SendToManyAsync called with no valid recipient IDs. Skipping.");
                return;
            }

            foreach (var userId in distinctIds)
            {
                var notification = BuildNotification(userId, message, actionUrl, iconCss, createdBy);
                await _unitOfWork.InAppNotifications.AddAsync(notification);
            }

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Notification sent to {Count} user(s): {Message}", distinctIds.Count, Truncate(message));
        }

        /// <inheritdoc />
        public async Task SendToRolesAsync(IEnumerable<eSysRole> roles, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null)
        {
            ArgumentNullException.ThrowIfNull(roles, nameof(roles));
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            var roleSet = roles.Distinct().ToHashSet();

            if (roleSet.Count == 0)
            {
                _logger.LogWarning("SendToRolesAsync called with an empty roles collection. Skipping.");
                return;
            }

            var users = await _unitOfWork.Users.GetAllAsync(
                u => u.IsActive && u.Role != null && roleSet.Contains(u.Role.Value));

            if (users.Count == 0)
            {
                _logger.LogInformation("No active users found for roles [{Roles}]. No notifications sent.", string.Join(", ", roleSet));
                return;
            }

            foreach (var user in users)
            {
                var notification = BuildNotification(user.Id, message, actionUrl, iconCss, createdBy);
                await _unitOfWork.InAppNotifications.AddAsync(notification);
            }

            await _unitOfWork.SaveAsync();

            _logger.LogInformation(
                "Notification sent to {Count} user(s) across roles [{Roles}]: {Message}",
                users.Count,
                string.Join(", ", roleSet),
                Truncate(message));
        }

        /// <inheritdoc />
        public async Task BroadcastAsync(string message, string? actionUrl = null, string iconCss = "fa fa-bullhorn", string? createdBy = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            var allUsers = await _unitOfWork.Users.GetAllAsync(u => u.IsActive);

            if (allUsers.Count == 0)
            {
                _logger.LogWarning("BroadcastAsync found no active users. No notifications sent.");
                return;
            }

            foreach (var user in allUsers)
            {
                var notification = BuildNotification(user.Id, message, actionUrl, iconCss, createdBy);
                await _unitOfWork.InAppNotifications.AddAsync(notification);
            }

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Broadcast notification sent to {Count} user(s): {Message}", allUsers.Count, Truncate(message));
        }

        #region Private Helpers

        /// <summary>
        /// Builds a new <see cref="InAppNotification"/> entity with common defaults.
        /// </summary>
        private static InAppNotification BuildNotification(Guid recipientUserId, string message, string? actionUrl, string iconCss, string? createdBy)
        {
            return new InAppNotification
            {
                Id = Helper.GenerateGuid(),
                RecipientUserId = recipientUserId,
                Message = message,
                ActionUrl = actionUrl,
                IconCss = iconCss,
                Code = Helper.RandomStringGenerator(8),
                Name = Truncate(message),
                UserCreated = createdBy
            };
        }

        /// <summary>
        /// Truncates a message to 20 characters with an ellipsis for the Name column.
        /// </summary>
        private static string Truncate(string value, int maxLength = 20)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Length <= maxLength
                ? value
                : $"{value[..maxLength]}...";
        }
        #endregion
    }
}