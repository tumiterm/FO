// <copyright file="IUserService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    20/01/2023 15:11 PM
// Purpose:         Defines the IUserService interface

#region Usings
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines methods for managing user-related operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a result of a list of <see cref="User"/> objects.</returns>
        Task<IReadOnlyList<User>> GetAllUsersAsync();

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object containing user information to be registered.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, 
        /// with a result tuple containing a boolean status and a message indicating success or failure details.</returns>
        Task<(bool Status, string Message)> RegisterUserAsync(User user);

        /// <summary>
        /// Removes an existing user by their unique identifier.
        /// </summary>
        /// <param name="userId">The <see cref="Guid"/> representing the user's unique identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, 
        /// with a result of <c>true</c> if the user was successfully removed; otherwise, <c>false</c>.</returns>
        Task<bool> RemoveUserAsync(Guid userId);

        /// <summary>
        /// Retrieves detailed information for a specific user.
        /// </summary>
        /// <param name="userId">The <see cref="Guid"/> representing the user's unique identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, 
        /// with a result of the <see cref="User"/> object containing user information.</returns>
        Task<User> GetUserInfoAsync(Guid userId);

        /// <summary>
        /// Updates the information of an existing user.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object containing updated information.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, 
        /// with a result of <c>true</c> if the user information was successfully updated; otherwise, <c>false</c>.</returns>
        Task<bool> UpdateUserInfoAsync(UserDetailsViewModel user);

        /// <summary>
        /// Maps a UserDetailsViewModel to a User entity and assigns the appropriate password.
        /// </summary>
        /// <param name="userVM">The view model containing user details.</param>
        /// <returns>The mapped User entity.</returns>
        User MapToUserEntity(UserDetailsViewModel userVM);

        /// <summary>
        /// Retrieves the current user from the session data.
        /// </summary>
        /// <returns>The current <see cref="User"/> object, or null if no user is found.</returns>
        User? OnGetCurrentUser();

        /// <summary>
        /// Retrieves the unique identifier of the user associated with the specified email address.
        /// </summary>
        /// <param name="email">The email address of the user to locate. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>A task whose result is the user's <see cref="Guid"/> if found; otherwise, <see langword="null"/>.</returns>
        Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<User>> GetUsersByRoleAsync(eSysRole role, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetUsersByRolesAsync(IEnumerable<eSysRole> roles, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<User>> GetUsersByDepartmentAsync(eDepartment department, CancellationToken cancellationToken = default);

        Task<User> GetDepartmentHeadAsync(eDepartment department, CancellationToken cancellationToken = default);
        Task<User> GetUserAdminRoleByDepartmentAsync(eDepartment department, CancellationToken cancellationToken = default);
        Task<bool> IsHeadOfDepartmentAsync(CancellationToken cancellationToken = default);
    }
}
