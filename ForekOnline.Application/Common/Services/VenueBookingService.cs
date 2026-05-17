// <copyright file="VenueBookingService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    15/03/2026
// Purpose:         Orchestrates the two-stage venue booking workflow

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Implements the two-stage venue booking workflow with mandatory HOD approval.
    /// </summary>
    public class VenueBookingService : IVenueBookingService
    {
        #region Fields
        private readonly IUnitOfWork _context;
        private readonly IHelperService _helperService;
        private static readonly TimeSpan SoftHoldDuration = TimeSpan.FromHours(24);
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the VenueBookingService class with the specified unit of work and helper
        /// service.
        /// </summary>
        /// <param name="context">The unit of work instance used to manage data persistence and transactions.</param>
        /// <param name="helperService">The helper service that provides utility functions required by the booking service.</param>
        /// <exception cref="ArgumentNullException">Thrown if context or helperService is null.</exception>
        public VenueBookingService(IUnitOfWork context, IHelperService helperService, IUserService userService, IStudentService studentService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _studentService = studentService;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<VenueAvailabilityViewModel>> GetAvailableVenuesAsync(
            string campus, int expectedStudents, DateTime date, DateTime startUtc, DateTime endUtc)
        {
            var venues = await _context.Venues.GetAllAsync(v => v.Status == eVenueStatus.Active);

            var campusVenues = venues.Where(v =>
                v.Campus.Equals(campus, StringComparison.OrdinalIgnoreCase)).ToList();

            var overlapping = await _context.VenueReservations.GetAllAsync(r =>
                r.Status != eReservationStatus.Expired &&
                r.Status != eReservationStatus.Cancelled &&
                r.Status != eReservationStatus.Rejected &&
                r.StartTimeUtc < endUtc &&
                r.EndTimeUtc > startUtc);

            var bookedVenueIds = overlapping.Select(r => r.VenueId).ToHashSet();

            return campusVenues.Select(v =>
            {
                bool isBooked = bookedVenueIds.Contains(v.Id);
                bool capacityOk = v.MaxCapacity >= expectedStudents;
                bool isAvailable = !isBooked && capacityOk && v.Status == eVenueStatus.Active;

                string? reason = null;
                if (isBooked) reason = "Already reserved/booked for this time slot.";
                else if (!capacityOk) reason = $"Capacity ({v.MaxCapacity}) is less than required ({expectedStudents}).";
                else if (v.Status == eVenueStatus.UnderMaintenance) reason = "Venue is under maintenance.";

                return new VenueAvailabilityViewModel
                {
                    VenueId = v.Id,
                    VenueName = v.Name ?? string.Empty,
                    Campus = v.Campus,
                    MaxCapacity = v.MaxCapacity,
                    VenueType = v.VenueType,
                    EquipmentChecklist = v.EquipmentChecklist,
                    PhotoUrl = v.PhotoUrl,
                    IsAvailable = isAvailable,
                    UnavailableReason = reason,
                    IsSuggested = isAvailable && v.MaxCapacity >= expectedStudents && v.MaxCapacity <= expectedStudents * 1.5
                };
            })
            .OrderByDescending(v => v.IsSuggested)
            .ThenByDescending(v => v.IsAvailable)
            .ThenBy(v => v.VenueName)
            .ToList();
        }

        /// <inheritdoc/>
        public async Task<ValidationResponse> CreateReservationAsync(VenueReservationCreateRequest request, Guid facilitatorId, string facilitatorName)
        {
            var overlap = await _context.VenueReservations.ExistsAsync(r =>
                r.VenueId == request.VenueId &&
                r.Status != eReservationStatus.Expired &&
                r.Status != eReservationStatus.Cancelled &&
                r.Status != eReservationStatus.Rejected &&
                r.StartTimeUtc < request.EndTimeUtc &&
                r.EndTimeUtc > request.StartTimeUtc);

            if (overlap)
            {
                return new ValidationResponse ("This venue slot is already reserved or booked. Please choose another slot.");
            }

            var reservation = new VenueReservation
            {
                Id = Guid.NewGuid(),
                VenueId = request.VenueId,
                FacilitatorId = facilitatorId,
                FacilitatorName = facilitatorName,
                Campus = request.Campus,
                ExpectedStudents = request.ExpectedStudents,
                ReservedDate = request.ReservedDate,
                StartTimeUtc = request.StartTimeUtc,
                EndTimeUtc = request.EndTimeUtc,
                Status = eReservationStatus.PendingHodApproval,
                ExpiresOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().Add(SoftHoldDuration).DateTime,
                DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                Name = $"{facilitatorName} - {request.ReservedDate:dd MMM yyyy} {request.StartTimeUtc:HH:mm} to {request.EndTimeUtc:HH:mm}",
                Code = $"RES-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                UserCreated = facilitatorName,
                UserModified = facilitatorName,
                DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                IsDeleted = false,
                //RowVersion = new byte[8],
            };

            await _context.VenueReservations.AddAsync(reservation);

            await _context.VenueReservationAudits.AddAsync(new VenueReservationAudit
            {
                Id = Guid.NewGuid(),
                ReservationId = reservation.Id,
                Action = eReservationAction.Created,
                UserCreated = facilitatorName,
                Remarks = $"Reserved venue for {request.ExpectedStudents} students.",
                DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                Code = $"RES-{reservation.Id.ToString().Substring(0, 8).ToUpper()}",
                Name = $"{facilitatorName} - created a reservation",
                RowVersion = new byte[8],
                UserModified = facilitatorName,
                IsDeleted = false,
            });

            await _context.SaveAsync();

            var venue = await _context.Venues.GetAsync(v => v.Id == request.VenueId);
            var hods = await _context.Users.GetAllAsync(u => u.IsHeadOfDepartment && !string.IsNullOrEmpty(u.Username));
            var timeSlot = $"{request.StartTimeUtc:HH:mm} – {request.EndTimeUtc:HH:mm}";
            var userDepartment = await DetermineUserDepartment(facilitatorId);

            foreach (var hod in hods.Where(d => d.Department == userDepartment))
            {
                try
                {
                    var emailBody = _helperService.OnSendVenueReservationHodNotification(
                        hodName: $"{hod.Name} {hod.LastName}",
                        facilitatorName: facilitatorName,
                        venueName: venue?.Name ?? "Unknown Venue",
                        campus: request.Campus,
                        expectedStudents: request.ExpectedStudents,
                        date: request.ReservedDate.ToString("dd MMM yyyy"),
                        timeSlot: timeSlot,
                        approveUrl: "https://forekonline.co.za/VenueBooking/PendingReservations");

                    await _helperService.SendMailNotificationAsync(new EmailDataViewModel
                    {
                        Recipient = hod.Username!,
                        Subject = $"Venue Reservation Pending Approval – {venue?.Name ?? "Venue"} ({request.ReservedDate:dd MMM yyyy})",
                        Body = emailBody,
                        From = "Forek Online",
                        Header = "Venue Reservation"
                    });
                }
                catch
                {
                }
            }

            return new ValidationResponse();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<VenueReservation>> GetPendingReservationsAsync(string? campus = null, string? department = null)
        {
            var pending = await _context.VenueReservations.GetAllAsync(
                r => r.Status == eReservationStatus.PendingHodApproval && r.ExpiresOnUtc > DateTime.UtcNow,
                includeProperties: new[] { nameof(VenueReservation.Venue) });

            IEnumerable<VenueReservation> filtered = pending;

            if (!string.IsNullOrWhiteSpace(campus))
                filtered = filtered.Where(r => r.Campus.Equals(campus, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(department))
                filtered = filtered.Where(r => r.Venue.Departments.ToString().Contains(department, StringComparison.OrdinalIgnoreCase));

            return filtered.OrderBy(r => r.StartTimeUtc).ToList();
        }

        /// <inheritdoc/>
        public async Task<ValidationResponse> ApproveReservationAsync(Guid reservationId, Guid hodId, string hodName)
        {
            var reservation = await _context.VenueReservations.GetAsync(r => r.Id == reservationId);

            if (reservation is null)
                return new ValidationResponse ("Reservation not found.");

            if (reservation.Status != eReservationStatus.PendingHodApproval)
                return new ValidationResponse ($"Reservation cannot be approved. Current status: {reservation.Status}.");

            if (reservation.ExpiresOnUtc <= DateTime.UtcNow)
            {
                reservation.Status = eReservationStatus.Expired;
                await _context.VenueReservations.UpdateReservationAsync(reservation);
                return new ValidationResponse ("Reservation has expired.");
            }

            reservation.Status = eReservationStatus.Approved;
            reservation.ActionedByHodId = hodId;
            reservation.ActionedByHodName = hodName;
            reservation.ActionedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            reservation.Name = $"{reservation.FacilitatorName} - {reservation.ReservedDate:dd MMM yyyy} {reservation.StartTimeUtc:HH:mm} to {reservation.EndTimeUtc:HH:mm} (Approved)";
            reservation.Code = $"RES-{reservation.Id.ToString().Substring(0, 8).ToUpper()}";

            await _context.VenueReservations.UpdateReservationAsync(reservation);

            await _context.VenueReservationAudits.AddAsync(new VenueReservationAudit
            {
                Id = Guid.NewGuid(),
                ReservationId = reservationId,
                Action = eReservationAction.Approved,
                UserCreated = hodName,
                DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                Name = reservation.Name ?? "N/A",
                Code = reservation.Code ?? $"RES-{reservation.Id.ToString().Substring(0, 8).ToUpper()}",
                IsDeleted = false,
                DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                UserModified = hodName,
                Remarks = "HOD approved the reservation.",
                RowVersion = new byte[8],
            });

            await _context.SaveAsync();

            try
            {
                var facilitator = await _context.Users.GetAsync(u => u.Id == reservation.FacilitatorId);
                var venue = await _context.Venues.GetAsync(v => v.Id == reservation.VenueId);
                var timeSlot = $"{reservation.StartTimeUtc:HH:mm} – {reservation.EndTimeUtc:HH:mm}";

                if (facilitator?.Username is not null)
                {
                    var emailBody = _helperService.OnSendVenueApprovalNotification(
                        facilitatorName: reservation.FacilitatorName,
                        venueName: venue?.Name ?? "Unknown Venue",
                        campus: reservation.Campus,
                        date: reservation.ReservedDate.ToString("dd MMM yyyy"),
                        timeSlot: timeSlot,
                        hodName: hodName,
                        bookAssessmentUrl: "https://forekonline.co.za/VenueBooking/BookAssessment");

                    await _helperService.SendMailNotificationAsync(new EmailDataViewModel
                    {
                        Recipient = facilitator.Username,
                        Subject = $"✅ Venue Reservation Approved – {venue?.Name ?? "Venue"} ({reservation.ReservedDate:dd MMM yyyy})",
                        Body = emailBody,
                        From = "Forek Online",
                        Header = "Venue Approved"
                    });
                }
            }
            catch {  }

            return new ValidationResponse();
        }

        /// <inheritdoc/>
        public async Task<ValidationResponse> RejectReservationAsync(Guid reservationId, Guid hodId, string hodName, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                return new ValidationResponse ("Rejection reason is mandatory.");

            var reservation = await _context.VenueReservations.GetAsync(r => r.Id == reservationId);

            if (reservation is null)
                return new ValidationResponse ("Reservation not found.");

            if (reservation.Status != eReservationStatus.PendingHodApproval)
                return new ValidationResponse ($"Reservation cannot be rejected. Current status: {reservation.Status}.");

            reservation.Status = eReservationStatus.Rejected;
            reservation.RejectionReason = reason;
            reservation.ActionedByHodId = hodId;
            reservation.ActionedByHodName = hodName;
            reservation.ActionedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            reservation.Name = $"{reservation.FacilitatorName} - {reservation.ReservedDate:dd MMM yyyy} {reservation.StartTimeUtc:HH:mm} to {reservation.EndTimeUtc:HH:mm} (Rejected)";
            reservation.Code = $"RES-{reservation.Id.ToString().Substring(0, 8).ToUpper()}";
            reservation.DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset();
            reservation.DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset();
            reservation.UserModified = hodName;
            reservation.RowVersion = new byte[8];

            await _context.VenueReservations.UpdateReservationAsync(reservation);

            await _context.VenueReservationAudits.AddAsync(new VenueReservationAudit
            {
                Id = Guid.NewGuid(),
                ReservationId = reservationId,
                Action = eReservationAction.Rejected,
                UserCreated = hodName,
                Remarks = reason,
                DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                Name = reservation.Name ?? "N/A",
                UserModified = hodName,
                DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                IsDeleted = false,
                Code = reservation.Code ?? "N/A",
                RowVersion = new byte[8],
            });

            await _context.SaveAsync();

            try
            {
                var facilitator = await _context.Users.GetAsync(u => u.Id == reservation.FacilitatorId);
                var venue = await _context.Venues.GetAsync(v => v.Id == reservation.VenueId);
                var timeSlot = $"{reservation.StartTimeUtc:HH:mm} – {reservation.EndTimeUtc:HH:mm}";

                if (facilitator?.Username is not null)
                {
                    var emailBody = _helperService.OnSendVenueRejectionNotification(
                        facilitatorName: reservation.FacilitatorName,
                        venueName: venue?.Name ?? "Unknown Venue",
                        campus: reservation.Campus,
                        date: reservation.ReservedDate.ToString("dd MMM yyyy"),
                        timeSlot: timeSlot,
                        hodName: hodName,
                        reason: reason);

                    await _helperService.SendMailNotificationAsync(new EmailDataViewModel
                    {
                        Recipient = facilitator.Username,
                        Subject = $"❌ Venue Reservation Rejected – {venue?.Name ?? "Venue"} ({reservation.ReservedDate:dd MMM yyyy})",
                        Body = emailBody,
                        From = "Forek Online",
                        Header = "Venue Rejected"
                    });
                }
            }
            catch {}

            return new ValidationResponse();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<VenueReservation>> GetApprovedReservationsForFacilitatorAsync(Guid facilitatorId)
        {
            return await _context.VenueReservations.GetAllAsync(
                r => r.FacilitatorId == facilitatorId && r.Status == eReservationStatus.Approved,
                includeProperties: new[] { nameof(VenueReservation.Venue) });
        }

        /// <inheritdoc/>
        public async Task<ValidationResponse> CreateAssessmentBookingAsync(VenueAssessmentBookingRequest request, string createdBy)
        {
            var reservation = await _context.VenueReservations.GetAsync(
                r => r.Id == request.ReservationId,
                includeProperties: new[] { nameof(VenueReservation.Venue) });

            if (reservation is null)
                return new ValidationResponse ("Reservation not found.");

            if (reservation.Status != eReservationStatus.Approved)
                return new ValidationResponse ("Assessment can only be created on an HOD-approved reservation.");

            var existingBooking = await _context.VenueAssessmentBookings.ExistsAsync(b => b.ReservationId == request.ReservationId);
            if (existingBooking)
                return new ValidationResponse ("An assessment has already been booked on this reservation.");

            var booking = new VenueAssessmentBooking
            {
                Id = Guid.NewGuid(),
                ReservationId = request.ReservationId,
                CourseId = request.CourseId,
                ModuleId = request.ModuleId,
                AssessmentName = request.AssessmentName,
                AssessmentType = request.AssessmentType,
                Instructions = request.Instructions,
                DurationMinutes = request.DurationMinutes,
                StudentList = string.Join("|", request.StudentIdentifiers),
                UserCreated = createdBy,
                DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                EmailsSent = false,
                Name = $"{request.AssessmentName} for {reservation.Name}",
                Code = $"ASMT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                IsDeleted = false,
                DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                RowVersion = new byte[8],
                UserModified = createdBy,
            };

            await _context.VenueAssessmentBookings.AddAsync(booking);

            reservation.Status = eReservationStatus.Booked;
            await _context.VenueReservations.UpdateReservationAsync(reservation);

            await _context.VenueReservationAudits.AddAsync(new VenueReservationAudit
            {
                Id = Guid.NewGuid(),
                ReservationId = reservation.Id,
                Action = eReservationAction.AssessmentBooked,
                UserCreated = createdBy,
                Remarks = $"Assessment '{request.AssessmentName}' booked with {request.StudentIdentifiers.Count} students.",
                DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset(),
                Name = reservation.Name ?? "N/A",
                Code = reservation.Code ?? "N/A",
                IsDeleted = false,
                RowVersion = new byte[8],
                UserModified = createdBy,
                DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset(),
            });

            await _context.SaveAsync();

            var timeSlot = $"{reservation.StartTimeUtc:HH:mm} – {reservation.EndTimeUtc:HH:mm}";
            var courseName = (await _context.Courses.GetAsync(c => c.CourseId == request.CourseId))?.CourseName ?? "N/A";
            var moduleName = (await _context.Modules.GetAsync(m => m.ModuleId == request.ModuleId))?.ModuleName ?? "N/A";

            var icsAttachment = _helperService.BuildAssessmentBookingIcsAttachment(
                request.AssessmentName,
                reservation.Venue?.Name ?? "Venue",
                reservation.Campus,
                reservation.StartTimeUtc,
                reservation.EndTimeUtc);

            foreach (var studentId in request.StudentIdentifiers)
            {
                try
                {
                    var student = await _studentService.GetStudentByEmailAsync(studentId);
                    if (string.IsNullOrWhiteSpace(student?.Email)) continue;

                    var studentName = $"{student.FirstName} {student.LastName}".Trim();
                    var emailBody = _helperService.OnSendAssessmentBookingStudentNotification(
                        studentName: string.IsNullOrWhiteSpace(studentName) ? "Student" : studentName,
                        assessmentName: request.AssessmentName,
                        courseName: courseName,
                        moduleName: moduleName,
                        venueName: reservation.Venue?.Name ?? "Venue",
                        campus: reservation.Campus,
                        date: reservation.ReservedDate.ToString("dd MMM yyyy"),
                        timeSlot: timeSlot,
                        instructions: request.Instructions ?? "");

                    await _helperService.SendMailNotificationAsync(new EmailDataViewModel
                    {
                        Recipient = student.Email,
                        Subject = $"📝 Assessment Scheduled – {request.AssessmentName} at {reservation.Venue?.Name ?? "Venue"}",
                        Body = emailBody,
                        From = "Forek Online",
                        Header = "Assessment Booking",
                        Attachments = new List<EmailAttachmentViewModel> { icsAttachment }
                    });
                }
                catch {  }
            }

            booking.EmailsSent = true;
            await _context.SaveAsync();

            return new ValidationResponse();
        }

        /// <inheritdoc/>
        public async Task<int> ExpireStaleReservationsAsync()
        {
            var stale = await _context.VenueReservations.GetAllAsync(r =>
                r.Status == eReservationStatus.PendingHodApproval &&
                r.ExpiresOnUtc <= DateTime.UtcNow);

            int count = 0;
            foreach (var reservation in stale)
            {
                reservation.Status = eReservationStatus.Expired;
                await _context.VenueReservations.UpdateReservationAsync(reservation);

                await _context.VenueReservationAudits.AddAsync(new VenueReservationAudit
                {
                    Id = Guid.NewGuid(),
                    ReservationId = reservation.Id,
                    Action = eReservationAction.Expired,
                    UserCreated = "System",
                    Remarks = "72-hour soft hold expired without HOD action.",
                    DateCreated = DateTime.UtcNow
                });

                count++;
            }

            if (count > 0) await _context.SaveAsync();

            return count;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Venue>> GetAllVenuesAsync()
        {
            return await _context.Venues.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<Venue?> GetVenueByIdAsync(Guid venueId)
        {
            return await _context.Venues.GetAsync(v => v.Id == venueId);
        }

        /// <inheritdoc/>
        public async Task<ValidationResponse> CreateVenueAsync(Venue venue)
        {
            if (string.IsNullOrWhiteSpace(venue.Name))
                return new ValidationResponse ("Venue name is required.");

            if (venue.MaxCapacity <= 0)
                return new ValidationResponse ("Capacity must be greater than 0.");

            var duplicate = await _context.Venues.ExistsAsync(v =>
                v.Name == venue.Name &&
                v.Campus == venue.Campus &&
                !v.IsDeleted);

            if (duplicate)
                return new ValidationResponse ($"A venue named '{venue.Name}' already exists on the '{venue.Campus}' campus.");

            var user = $"{_userService.OnGetCurrentUser()?.Name} {_userService.OnGetCurrentUser()?.LastName}";

            venue.Id = Guid.NewGuid();
            venue.DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset();
            venue.DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset();
            venue.IsDeleted = false;
            venue.UserCreated = user;
            venue.UserModified = user;
            venue.Code = "VEN-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            venue.Name = venue.Name.Trim();
            venue.RowVersion = new byte[8];

            await _context.Venues.AddAsync(venue);
            await _context.SaveAsync();

            return new ValidationResponse();
        }

        /// <inheritdoc/>
        public async Task<ValidationResponse> UpdateVenueAsync(Venue venue)
        {
            var existing = await _context.Venues.GetAsync(v => v.Id == venue.Id);
            if (existing is null)
                return new ValidationResponse("Venue not found.");

            var duplicate = await _context.Venues.ExistsAsync(v =>
                v.Name == venue.Name &&
                v.Campus == venue.Campus &&
                v.Id != venue.Id &&
                !v.IsDeleted);

            if (duplicate)
                return new ValidationResponse ($"A venue named '{venue.Name}' already exists on the '{venue.Campus}' campus.");

            var user = $"{_userService.OnGetCurrentUser()?.Name} {_userService.OnGetCurrentUser()?.LastName}";

            existing.Name = venue.Name;
            existing.Campus = venue.Campus;
            existing.Departments = venue.Departments;
            existing.MaxCapacity = venue.MaxCapacity;
            existing.VenueType = venue.VenueType;
            existing.EquipmentChecklist = venue.EquipmentChecklist;
            existing.DefaultBookingRules = venue.DefaultBookingRules;
            existing.PhotoUrl = venue.PhotoUrl;
            existing.FloorPlanUrl = venue.FloorPlanUrl;
            existing.Status = venue.Status;
            existing.UserModified = user;
            existing.DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset();

            await _context.Venues.UpdateVenueAsync(existing);
            //await _context.Save();

            var pending = await _context.VenueReservations.GetAllAsync(r =>
                r.VenueId == existing.Id &&
                (r.Status == eReservationStatus.PendingHodApproval || r.Status == eReservationStatus.Approved));

            if (pending.Any())
            {
                // TODO: Send notification to facilitators with pending/approved reservations about venue change
            }

            return new ValidationResponse();
        }

        /// <inheritdoc/>
        public async Task<ValidationResponse> DeactivateVenueAsync(Guid venueId)
        {
            var existing = await _context.Venues.GetAsync(v => v.Id == venueId);
            if (existing is null)
                return new ValidationResponse("Venue not found.");

            var user = $"{_userService.OnGetCurrentUser()?.Name} {_userService.OnGetCurrentUser()?.LastName}";

            existing.Status = eVenueStatus.Inactive;
            existing.UserModified = user;
            existing.DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset();

            await _context.Venues.UpdateVenueAsync(existing);
            await _context.SaveAsync();

            return new ValidationResponse();
        }

        #region Private Helpers
        private async Task<eDepartment> DetermineUserDepartment(Guid userId)
        {
            if (_userService.OnGetCurrentUser() is null)
                return eDepartment.None;

            var userInfo = await _userService.GetUserInfoAsync(userId);
            return userInfo?.Department ?? eDepartment.None;
        }
        #endregion
    }
}