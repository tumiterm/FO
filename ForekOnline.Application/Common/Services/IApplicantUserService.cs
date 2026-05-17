using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.ViewModels;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines methods for managing applicant user accounts, including registration, authentication, profile retrieval,
    /// profile updates, and password changes.
    /// </summary>
    /// <remarks>Implementations of this interface provide asynchronous operations for common user account
    /// management tasks. All methods return a ValidationResponse that indicates the success or failure of the operation
    /// and may include validation errors or additional information relevant to the request.</remarks>
    public interface IApplicantUserService
    {
        /// <summary>
        /// Asynchronously registers a new user account using the specified registration details.
        /// </summary>
        /// <param name="request">An object containing the user's registration information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether the registration was successful and any validation errors encountered.</returns>
        Task<ValidationResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Asynchronously attempts to authenticate a user with the provided login credentials.
        /// </summary>
        /// <param name="request">An object containing the user's login credentials and any additional authentication data. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether authentication was successful and providing any relevant validation details.</returns>
        Task<ValidationResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Asynchronously retrieves the user associated with the specified user identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse with the
        /// user information if found; otherwise, indicates the reason for failure.</returns>
        Task<ValidationResponse> GetUserAsync(string userId);

        /// <summary>
        /// Asynchronously updates the details of an existing user with the specified information.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to update. Cannot be null or empty.</param>
        /// <param name="request">An object containing the updated user information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// the outcome of the update operation, including any validation errors.</returns>
        Task<ValidationResponse> UpdateUserAsync(string userId, UpdateUserRequest request);

        /// <summary>
        /// Asynchronously changes the password for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose password is to be changed. Cannot be null or empty.</param>
        /// <param name="request">An object containing the current and new password information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether the password change was successful and any validation errors that occurred.</returns>
        Task<ValidationResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    }
}
