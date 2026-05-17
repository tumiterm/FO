// <copyright file="ApplicantUserService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2026 22:46 PM
// Purpose:         Defines the ApplicantUserService 

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Web.WebPages;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides user account management operations for applicant users, including registration, authentication,
    /// password changes, and profile updates.
    /// </summary>
    /// <remarks>This service encapsulates user-related functionality for applicants, such as registering new
    /// users, validating credentials, and updating user information. All operations are performed asynchronously. The
    /// service relies on an injected unit of work for data persistence and a logger for diagnostic purposes. Methods
    /// return validation responses that indicate success or provide error details for failed operations.</remarks>
    public class ApplicantUserService : IApplicantUserService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ApplicantUserService> _logger;
        #endregion

        /// <summary>
        /// Initializes a new instance of the ApplicantUserService class with the specified unit of work and logger.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance used to manage data persistence operations.</param>
        /// <param name="logger">The logger used to record diagnostic and operational information for the service.</param>
        /// <exception cref="ArgumentNullException">Thrown if unitOfWork or logger is null.</exception>
        public ApplicantUserService(IUnitOfWork unitOfWork, ILogger<ApplicantUserService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<ValidationResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResponse> GetUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResponse> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to register a new user asynchronously using the provided registration details.
        /// </summary>
        /// <remarks>If the request contains invalid or duplicate data, the response will include the
        /// relevant error messages. The operation does not persist the user if validation fails. This method performs
        /// both synchronous and asynchronous validation before attempting to save the user.</remarks>
        /// <param name="request">The registration information for the new user. Must contain all required fields and valid data.</param>
        /// <returns>A ValidationResponse indicating the result of the registration attempt. If registration is successful, the
        /// response contains no errors; otherwise, it contains validation or uniqueness error messages.</returns>
        public async Task<ValidationResponse> RegisterAsync(RegisterRequest request)
        {
            TrimRequestProperties(request);

            SplitIdOrPassport(request);

            var syncErrors = ValidateRequest(request);

            if (syncErrors.Any()) return new ValidationResponse(syncErrors.First());

            var uniquenessErrors = await ValidateUniquenessAsync(request);

            if (uniquenessErrors.Any())
            {
                return new ValidationResponse(string.Join(" ", uniquenessErrors));
            }

            var user = MapToOnlineApplicationUser(request);

            await _unitOfWork.OnlineApplicantUser.AddAsync(user);

            var saved = await _unitOfWork.SaveAsync();
            if (saved <= 0)
            {
                _logger.LogError("RegisterAsync failed to persist new OnlineApplicationUser. Username: {Username}", user.Username);
                return new ValidationResponse("Failed to register user.");
            }

            var payloadJson = JsonSerializer.Serialize(new { OnlineApplicationUserId = user.Id });

            var queueItem = new BackgroundJobQueueItem
            {
                Id = Guid.NewGuid(),
                Queue = "onlineapps",
                JobType = "OnlineApplicationSubmission",
                PayloadJson = payloadJson,
                Status = "Pending",
                Attempts = 0,
                DateCreated = DateTimeOffset.UtcNow,
                DateModified = DateTimeOffset.UtcNow,
                IsDeleted = false,
                UserCreated = user.Username,
                UserModified = user.Username,
                RowVersion = Array.Empty<byte>()
            };

            await _unitOfWork.BackgroundJobQueue.AddAsync(queueItem).ConfigureAwait(false);

            var queuedSaved = await _unitOfWork.SaveAsync().ConfigureAwait(false);
            if (queuedSaved <= 0)
            {
                _logger.LogError("RegisterAsync created user but failed to enqueue BackgroundJobQueueItem. UserId: {UserId}", user.Id);
                return new ValidationResponse("User registered but background processing could not be queued.");
            }

            _logger.LogInformation("RegisterAsync queued BackgroundJobQueueItem {QueueItemId} for user {UserId}", queueItem.Id, user.Id);


            return new ValidationResponse();
        }

        public Task<ValidationResponse> UpdateUserAsync(string userId, UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        /// <summary>
        /// Trims leading and trailing whitespace from selected string properties of the specified registration request.
        /// </summary>
        /// <remarks>This method replaces null property values with empty strings before trimming. Only
        /// the listed properties are affected; other properties of the request remain unchanged.</remarks>
        /// <param name="request">The registration request whose LastName, Username, IdNumber, PassportNumber, and FirstName properties will
        /// be trimmed. Cannot be null.</param>
        private void TrimRequestProperties(RegisterRequest request)
        {
            request.LastName = (request.LastName ?? string.Empty).Trim();
            request.Username = (request.Username ?? string.Empty).Trim();
            request.IdOrPassport = (request.IdOrPassport ?? string.Empty).Trim();

            request.IdNumber = (request.IdNumber ?? string.Empty).Trim();
            request.PassportNumber = (request.PassportNumber ?? string.Empty).Trim();
            request.FirstName = (request.FirstName ?? string.Empty).Trim();
        }

        /// <summary>
        /// Asynchronously validates that the username, ID number, and passport number in the registration request are
        /// unique.
        /// </summary>
        /// <remarks>This method checks for existing users with the same username, ID number, or passport
        /// number as provided in the request. Only non-empty ID number and passport number fields are validated for
        /// uniqueness.</remarks>
        /// <param name="request">The registration request containing the username, ID number, and passport number to validate for uniqueness.</param>
        /// <returns>A collection of error messages indicating which fields are not unique. The collection is empty if all fields
        /// are unique.</returns>
        private async Task<IEnumerable<string>> ValidateUniquenessAsync(RegisterRequest request)
        {
            var errors = new List<string>();

            if (await _unitOfWork.OnlineApplicantUser.ExistsAsync(x => x.Username == request.Username))
            {
                errors.Add("Username is already in use.");
            }

            if (!string.IsNullOrWhiteSpace(request.IdNumber) &&
                await _unitOfWork.OnlineApplicantUser.ExistsAsync(x => x.IdNumber == request.IdNumber))
            {
                errors.Add("ID Number is already registered.");
            }

            if (!string.IsNullOrWhiteSpace(request.PassportNumber) &&
                await _unitOfWork.OnlineApplicantUser.ExistsAsync(x => x.PassportNumber == request.PassportNumber))
            {
                errors.Add("Passport Number is already registered.");
            }

            return errors;
        }

        /// <summary>
        /// Creates a new instance of the OnlineApplicationUser class using the data provided in the specified
        /// registration request.
        /// </summary>
        /// <param name="request">The registration request containing user information to be mapped to an OnlineApplicationUser. Cannot be
        /// null.</param>
        /// <returns>An OnlineApplicationUser object populated with values from the registration request.</returns>
        private OnlineApplicationUser MapToOnlineApplicationUser(RegisterRequest request)
        {
            var entityId = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;

            return new OnlineApplicationUser
            {
                Id = entityId,
                LastName = request.LastName,
                IdNumber = string.IsNullOrWhiteSpace(request.IdNumber) ? null : request.IdNumber,
                PassportNumber = string.IsNullOrWhiteSpace(request.PassportNumber) ? null : request.PassportNumber,
                Username = request.Username,
                StudentNumber = null,
                Cellphone = request.Cellphone,
                Password = request.Password,
                Code = "Online",
                Name = request.FirstName,
                DateCreated = DateTimeOffset.UtcNow,
                DateModified = DateTimeOffset.UtcNow,
                IsDeleted = false,
                UserCreated = request.Username,
                UserModified = request.Username,
                RowVersion = Array.Empty<byte>()
            };
        }

        /// <summary>
        /// Validates the specified registration request and returns a collection of validation error messages, if any.
        /// </summary>
        /// <param name="request">The registration request to validate. Cannot be null.</param>
        /// <returns>An enumerable collection of strings containing validation error messages. The collection is empty if the
        /// request is valid.</returns>
        private IEnumerable<string> ValidateRequest(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LastName)) yield return "LastName is required.";
            if (string.IsNullOrWhiteSpace(request.Username)) yield return "Username is required.";
            if (string.IsNullOrWhiteSpace(request.IdNumber) && string.IsNullOrWhiteSpace(request.PassportNumber)) yield return "Either IdNumber or PassportNumber is required.";
            // Add more
        }

        private static void SplitIdOrPassport(RegisterRequest request)
        {
            var raw = (request.IdOrPassport ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(raw))
            {
                request.IdNumber = null;
                request.PassportNumber = null;
                return;
            }

            var normalized = raw.Replace(" ", string.Empty).Replace("-", string.Empty);

            var digitsOnly = normalized.All(char.IsDigit);
            var looksLikeSaId = digitsOnly && normalized.Length == 13;

            if (looksLikeSaId)
            {
                request.IdNumber = normalized;
                request.PassportNumber = null;
            }
            else
            {
                request.IdNumber = null;
                request.PassportNumber = normalized;
            }
        }
        #endregion
    }
}
