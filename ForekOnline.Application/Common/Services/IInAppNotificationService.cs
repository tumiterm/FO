// <copyright file="IInAppNotificationService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/04/2026 21:36 PM
// Purpose:         Defines the IInAppNotificationService

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Service for creating in-app notifications.
    /// </summary>
    public interface IInAppNotificationService
    {
        /// <summary>
        /// Sends a notification to a specific user.
        /// </summary>
        /// <param name="recipientUserId">The target user's ID.</param>
        /// <param name="message">The notification message.</param>
        /// <param name="actionUrl">Optional clickable URL.</param>
        /// <param name="iconCss">Optional icon CSS class.</param>
        /// <param name="createdBy">The name of the person/system that triggered it.</param>
        Task SendAsync(Guid recipientUserId, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null);

        /// <summary>
        /// Sends a notification to all users with a specific role.
        /// </summary>
        Task SendToRoleAsync(eSysRole role, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null);

        /// <summary>
        /// Sends a notification to multiple specific users.
        /// </summary>
        /// <param name="recipientUserIds">The list of target user IDs.</param>
        /// <param name="message">The notification message.</param>
        /// <param name="actionUrl">Optional clickable URL.</param>
        /// <param name="iconCss">Optional icon CSS class.</param>
        /// <param name="createdBy">The name of the person/system that triggered it.</param>
        Task SendToManyAsync(IEnumerable<Guid> recipientUserIds, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null);

        /// <summary>
        /// Sends a notification to all active users that belong to any of the specified roles.
        /// </summary>
        /// <param name="roles">The target roles.</param>
        /// <param name="message">The notification message.</param>
        /// <param name="actionUrl">Optional clickable URL.</param>
        /// <param name="iconCss">Optional icon CSS class.</param>
        /// <param name="createdBy">The name of the person/system that triggered it.</param>
        Task SendToRolesAsync(IEnumerable<eSysRole> roles, string message, string? actionUrl = null, string iconCss = "fa fa-bell", string? createdBy = null);

        /// <summary>
        /// Sends a notification to every active user in the system regardless of role.
        /// </summary>
        /// <param name="message">The notification message.</param>
        /// <param name="actionUrl">Optional clickable URL.</param>
        /// <param name="iconCss">Optional icon CSS class.</param>
        /// <param name="createdBy">The name of the person/system that triggered it.</param>
        Task BroadcastAsync(string message, string? actionUrl = null, string iconCss = "fa fa-bullhorn", string? createdBy = null);
    }
}