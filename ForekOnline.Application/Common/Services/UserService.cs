// <copyright file="UserService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    20/01/2023 15:00 PM
// Purpose:         Defines the UserService interface

#region Usings

using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using static ForekOnline.Domain.Enums.EnumRegistry;

#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides user-related operations.
    /// Ensures that all operations are validated and logged for traceability.
    /// </summary>
    public class UserService : IUserService
    {
        #region Privates
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _context;
        private readonly IBlobFileService _blobFileService;
        private IHelperService _helperService;
        private readonly string _containerName;
        private readonly IFileUploadService _fileUploadService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the UserService class with the specified dependencies.
        /// </summary>
        /// <param name="logger">The logger used to record diagnostic and operational information for the UserService.</param>
        /// <param name="context">The unit of work that manages data access and transactions for user-related operations.</param>
        /// <param name="blobFileService">The service used for managing blob file storage, such as user profile images.</param>
        /// <param name="helperService">The helper service that provides configuration values and utility functions required by the UserService.</param>
        /// <param name="httpContextAccessor">The accessor used to obtain information about the current HTTP context.</param>
        public UserService(ILogger<UserService> logger, IUnitOfWork context, IBlobFileService blobFileService, IHelperService helperService, IHttpContextAccessor httpContextAccessor, IFileUploadService fileUploadService)
        {
            _logger = logger;
            _context = context;
            _blobFileService = blobFileService;
            _helperService = helperService;
            _httpContextAccessor = httpContextAccessor;
            _containerName = _helperService.GetConfigurationValue("AzureStorage:Containers:ImageProfiles", string.Empty);
            _fileUploadService = fileUploadService;
        }

        #region Public

        /// <summary>
        /// Retrieves all users from the context.
        /// </summary>
        /// <returns>A list of users.</returns>
        public async Task<IReadOnlyList<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users.GetAllAsync();

                _logger.LogInformation("Successfully retrieved all users at {Time}", DateTime.UtcNow);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users at {Time}", DateTime.UtcNow);

                throw;
            }
        }

        /// <summary>
        /// Registers a new user and handles account creation.
        /// </summary>
        /// <param name="user">User to be registered.</param>
        /// <returns>A tuple containing registration status and a message.</returns>
        public async Task<(bool Status, string Message)> RegisterUserAsync(User user)
        {
            try
            {
                GenerateUserDetails(user);

                HashUserPassword(user);

                user.IsEmailVerified = false;

                user.IsActive = true;

                user.CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString();

                user.Role = eSysRole.Student;

                if (!await _context.Users.ExistsAsync(m => m.StudentNumber == user.StudentNumber))
                {
                    var userAddition = await _context.Users.AddAsync(user);

                    if (userAddition != null)
                    {
                        int rc = await _context.SaveAsync();

                        if (rc > 0)
                        {
                            // Uncomment the email sending logic when ready
                            // SendVerificationLinkEmail(user.Username, user.ActivationCode.ToString(), "", "VerifyAccount");

                            _logger.LogInformation("User successfully registered with email {Email} at {Time}", user.Username, DateTime.UtcNow);
                            return (true, "User Successfully Registered");
                        }
                    }
                }

                return (false, "Registration failed: User already exists or database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration at {Time}", DateTime.UtcNow);

                return (false, "An error occurred while registering the user.");
            }
        }

        /// <summary>
        /// Removes a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to be removed.</param>
        /// <returns>True if the user was removed; otherwise, false.</returns>
        public async Task<bool> RemoveUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            var user = await _context.Users.GetAsync(filter: u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            var isDeleteSuccessful = await _context.Users.RemoveAsync(user);

            return isDeleteSuccessful;
        }

        /// <summary>
        /// Retrieves user information by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The User object if found; otherwise, null.</returns>
        public async Task<User> GetUserInfoAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            return await _context.Users.GetAsync(filter: u => u.Id == userId);
        }

        /// <summary>
        /// Updates user information in the database based on the provided user details view model.
        /// </summary>
        /// <param name="userVM">The view model containing user details to be updated.</param>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// Returns <c>true</c> if the user was updated successfully, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="userVM"/> is null
        public async Task<bool> UpdateUserInfoAsync(UserDetailsViewModel userVM)
        {
            if (userVM == null)
            {
                throw new ArgumentNullException(nameof(userVM));
            }

            User user = MapToUserEntity(userVM);

            if (!string.IsNullOrEmpty(userVM.NewPassword))
            {
                user.Password = Helper.ValueEncryption(user.Password);
            }

            if (string.IsNullOrEmpty(userVM.ConfirmPassword))
            {
                user.ConfirmPassword = Helper.ValueEncryption(user.Password);
            }

            await UploadProfileImageAsync(user, CancellationToken.None);

            var updatedUser = await _context.Users.UpdateUserAsync(user);

            return updatedUser != null;
        }

        /// <summary>
        /// Maps a UserDetailsViewModel to a User entity and assigns the appropriate password.
        /// </summary>
        /// <param name="userVM">The view model containing user details.</param>
        /// <returns>The mapped User entity.</returns>
        public User MapToUserEntity(UserDetailsViewModel userVM)
        {
            return new User
            {
                ModifiedBy = Helper.loggedInUser,
                ModifiedOn = Helper.OnGetCurrentDateTime(),
                Username = userVM.Email,
                Cellphone = userVM.Cellphone,
                Password = GetPassword(userVM),
                Department = userVM.Department,
                IDPass = userVM.IDPass,
                IsActive = userVM.IsActive,
                LastName = userVM.LastName,
                Name = userVM.Name,
                Role = userVM.Role,
                LastLoginDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                Id = userVM.Id,
                LastActivityDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                ProfileImage = userVM.ProfileImage,
                ProfileImageFile = userVM.ProfileImageFile,
                ConfirmPassword = userVM.ConfirmPassword,
                StudentNumber = userVM.StudentNumber, 
                EmailSignatureLink = userVM.EmailSignatureLink,
                CreatedOn = userVM.CreatedOn,
                CreatedBy = userVM.CreatedBy,
            };
        }

        /// <summary>
        /// Retrieves the current user from the session data.
        /// </summary>
        /// <returns>The current <see cref="User"/> object, or null if no user is found.</returns>
        public User? OnGetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                _logger.LogWarning("HttpContext is unavailable.");

                return null;
            }

            string? sessionUserJson = _httpContextAccessor.HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrWhiteSpace(sessionUserJson))
            {
                _logger.LogInformation("No session data found for 'SessionUser'.");

                return null;
            }

            try
            {
                User? user = JsonConvert.DeserializeObject<User>(sessionUserJson);

                if (user == null)
                {
                    _logger.LogWarning("Deserialization returned a null User object.");

                    return null;
                }

                return user;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize 'SessionUser' JSON string.");

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the current user.");

                return null;
            }
        }

        /// <summary>
        /// Retrieves the unique identifier of the user associated with the specified email address.
        /// </summary>
        /// <param name="email">The email address of the user to locate. Cannot be null, empty, or whitespace.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>The user's <see cref="Guid"/> if found; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="email"/> is null, empty, whitespace, or not a valid email format.</exception>
        public async Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (!ValidationHelper.IsValidEmailAddress(normalizedEmail))
            {
                throw new ArgumentException("Invalid email format.", nameof(email));
            }

            try
            {
                var user = await _context.Users.GetAsync(
                    filter: u => u.Username != null && u.Username.ToLower() == normalizedEmail,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                if (user == null)
                {
                    _logger.LogInformation("No user found for email {Email} at {Time}", normalizedEmail, DateTime.UtcNow);
                    return null;
                }

                _logger.LogInformation("User {UserId} resolved for email {Email} at {Time}", user.Id, normalizedEmail, DateTime.UtcNow);
                return user.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resolve user by email {Email} at {Time}", normalizedEmail, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(eSysRole role, CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _context.Users.GetAllAsync(
                    filter: u => u.Role == role,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);
                _logger.LogInformation("Successfully retrieved users with role {Role} at {Time}", role, DateTime.UtcNow);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users with role {Role} at {Time}", role, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<User>> GetUsersByRolesAsync(IEnumerable<eSysRole> roles, CancellationToken cancellationToken = default)
        {
            try
            {
                var roleList = roles?.ToList() ?? new List<eSysRole>();

                if (!roleList.Any())
                    return Array.Empty<User>();

                var users = await _context.Users.GetAllAsync(
                    filter: u => roleList.Contains(u.Role.Value),
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Successfully retrieved users with roles {Roles} at {Time}",
                    string.Join(", ", roleList),
                    DateTime.UtcNow);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while retrieving users with roles {Roles} at {Time}",
                    roles,
                    DateTime.UtcNow);

                throw;
            }
        }

        public async Task<IReadOnlyList<User>> GetUsersByDepartmentAsync(eDepartment department, CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _context.Users.GetAllAsync(
                    filter: u => u.Department == department,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);
                _logger.LogInformation("Successfully retrieved users in department {Department} at {Time}", department, DateTime.UtcNow);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users in department {Department} at {Time}", department, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<User> GetUserAdminRoleByDepartmentAsync(eDepartment department,CancellationToken cancellationToken)
        {
            try
            {
               var user = await _context.Users.GetAsync(
                   filter: u => u.Department == department && u.Role == eSysRole.Admin,
                   asNoTracking: true,
                   cancellationToken: cancellationToken);

                return user ?? throw new InvalidOperationException($"No admin user found for department {department}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the department head for {Department} at {Time}", department, DateTime.UtcNow);
                throw;
            }
        }
        public async Task<User> GetDepartmentHeadAsync(eDepartment department, CancellationToken cancellationToken = default)
        {
            User? user = new();

            try
            {
                 user = await _context.Users.GetAsync(
                    filter: u => u.Department == department && u.IsHeadOfDepartment,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);

                if (user != null)
                {
                    _logger.LogInformation("Successfully retrieved department head for {Department} at {Time}", department, DateTime.UtcNow);
                }
                else
                {
                   user = await GetUserAdminRoleByDepartmentAsync(department, cancellationToken);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the department head for {Department} at {Time}", department, DateTime.UtcNow);
                throw;
            }
        }
        public async Task<bool> IsHeadOfDepartmentAsync(CancellationToken cancellationToken = default)
        {
            var loggedInUser = OnGetCurrentUser();

            if (loggedInUser == null)
            {
                return false;
            }

            var user = await _context.Users.GetAsync(
                       filter: u => u.Department == loggedInUser.Department && u.IsHeadOfDepartment,
                       asNoTracking: true,
                       cancellationToken: cancellationToken);

            return user != null && user.Id == loggedInUser.Id;
        }
        #endregion

        #region Private

        /// <summary>
        /// Generate details for user such as activation code and unique identifiers.
        /// </summary>
        private void GenerateUserDetails(User user)
        {
            user.ActivationCode = Helper.GenerateGuid();

            user.LastLoginDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            user.ResetPasswordCode = string.Empty;

            user.Id = Helper.GenerateGuid();
        }

        /// <summary>
        /// Hashes the user's password and confirmation password.
        /// </summary>
        private void HashUserPassword(User user)
        {
            user.Password = Helper.ValueEncryption(user.Password);

            //user.ConfirmPassword = Helper.ValueEncryption(user.ConfirmPassword);
        }

        /// <summary>
        /// Determines the password to be used based on the NewPassword Field in the view model.
        /// </summary>
        /// <param name="userVM">The view model containing password details.</param>
        /// <returns>
        /// The new password if provided; otherwise, the old password.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="userVM"/> is null.</exception>
        private string GetPassword(UserDetailsViewModel userVM)
        {
            if (userVM == null)
            {
                throw new ArgumentNullException(nameof(userVM));
            }

            return string.IsNullOrWhiteSpace(userVM.NewPassword)
                ? userVM.OldPassword
                : userVM.NewPassword;
        }

        /// <summary>
        /// Uploads the user's profile image asynchronously if a valid image file is provided.
        /// </summary>
        /// <remarks>If the user's ProfileImageFile property is null or empty, the method completes
        /// without performing an upload. Upon successful upload, the user's ProfileImage property is updated with the
        /// identifier of the uploaded file.</remarks>
        /// <param name="user">The user whose profile image is to be uploaded. The user's ProfileImageFile property must not be null and
        /// must contain data for the upload to occur.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous upload operation. The task completes when the upload is finished or
        /// if no upload is performed.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the user parameter is null.</exception>
        private async Task UploadProfileImageAsync(User user, CancellationToken ct)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.ProfileImageFile is null || user.ProfileImageFile.Length <= 0)
            {
                return;
            }

            await using var stream = user.ProfileImageFile.OpenReadStream();

            var uploadResponse = await _fileUploadService.UploadAsync(
                new UploadFileRequest(
                    FileStream: stream,
                    FileName: user.ProfileImageFile.FileName,
                    ContentType: user.ProfileImageFile.ContentType,
                    Metadata: new Dictionary<string, string>
                    {
                        ["Entity"] = "User",
                        ["UserId"] = user.Id.ToString("D")
                    },
                    ProviderHint: null,
                    ExpiryDate: null,
                    TenantId: null,
                    DocumentType: "User"),
                ct).ConfigureAwait(false);

            user.ProfileImage = uploadResponse.FileId;
        }

       

        #endregion
    }
}
