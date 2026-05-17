// <copyright file="VenueBookingController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    15/03/2026
// Purpose:         Handles venue reservation and assessment booking actions

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Controller for the two-stage venue booking workflow.
    /// </summary>
    [Authorize]
    public class VenueBookingController : Controller
    {
        #region Fields
        private readonly IVenueBookingService _bookingService;
        private readonly IUnitOfWork _context;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IHelperService _helperService;
        private readonly ILogger<VenueBookingController> _logger;
        private readonly IInAppNotificationService _inAppNotificationService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the VenueBookingController class with the specified services and logger.
        /// </summary>
        /// <param name="bookingService">The service used to manage venue bookings. Cannot be null.</param>
        /// <param name="context">The unit of work context for managing data persistence. Cannot be null.</param>
        /// <param name="userService">The service used to manage user-related operations. Cannot be null.</param>
        /// <param name="studentService">The service used to manage student-related operations. Cannot be null.</param>
        /// <param name="helperService">The service providing helper utilities for venue booking operations. Cannot be null.</param>
        /// <param name="logger">The logger used to record diagnostic and operational information for the controller.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the bookingService, context, userService, studentService, or helperService parameters is
        /// null.</exception>
        public VenueBookingController(IVenueBookingService bookingService, IUnitOfWork context, IUserService userService, IStudentService studentService,IHelperService helperService, ILogger<VenueBookingController> logger, IInAppNotificationService inAppNotificationService)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _logger = logger;
            _inAppNotificationService = inAppNotificationService;
        }

        /// <summary>
        /// Displays the Reserve Venue screen.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public IActionResult ReserveVenue()
        {
            return View();
        }

        /// <summary>
        /// Returns filtered available venues as JSON (FR-02 step 4).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> AvailableVenues(string campus, int expectedStudents, DateTime date, DateTime startUtc, DateTime endUtc)
        {
            var venues = await _bookingService.GetAvailableVenuesAsync(campus, expectedStudents, date, startUtc, endUtc);
            return Json(venues);
        }

        /// <summary>
        /// Creates a venue reservation.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> ReserveVenue([FromBody] VenueReservationCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userService.OnGetCurrentUser();
            var result = await _bookingService.CreateReservationAsync(request, user.Id, $"{user.Name} {user.LastName}");

            if (result.IsError)
            {
                TempData["error"] = result.Message;
                return BadRequest(new { message = result.Message });
            }

            await _inAppNotificationService.SendToRolesAsync([eSysRole.Admin, eSysRole.Facilitator], $"A new venue reservation has been created by {user.Name} {user.LastName}", null, "fa fa-bell", $"{user.Name} {user.LastName}");

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Displays the HOD pending reservations dashboard.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PendingReservations(string? campus, string? department)
        {
            var user = _userService.OnGetCurrentUser();

            if (!user.IsHeadOfDepartment && !User.IsInRole("SuperAdmin"))
            {
                ViewBag.UserName = $"{user.Name} {user.LastName}";
                ViewBag.UserRole = user.Role?.ToString() ?? "Unknown";
                return View("ReservationAccessDenied");
            }

            var reservations = await _bookingService.GetPendingReservationsAsync(campus, department);
            return View(reservations);
        }

        /// <summary>
        /// Approves a venue reservation.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ApproveReservation(Guid reservationId)
        {
            var user = _userService.OnGetCurrentUser();

            if (!user.IsHeadOfDepartment && !User.IsInRole("SuperAdmin"))
            {
                ViewBag.UserName = $"{user.Name} {user.LastName}";
                ViewBag.UserRole = user.Role?.ToString() ?? "Unknown";
                return View("ReservationAccessDenied");
            }

            var result = await _bookingService.ApproveReservationAsync(reservationId, user.Id, $"{user.Name} {user.LastName}");

            if (result.IsError)
            {
                TempData["error"] = result.Message;
                return BadRequest(new { message = result.Message });
            }

            TempData["success"] = "Reservation successfully approved.";

            await _inAppNotificationService.SendToRolesAsync([eSysRole.Admin, eSysRole.Facilitator], $"A new venue reservation has been approved by {user.Name} {user.LastName}", null, "fa fa-bell", $"{user.Name} {user.LastName}");

            return RedirectToAction(nameof(PendingReservations));
        }

        /// <summary>
        /// Rejects a venue reservation (mandatory reason).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RejectReservation(Guid reservationId, string reason)
        {
            var user = _userService.OnGetCurrentUser();

            if (!user.IsHeadOfDepartment && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
                return Forbid();

            var result = await _bookingService.RejectReservationAsync(reservationId, user.Id, $"{user.Name} {user.LastName}", reason);

            TempData[result.IsError ? "error" : "success"] = result.Message;
            return RedirectToAction(nameof(PendingReservations));
        }

        /// <summary>
        /// Displays the Create Assessment Booking screen.
        /// Only approved reservations for the current facilitator are shown.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> BookAssessment()
        {
            var user = _userService.OnGetCurrentUser();
            var approved = await _bookingService.GetApprovedReservationsForFacilitatorAsync(user.Id);

            ViewBag.ApprovedReservations = approved.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = $"{r.Venue.Name} – {r.ReservedDate:dd MMM yyyy} ({r.StartTimeUtc:HH:mm}–{r.EndTimeUtc:HH:mm})"
            });

            var courses = await _context.Courses.GetAllAsync();
            ViewBag.Courses = courses.Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = c.CourseName });

            var modules = await _context.Modules.GetAllAsync();
            ViewBag.Modules = modules.Select(m => new SelectListItem { Value = m.ModuleId.ToString(), Text = m.ModuleName });

            return View();
        }

        /// <summary>
        /// Creates the assessment booking and sends student emails.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Facilitator,Admin,SuperAdmin")]
        public async Task<IActionResult> BookAssessment([FromBody] VenueAssessmentBookingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userService.OnGetCurrentUser();
            var result = await _bookingService.CreateAssessmentBookingAsync(request, $"{user.Name} {user.LastName}");

            if (result.IsError)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Displays the venue management dashboard (list all venues).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ManageVenues()
        {
            var venues = await _bookingService.GetAllVenuesAsync();
            return View(venues);
        }

        /// <summary>
        /// Returns all venues as JSON for AJAX table refresh.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> VenueListJson()
        {
            var venues = await _bookingService.GetAllVenuesAsync();
            return Json(venues.Select(v => new
            {
                v.Id,
                v.Name,
                v.Campus,
                Department = v.Departments.ToString(),
                v.MaxCapacity,
                VenueType = v.VenueType.ToString(),
                Status = v.Status.ToString(),
                v.EquipmentChecklist,
                CreatedOn = v.DateCreated.ToString("dd MMM yyyy")
            }));
        }

        /// <summary>
        /// Displays the Create Venue form.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult CreateVenue()
        {
            return View(new ForekOnline.Domain.Entities.Venue());
        }

        /// <summary>
        /// Creates a new venue.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateVenue(ForekOnline.Domain.Entities.Venue venue)
        {
            var result = await _bookingService.CreateVenueAsync(venue);

            if (result.IsError)
            {
                TempData["error"] = result.Message;
                return View(venue);
            }

            TempData["success"] = "Venue successfully created.";
            return RedirectToAction(nameof(ManageVenues));
        }

        /// <summary>
        /// Displays the Edit Venue form.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> EditVenue(Guid id)
        {
            var venue = await _bookingService.GetVenueByIdAsync(id);
            if (venue is null) return NotFound();
            return View(venue);
        }

        /// <summary>
        /// Updates an existing venue.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> EditVenue(ForekOnline.Domain.Entities.Venue venue)
        {
            var user = _userService.OnGetCurrentUser();
            var result = await _bookingService.UpdateVenueAsync(venue);

            if (result.IsError)
            {
                TempData["error"] = result.Message;
                return View(venue);
            }
            
            TempData["success"] = "Venue successfully updated.";
            return RedirectToAction(nameof(ManageVenues));
        }

        /// <summary>
        /// Deactivates a venue (soft delete).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeactivateVenue(Guid id)
        {
            var user = _userService.OnGetCurrentUser();
            var result = await _bookingService.DeactivateVenueAsync(id);

            if(result.IsError)
            {
                TempData["error"] = result.Message;
                return RedirectToAction(nameof(ManageVenues));
            }

            TempData["success"] = "Venue successfully deactivated.";
            return RedirectToAction(nameof(ManageVenues));
        }
    }
}