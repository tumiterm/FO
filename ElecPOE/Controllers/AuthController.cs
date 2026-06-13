// <copyright file="AuthController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    20/01/2023 13:09:27 PM
// Purpose:         Defines the AuthController class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using ForekOnline.Domain.Security;
using ForekOnline.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Provides authentication and user management actions for the application, including user registration, sign-in,
    /// profile management, and administrative operations such as viewing, updating, and removing users.
    /// </summary>
    /// <remarks>This controller exposes endpoints for both authenticated and anonymous users, supporting
    /// common authentication workflows such as sign-in, sign-up, logout, and password recovery. Administrative actions
    /// are protected by role-based authorization and allow management of user accounts. The controller relies on
    /// injected services for user data access, logging, file uploads, and configuration. Most actions return views or
    /// redirect results appropriate for web applications using ASP.NET Core MVC. Thread safety and concurrency are
    /// managed by ASP.NET Core's request handling; controller instances are scoped per request.</remarks>
    public class AuthController : Controller
    {
        #region Private Variables
        private IConfiguration _configuration;
        private readonly IUnitOfWork _context;
        private IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<AuthController> _logger;
        private readonly string _studentApiUrl;
        private readonly IUserService _userService;
        private readonly IHelperService _helperService;
        private readonly IFileUploadService _fileUploadService;
        private const string LoginSessionKeyClaim = "LoginSessionKey";
        private readonly ILoginHistoryService _loginHistoryService;
        private readonly ITenantContext _tenantContext;
        private readonly ApplicationDbContext _db;
        #endregion

        /// <summary>
        /// Initializes a new instance of the AuthController class with the specified dependencies required for
        /// authentication and user-related operations.
        /// </summary>
        /// <remarks>All parameters are required and must not be null. This constructor is typically used
        /// by dependency injection to provide the necessary services for the controller's operation.</remarks>
        /// <param name="context">The unit of work instance used to manage data access and transactions.</param>
        /// <param name="hostEnvironment">Provides information about the web hosting environment, such as content root and environment name.</param>
        /// <param name="logger">The logger used for logging authentication-related events and errors.</param>
        /// <param name="httpClientFactory">The factory used to create HttpClient instances for making external HTTP requests.</param>
        /// <param name="configuration">The application configuration settings provider.</param>
        /// <param name="userService">The service used for user management and authentication operations.</param>
        /// <param name="helperService">The helper service used for retrieving configuration values and utility functions.</param>
        /// <param name="fileUploadService">The service responsible for handling file uploads.</param>
        public AuthController(IUnitOfWork context, ILoginHistoryService loginHistoryService,
                                IWebHostEnvironment hostEnvironment,
                                ILogger<AuthController> logger, ApplicationDbContext db,
                                IHttpClientFactory httpClientFactory, IConfiguration configuration, IUserService userService, IHelperService helperService, IFileUploadService fileUploadService, ITenantContext tenantContext)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _helperService = helperService;
            _studentApiUrl = _helperService.GetConfigurationValue("ApiSettings:StudentApiUrl", "Unknown");
            _userService = userService;
            _fileUploadService = fileUploadService;
            _tenantContext = tenantContext;
            _loginHistoryService = loginHistoryService ?? throw new ArgumentNullException(nameof(loginHistoryService));
            _db = db;
        }

        /// <summary>
        /// Displays user information.
        /// </summary>
        /// <param name="id">The ID of the user to view.</param>
        /// <returns>An ActionResult to render the view.</returns>
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> OnViewUserInfo(Guid Id)
        {
            try
            {
                var user = await _userService.GetUserInfoAsync(Id);

                if(user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found at {Time}", Id, DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);
                    throw new InvalidOperationException("User not found.");
                }

                ViewData["Password"] = user.Password;
                ViewData["CPassword"] = user.ConfirmPassword;
                ViewData["user"] = $"{user.Name} {user.LastName}";
                ViewData["studNum"] = $"{user.Name} {user.LastName}";

                var mapUserDetails = MapUserDetails(user);
                var actorId = CurrentUserId();
                mapUserDetails.IsViewingSelf = actorId == user.Id;
                mapUserDetails.CanDirectDowngrade = mapUserDetails.IsViewingSelf && RoleTransitionPolicy.IsPrivileged(user.Role);
                mapUserDetails.RoleHistory = await _db.UserRoleHistory
                    .Where(x => x.UserId == user.Id)
                    .OrderByDescending(x => x.ChangedUtc)
                    .Take(20)
                    .ToListAsync();
                mapUserDetails.RoleRequests = await _db.RoleAccessRequests
                    .Where(x => x.TargetUserId == user.Id)
                    .OrderByDescending(x => x.RequestedUtc)
                    .Take(10)
                    .ToListAsync();
                mapUserDetails.HasPendingRoleRequest = mapUserDetails.RoleRequests.Any(x => x.Status == RoleAccessRequestStatus.Pending);

                return View(mapUserDetails);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Attempted to view user info with empty ID at {Time}", DateTime.UtcNow);

                return RedirectToAction("PageNotFound", "Global");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user info for ID {UserId} at {Time}", Id, DateTime.UtcNow);

                return View("Error");
            }
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <param name="user">The User object with updated details.</param>
        /// <returns>An ActionResult indicating the outcome.</returns>
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnViewUserInfo(UserDetailsViewModel user)
        {
            if (user.ProfileImageFile is { Length: > 0 })
            {
                const long maxProfileImageSize = 5 * 1024 * 1024;
                var allowedImageTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "image/jpeg",
                    "image/png",
                    "image/webp"
                };

                if (user.ProfileImageFile.Length > maxProfileImageSize)
                {
                    ModelState.AddModelError(nameof(user.ProfileImageFile), "The profile picture must be 5 MB or smaller.");
                }

                if (!allowedImageTypes.Contains(user.ProfileImageFile.ContentType))
                {
                    ModelState.AddModelError(nameof(user.ProfileImageFile), "Choose a JPG, PNG, or WebP image.");
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = ModelState[nameof(user.ProfileImageFile)]?.Errors.FirstOrDefault()?.ErrorMessage
                    ?? "Please fill in all the fields!";
                return RedirectToAction(nameof(OnViewUserInfo), new { Id = user.Id });
            }

            try
            {
                var storedUser = await _db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == user.Id);
                if (storedUser == null)
                {
                    return NotFound();
                }

                user.Role = storedUser.Role;
                user.OldPassword = storedUser.Password;
                user.ConfirmPassword = storedUser.ConfirmPassword;
                user.ProfileImage = storedUser.ProfileImage;

                bool isUpdated = await _userService.UpdateUserInfoAsync(user);

                if (isUpdated)
                {
                    TempData["success"] = "User details successfully saved";
                    return RedirectToAction(nameof(RetrieveUsers)); 

                }
                else
                {
                    TempData["error"] = "Unable to save user details!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user info for ID {UserId} at {Time}", user.Id, DateTime.UtcNow);

                TempData["error"] = "An error occurred while saving user details.";
            }

            return RedirectToAction(nameof(RetrieveUsers));
        }

        /// <summary>
        /// Allows an admin to reset a user's password without requiring the old password.
        /// Ensures proper audit logging and access control.
        /// </summary>
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetUserPassword(Guid userId, string newPassword)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["error"] = "Invalid user or password provided.";
                return RedirectToAction(nameof(RetrieveUsers));
            }

            try
            {
                var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    return NotFound();
                }

                user.Password = Helper.ValueEncryption(newPassword);
                user.ConfirmPassword = Helper.ValueEncryption(newPassword);
                user.ModifiedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString("0");
                user.ModifiedBy = Helper.loggedInUser;

                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Admin {AdminId} reset password for user {UserId} at {Time}",
                    CurrentUserId(),
                    userId,
                    DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);

                TempData["success"] = "User password has been reset successfully.";
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cross-tenant password reset attempt for user {UserId} at {Time}", userId, DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);
                TempData["error"] = "Cannot reset password for this user.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting password for user {UserId} at {Time}", userId, DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);
                TempData["error"] = "An error occurred while resetting the password.";
            }

            return RedirectToAction(nameof(OnViewUserInfo), new { Id = userId });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestRoleAccess(RoleAccessRequestInput input)
        {
            var actorId = CurrentUserId();
            var actor = await _db.Users.SingleAsync(x => x.Id == actorId);
            var target = await _db.Users.SingleOrDefaultAsync(x => x.Id == input.TargetUserId);
            if (target == null)
                return NotFound();

            if (actorId != target.Id && !RoleTransitionPolicy.CanRequestForAnother(actor.Role))
                return Forbid();
            if (actorId == target.Id && target.Role == eSysRole.Admin && input.RequestedRole == eSysRole.SuperAdmin)
            {
                TempData["error"] = "Administrators cannot elevate themselves to SuperAdmin.";
                return RedirectToAction(nameof(OnViewUserInfo), new { Id = target.Id });
            }
            if (!RoleTransitionPolicy.RequiresApproval(target.Role, input.RequestedRole))
            {
                TempData["error"] = "This transition is a downgrade. Use the confirmed downgrade action.";
                return RedirectToAction(nameof(OnViewUserInfo), new { Id = target.Id });
            }
            if (!ModelState.IsValid || input.RequestedRole == eSysRole.None)
            {
                TempData["error"] = "Choose a valid role and provide a reason.";
                return RedirectToAction(nameof(OnViewUserInfo), new { Id = target.Id });
            }
            if (await _db.RoleAccessRequests.AnyAsync(x => x.TargetUserId == target.Id && x.Status == RoleAccessRequestStatus.Pending))
            {
                TempData["error"] = "This user already has a pending role request.";
                return RedirectToAction(nameof(OnViewUserInfo), new { Id = target.Id });
            }

            _db.RoleAccessRequests.Add(new RoleAccessRequest
            {
                Id = Guid.NewGuid(),
                TenantId = target.TenantId,
                TargetUserId = target.Id,
                RequestedByUserId = actorId,
                FromRole = target.Role,
                RequestedRole = input.RequestedRole,
                Reason = input.Reason.Trim(),
                RequestedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                IsDeleted = false
            });
            await _db.SaveChangesAsync();
            TempData["success"] = "Role access request submitted for approval.";
            return RedirectToAction(nameof(OnViewUserInfo), new { Id = target.Id });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DowngradeOwnRole(DirectRoleDowngradeInput input)
        {
            var actorId = CurrentUserId();
            if (actorId != input.TargetUserId)
                return Forbid();

            var actor = await _db.Users.SingleAsync(x => x.Id == actorId);
            if (!input.ConfirmLockoutRisk || !RoleTransitionPolicy.CanDirectlyDowngradeSelf(actor.Role, input.NewRole))
            {
                TempData["error"] = "The downgrade requires explicit confirmation and a lower-power role.";
                return RedirectToAction(nameof(OnViewUserInfo), new { Id = actorId });
            }

            var otherPrivilegedUsers = await _db.Users.CountAsync(x =>
                x.Id != actorId && x.IsActive && (x.Role == eSysRole.Admin || x.Role == eSysRole.SuperAdmin));
            if (otherPrivilegedUsers == 0)
            {
                TempData["error"] = "You are the last active administrator. Assign another Admin or SuperAdmin first.";
                return RedirectToAction(nameof(OnViewUserInfo), new { Id = actorId });
            }

            var oldRole = actor.Role;
            actor.Role = input.NewRole;
            actor.ModifiedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime.ToString("O");
            _db.UserRoleHistory.Add(NewRoleHistory(actor, actorId, oldRole, input.NewRole, input.Reason, null));
            await _db.SaveChangesAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            TempData["success"] = "Your role was downgraded. Sign in again to refresh your access.";
            return RedirectToAction(nameof(SignIn));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> RoleAccessRequests()
        {
            var model = new RoleAccessManagementViewModel
            {
                Pending = await _db.RoleAccessRequests
                    .Include(x => x.TargetUser).Include(x => x.RequestedByUser)
                    .Where(x => x.Status == RoleAccessRequestStatus.Pending)
                    .OrderBy(x => x.RequestedUtc).ToListAsync(),
                Recent = await _db.RoleAccessRequests
                    .Include(x => x.TargetUser).Include(x => x.RequestedByUser).Include(x => x.ReviewedByUser)
                    .Where(x => x.Status != RoleAccessRequestStatus.Pending)
                    .OrderByDescending(x => x.ReviewedUtc).Take(30).ToListAsync()
            };
            return View(model);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRoleAccess(RoleRequestDecisionInput input)
        {
            var reviewerId = CurrentUserId();
            var reviewer = await _db.Users.SingleAsync(x => x.Id == reviewerId);
            var request = await _db.RoleAccessRequests.Include(x => x.TargetUser)
                .SingleOrDefaultAsync(x => x.Id == input.RequestId && x.Status == RoleAccessRequestStatus.Pending);
            if (request == null)
                return NotFound();
            if (request.RequestedByUserId == reviewerId || request.TargetUserId == reviewerId)
            {
                TempData["error"] = "Role requests require an independent approver.";
                return RedirectToAction(nameof(RoleAccessRequests));
            }
            if (!RoleTransitionPolicy.CanApprove(reviewer.Role, request.RequestedRole))
                return Forbid();

            var oldRole = request.TargetUser.Role;
            request.TargetUser.Role = request.RequestedRole;
            request.Status = RoleAccessRequestStatus.Approved;
            request.ReviewedByUserId = reviewerId;
            request.ReviewedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            request.ReviewNote = input.ReviewNote;
            _db.UserRoleHistory.Add(NewRoleHistory(request.TargetUser, reviewerId, oldRole, request.RequestedRole, request.Reason, request.Id));
            await _db.SaveChangesAsync();
            TempData["success"] = "Role request approved and applied.";
            return RedirectToAction(nameof(RoleAccessRequests));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRoleAccess(RoleRequestDecisionInput input)
        {
            var reviewerId = CurrentUserId();
            var reviewer = await _db.Users.SingleAsync(x => x.Id == reviewerId);
            var request = await _db.RoleAccessRequests.SingleOrDefaultAsync(x => x.Id == input.RequestId && x.Status == RoleAccessRequestStatus.Pending);
            if (request == null)
                return NotFound();
            if (request.RequestedByUserId == reviewerId || request.TargetUserId == reviewerId)
            {
                TempData["error"] = "Role requests require an independent reviewer.";
                return RedirectToAction(nameof(RoleAccessRequests));
            }
            if (!RoleTransitionPolicy.CanApprove(reviewer.Role, request.RequestedRole))
                return Forbid();
            request.Status = RoleAccessRequestStatus.Rejected;
            request.ReviewedByUserId = reviewerId;
            request.ReviewedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            request.ReviewNote = input.ReviewNote;
            await _db.SaveChangesAsync();
            TempData["success"] = "Role request rejected.";
            return RedirectToAction(nameof(RoleAccessRequests));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemindRoleApprovers(Guid requestId)
        {
            var request = await _db.RoleAccessRequests.SingleOrDefaultAsync(x => x.Id == requestId && x.Status == RoleAccessRequestStatus.Pending);
            if (request == null)
                return NotFound();
            request.LastReminderUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            request.ReminderCount++;
            await _db.SaveChangesAsync();
            TempData["success"] = "The approval reminder was recorded.";
            return RedirectToAction(nameof(RoleAccessRequests));
        }

        private Guid CurrentUserId() =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                ? id
                : throw new UnauthorizedAccessException("The current user identifier is unavailable.");

        private static UserRoleHistory NewRoleHistory(User target, Guid actorId, eSysRole? fromRole, eSysRole? toRole, string reason, Guid? requestId) =>
            new()
            {
                Id = Guid.NewGuid(),
                TenantId = target.TenantId,
                UserId = target.Id,
                ChangedByUserId = actorId,
                FromRole = fromRole,
                ToRole = toRole,
                ChangedUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime, 
                Reason = reason.Trim(),
                RoleAccessRequestId = requestId, IsDeleted = false
            };

        /// <summary>
        /// Removes a user with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to remove.</param>
        /// <returns>Action result indicating the outcome.</returns>
        [Authorize]
        [Route("/Auth/RemoveUser/{Id}")]
        public async Task<IActionResult> RemoveUser(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            try
            {
                bool isRemoved = await _userService.RemoveUserAsync(Id);

                if (isRemoved)
                {
                    _logger.LogInformation("User with ID {UserId} successfully removed at {Time}", Id, DateTime.UtcNow);

                    return CreatedAtAction("RemoveUser", new { Id = Id });
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Attempted to remove user with empty ID at {Time}", DateTime.UtcNow);

                return RedirectToAction("RouteNotFound", "Global");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Attempted to remove non-existent user with ID {UserId} at {Time}", Id, DateTime.UtcNow);

                return RedirectToAction("RouteNotFound", "Global");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while removing user with ID {UserId} at {Time}", Id, DateTime.UtcNow);

                return View("Error");
            }

            return View();
        }

        /// <summary>
        /// Displays the sign-up view for new user registration.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the sign-up page to the client.</returns>
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        /// <summary>
        /// Handles user registration and displays the result.
        /// </summary>
        /// <param name="user">User details for registration.</param>
        /// <returns>View with registration status.</returns>
        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Registration failed: Invalid model data.";

                ViewBag.Status = false;

                return View(user);
            }

            try
            {
                var (status, message) = await _userService.RegisterUserAsync(user);

                ViewBag.Message = message;

                ViewBag.Status = status;

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration at {Time}", DateTime.UtcNow);

                ViewBag.Message = "An unexpected error occurred.";

                ViewBag.Status = false;

                return View(user);
            }
        }

        /// <summary>
        /// Displays the sign-in view to the user.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the sign-in view.</returns>
        public IActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// Handles the user sign-in process, including validation, authentication, and redirection.
        /// </summary>
        /// <param name="user">The user details provided for sign-in.</param>
        /// <param name="returnUrl">An optional return URL to redirect the user after sign-in.</param>
        /// <returns>An IActionResult representing the result of the sign-in process.</returns>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(UserViewModel user, string? returnUrl = "")
        {
            if (string.IsNullOrEmpty(user.StudentNumber))
            {
                return HandleSignInError("Invalid or Empty Email Provided!");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                return HandleSignInError("Invalid or Empty Password!");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var userInfo = await GetActiveUserAsync(user.StudentNumber, user.Password);

            if (userInfo == null)
            {
                return HandleSignInError("Invalid email or password.");
            }

            if (userInfo.IsActive == false)
            {
                return RedirectToAction("BlockUserAccount", "Home");
            }

            //userInfo.LastLoginDate = DateTime.UtcNow.ToShortDateString();

            await SignInUserAsync(userInfo, user.RememberMe);

            SetUserSession(userInfo);

            return RedirectUserBasedOnRole(userInfo, returnUrl);
        }

        /// <summary>
        /// Retrieves and displays all users.
        /// </summary>
        /// <returns>A view displaying the list of users.</returns>
        [HttpGet]
        public async Task<IActionResult> RetrieveUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                _logger.LogInformation("Successfully retrieved all users at {Time}", DateTime.UtcNow);

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users at {Time}", DateTime.UtcNow);

                throw;
            }
        }

        /// <summary>
        /// Retrieves the profile image file associated with the specified file identifier for inline display.
        /// </summary>
        /// <remarks>The returned file is served for inline display and is cached on the client for up to
        /// one hour. The content type is determined from the file metadata; if unavailable, 'application/octet-stream'
        /// is used.</remarks>
        /// <param name="fileId">The unique identifier of the profile image file to retrieve. Cannot be null, empty, or whitespace.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the profile image file stream if found; otherwise, a NotFound
        /// result if the file identifier is invalid.</returns>
        [HttpGet]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> ProfileImage(string fileId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return NotFound();
            }

            var download = await _fileUploadService.DownloadAsync(fileId, ct).ConfigureAwait(false);

            return File(download.FileStream, download.ContentType ?? "application/octet-stream");
        }

        /// <summary>
        /// Retrieves a list of active facilitators.
        /// </summary>
        /// <returns>A list of active users with the Facilitator role.</returns>
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IEnumerable<User>> Facilitator()
        {
            try
            {
                var users = await _context.Users.GetAllAsync(); 

                IEnumerable<User> facilitators = users.Where(n => n.IsActive && n.Role == eSysRole.Facilitator);

                _logger.LogInformation("Successfully retrieved active facilitators at {Time}", DateTime.UtcNow);

                return facilitators;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving facilitators at {Time}", DateTime.UtcNow);

                throw;
            }
        }

        /// <summary>
        /// Logs out the current user by clearing the session and signing them out of the authentication scheme.
        /// </summary>
        /// <returns>A redirect to the SignIn action on successful logout.</returns>
        public async Task<IActionResult> Logout()
        {
            try
            {
                var sessionKey = User.FindFirstValue(LoginSessionKeyClaim);

                if (!string.IsNullOrWhiteSpace(sessionKey))
                {
                    var row = await _context.UserLoginHistories.GetAsync(
                        h => h.SessionKey == sessionKey && h.LogoutTimeUtc == null,
                        asNoTracking: false,
                        cancellationToken: HttpContext.RequestAborted);

                    if (row is not null)
                    {
                        row.LogoutTimeUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
                        row.LastActivityUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
                        row.IsCurrentSession = false;
                        row.LogoutReason = "Explicit";
                        await _context.SaveAsync();
                    }
                }

                ClearUserSession(HttpContext);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation("User successfully logged out at {Time}", DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout at {Time}", DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime);
                throw;
            }

            return RedirectToAction("SignIn", "Auth");
        }

        /// <summary>
        /// Updates the last activity timestamp for the current user's session if the session is active.
        /// </summary>
        /// <remarks>This method requires the user to be authenticated and a valid anti-forgery token. If
        /// the session is not active or cannot be found, no update is performed.</remarks>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="NoContentResult"/> indicating that the request was processed, regardless of whether the session
        /// was updated.</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SessionPing(CancellationToken ct)
        {
            var sessionKey = User.FindFirstValue(LoginSessionKeyClaim);
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                return NoContent();
            }

            var row = await _context.UserLoginHistories.GetAsync(
                h => h.SessionKey == sessionKey && h.LogoutTimeUtc == null,
                asNoTracking: false,
                cancellationToken: ct);

            if (row is null)
            {
                return NoContent();
            }

            row.LastActivityUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            await _context.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// Displays the enhanced login history dashboard (Admin).
        /// Delegates all logic to <see cref="ILoginHistoryService"/>.
        /// </summary>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<IActionResult> LoginHistory(int take = 200, string filter = "all", DateOnly? from = null, DateOnly? to = null)
        {
            var dashboard = await _loginHistoryService.GetDashboardAsync(take, filter, from, to, HttpContext.RequestAborted);

            return View(dashboard);
        }

        /// <summary>
        /// CSV export of login history data.
        /// </summary>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<IActionResult> ExportLoginHistory(DateOnly? from = null, DateOnly? to = null, string format = "csv")
        {
            var rows = await _loginHistoryService.GetExportDataAsync(from, to, HttpContext.RequestAborted);

            if (string.Equals(format, "json", StringComparison.OrdinalIgnoreCase))
            {
                return Json(rows);
            }

            var sb = new StringBuilder();
            sb.AppendLine("Id,UserId,DisplayName,Email,Role,SessionKey,LoginTimeUtc,LogoutTimeUtc,LastActivityUtc,Duration,IpAddress,UserAgent,DeviceType,Browser,IsCurrentSession,ForceLogoutPerformed,LogoutReason,Status");

            foreach (var r in rows)
            {
                var s = r.Session;
                sb.AppendLine(string.Join(",",
                    Csv(s.Id.ToString()),
                    Csv(s.UserId.ToString()),
                    Csv(r.DisplayName),
                    Csv(r.Email ?? ""),
                    Csv(r.Role?.ToString() ?? ""),
                    Csv(s.SessionKey ?? ""),
                    Csv(s.LoginTimeUtc.ToString("u")),
                    Csv(s.LogoutTimeUtc?.ToString("u") ?? ""),
                    Csv(s.LastActivityUtc?.ToString("u") ?? ""),
                    Csv(r.DurationDisplay),
                    Csv(s.IpAddress ?? ""),
                    Csv(s.UserAgent ?? ""),
                    Csv(s.DeviceType ?? ""),
                    Csv(s.Browser ?? ""),
                    Csv(s.IsCurrentSession.ToString()),
                    Csv(s.ForceLogoutPerformed?.ToString() ?? ""),
                    Csv(s.LogoutReason ?? ""),
                    Csv(r.Status)));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"LoginHistory_{DateTimeHelper.GetCurrentSastDateTimeOffset():yyyyMMdd_HHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }

        /// <summary>
        /// AJAX endpoint: returns the count of recent logins for the live ticker.
        /// </summary>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<IActionResult> RecentLoginCount(int minutes = 5, CancellationToken ct = default)
        {
            var count = await _loginHistoryService.GetRecentLoginCountAsync(minutes, ct);
            return Json(new { count });
        }

        /// <summary>
        /// Forcibly logs out a user session identified by the specified session key. Only accessible to users with
        /// SuperAdmin or Admin roles.
        /// </summary>
        /// <remarks>This action immediately ends the specified active session and records the forced
        /// logout. Use this method to terminate sessions that may pose a security risk or require administrative
        /// intervention.</remarks>
        /// <param name="sessionKey">The unique key identifying the user session to be terminated. Cannot be null, empty, or whitespace.</param>
        /// <returns>A redirect result to the login history view. If the session key is invalid or the session is not found, an
        /// error message is provided; otherwise, a success message is displayed.</returns>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceLogoutSession(string sessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                TempData["error"] = "Invalid session key.";
                return RedirectToAction(nameof(LoginHistory));
            }

            var success = await _loginHistoryService.ForceLogoutSessionAsync(
                sessionKey, HttpContext.RequestAborted);

            TempData[success ? "success" : "error"] = success
                ? "Session closed successfully."
                : "Session not found or already closed.";

            return RedirectToAction(nameof(LoginHistory));
        }

        /// <summary>
        /// Force-closes one of the current user's own sessions.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceLogoutOwnSession(string sessionKey)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction(nameof(SignIn));
            }

            var row = await _context.UserLoginHistories.GetAsync(
                h => h.SessionKey == sessionKey && h.UserId == userId && h.LogoutTimeUtc == null,
                asNoTracking: false,
                cancellationToken: HttpContext.RequestAborted);

            if (row is null)
            {
                TempData["error"] = "Session not found or already closed.";
                return RedirectToAction(nameof(MyLoginHistory));
            }

            var now = DateTimeHelper.GetCurrentSastDateTimeOffset();
            row.LogoutTimeUtc = now;
            row.LastActivityUtc = now;
            row.IsCurrentSession = false;
            row.ForceLogoutPerformed = true;
            row.LogoutReason = "ForceLogoutSelf";
            await _context.SaveAsync();


            TempData["success"] = "Session closed.";
            return RedirectToAction(nameof(MyLoginHistory));
        }

        /// <summary>
        /// Bulk force-logout of multiple sessions (Admin only).
        /// </summary>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkForceLogout(string[] sessionKeys)
        {
            if (sessionKeys is null || sessionKeys.Length == 0)
            {
                TempData["error"] = "No sessions selected.";
                return RedirectToAction(nameof(LoginHistory));
            }

            var closedCount = await _loginHistoryService.BulkForceLogoutAsync(
                sessionKeys, HttpContext.RequestAborted);

            TempData["success"] = $"{closedCount} session(s) force-closed successfully.";

            return RedirectToAction(nameof(LoginHistory));
        }

        /// <summary>
        /// Displays the "My Login History" self-service page for the current user.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyLoginHistory()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction(nameof(SignIn));
            }

            var model = await _loginHistoryService.GetMyLoginHistoryAsync(userId, HttpContext.RequestAborted);

            return View(model);
        }

        /// <summary>
        /// Displays the view that allows users to initiate the password reset process.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the password reset request view.</returns>
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Returns the view that displays the application settings page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the settings view to the client.</returns>
        public IActionResult Settings()
        {
            return View();
        }

        /// <summary>
        /// Returns the view for the Communication page.
        /// </summary>
        /// <returns>A view that renders the Communication page to the response.</returns>
        public IActionResult Communication()
        {
            return View();
        }

        #region Private

        /// <summary>
        /// Handles errors during the sign-in process by logging the error and setting the message.
        /// </summary>
        /// <param name="errorMessage">The error message to log and display.</param>
        /// <returns>An IActionResult to redirect back to the sign-in view.</returns>
        private IActionResult HandleSignInError(string errorMessage)
        {
            TempData["ErrorMessage"] = errorMessage;

            return RedirectToAction(nameof(SignIn));
        }

        /// <summary>
        /// Retrieves an active user based on student number and password.
        /// </summary>
        /// <param name="studentNumber">The student's number.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A Task that represents the asynchronous operation, with the user info as the result.</returns>
        private async Task<User?> GetActiveUserAsync(string studentNumber, string password)
        {
            var getUsers = await _context.Users.GetAllAsync();

            return getUsers.FirstOrDefault(u =>
                u.StudentNumber == studentNumber &&
                u.Password == Helper.ValueEncryption(password));
        }
        /// <summary>
        /// Signs in the user by creating claims and setting authentication cookies.
        /// </summary>
        /// <param name="userInfo">The user information.</param>
        /// <param name="rememberMe">Indicates whether to persist the login.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task SignInUserAsync(User userInfo, bool rememberMe)
        {
            var sessionKey = Guid.NewGuid().ToString("N");

            var loginRow = new UserLoginHistory
            {
                Id = Guid.NewGuid(),
                UserId = userInfo.Id,
                SessionKey = sessionKey,
                LoginTimeUtc = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                LastActivityUtc = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString(),
                IsCurrentSession = true,
                LogoutReason = null
            };

            await _context.UserLoginHistories.AddAsync(loginRow, HttpContext.RequestAborted);
            await _context.SaveAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Surname, userInfo.LastName),
                new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
                new Claim("TenantId", userInfo.TenantId.ToString("D")),
                new Claim("TenantSlug", _tenantContext.TenantSlug ?? string.Empty),
                new Claim("POEAppCookie", "Code"),

                new Claim(LoginSessionKeyClaim, sessionKey)
            };

            if (userInfo.Role == eSysRole.SuperAdmin && userInfo.TenantId == Guid.Parse("00000000-0000-0000-0000-000000000001"))
                claims.Add(new Claim("PlatformAdmin", "true"));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = rememberMe });

            await UpdateUserLastLoginAsync(userInfo);
        }

        /// <summary>
        /// Updates the user's last login date if they are active, 
        /// or deactivates the user if inactive for more than the specified threshold.
        /// </summary>
        /// <param name="userInfo">The user information to be updated.</param>
        /// <param name="inactiveThresholdDays">Number of days of inactivity allowed before deactivation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task UpdateUserLastLoginAsync(User userInfo, int inactiveThresholdDays = 10)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException(nameof(userInfo), "User information cannot be null.");
            }

            DateTime lastActivityDate = userInfo.LastActivityDate ?? DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            TimeSpan inactiveDuration = DateTimeHelper.GetCurrentSastDateTimeOffset() - lastActivityDate;

            try
            {
                if (userInfo.IsActive)
                {
                    if (inactiveDuration.TotalDays <= inactiveThresholdDays)
                    {
                        userInfo.LastLoginDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

                        userInfo.LastActivityDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
                    }
                    else if (userInfo.Role == eSysRole.Facilitator)
                    {
                        userInfo.IsActive = false;

                        _logger.LogInformation($"User with ID {userInfo.Id} has been deactivated due to inactivity.");
                    }
                }

                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user's last login status.");
                throw;
            }
        }

        /// <summary>
        /// Escapes a value for safe CSV output.
        /// </summary>
        private static string Csv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }

        /// <summary>
        /// Sets the user session information after a successful login.
        /// </summary>
        /// <param name="userInfo">The user information.</param>
        private void SetUserSession(User userInfo)
        {
            var currentUserName = userInfo.Name;

            var currentUserSurname = userInfo.LastName;

            Helper.loggedInUser = $"{currentUserName} {currentUserSurname}";

            HttpContext.Session.SetString("SessionUser", JsonConvert.SerializeObject(userInfo));
        }

        /// <summary>
        /// Redirects the user to the appropriate page based on their role and active status.
        /// </summary>
        /// <param name="userInfo">The user information containing role and status.</param>
        /// <param name="returnUrl">Optional return URL after login.</param>
        /// <returns>An IActionResult representing the redirect.</returns>
        private IActionResult RedirectUserBasedOnRole(User userInfo, string? returnUrl)
        {
            if (userInfo == null) throw new ArgumentNullException(nameof(userInfo));

            if (!userInfo.IsActive)
            {
                return RedirectToAction("BlockUserAccount", "Home");
            }

            if (!userInfo.Role.HasValue)
            {
                return RedirectToAction("AccessDenied", "Global");
            }

            var roleRedirectMap = new Dictionary<eSysRole, Func<IActionResult>>
            {
                { eSysRole.None, () => RedirectToAction("AccessDenied", "Global") },
                { eSysRole.Admin, () => RedirectToAction("AdminPanel", "Home") },
                { eSysRole.SuperAdmin, () => RedirectToAction("AdminPanel", "Home") },
                { eSysRole.Facilitator, () => RedirectToAction("EmployeeInfo", "Employee")},
                { eSysRole.Student, () => RedirectToAction("StudentDetail", "Student", new { StudentNumber = userInfo.StudentNumber }) }
            };

            if (roleRedirectMap.TryGetValue(userInfo.Role.Value, out var redirectAction))
            {
                return redirectAction();
            }

            return RedirectToAction(nameof(SignIn));
        }

        /// <summary>
        /// Clears the logged-in user session data.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        private void ClearUserSession(HttpContext httpContext)
        {
            Helper.loggedInUser = string.Empty;

            httpContext.Session.Clear();
        }

        /// <summary>
        /// Maps a <see cref="User"/> entity to a <see cref="UserDetailsViewModel"/>.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object to be mapped.</param>
        /// <returns>
        /// A <see cref="UserDetailsViewModel"/> containing the mapped user details, 
        /// or null if the input <paramref name="user"/> is invalid.
        /// </returns>
        private UserDetailsViewModel MapUserDetails(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("MapUserDetails was called with a null user.");

                return null;
            }

            try
            {
                var userDetailsViewModel = new UserDetailsViewModel
                {
                    Department = user.Department,
                    Role = user.Role,
                    Cellphone = user.Cellphone,
                    Email = user.Username,
                    LastName = user.LastName,
                    LastLoginDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                    Name = user.Name,
                    OldPassword = user.Password,
                    IDPass = user.IDPass,
                    LastActivityDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                    Id = user.Id,
                    IsActive = user.IsActive,
                    ProfileImage = user.ProfileImage,
                    StudentNumber = user.StudentNumber,
                    ConfirmPassword = user.ConfirmPassword,
                };

                _logger.LogInformation("Successfully mapped User to UserDetailsViewModel for user {Username}", user.Username);

                return userDetailsViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while mapping User to UserDetailsViewModel.");

                throw;
            }
        }
    }
}

#endregion
