// <copyright file="IVenueBookingService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    15/03/2026
// Purpose:         Service interface for venue booking orchestration

#region Usings
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Orchestrates the two-stage venue booking workflow.
    /// </summary>
    public interface IVenueBookingService
    {

        // ── FR-01: Venue Master Data ──
        Task<IReadOnlyList<Venue>> GetAllVenuesAsync();
        Task<Venue?> GetVenueByIdAsync(Guid venueId);
        Task<ValidationResponse> CreateVenueAsync(Venue venue);
        Task<ValidationResponse> UpdateVenueAsync(Venue venue);
        Task<ValidationResponse> DeactivateVenueAsync(Guid venueId);

        // ── Stage 1: Reservation ──
        Task<IReadOnlyList<VenueAvailabilityViewModel>> GetAvailableVenuesAsync(string campus, int expectedStudents, DateTime date, DateTime startUtc, DateTime endUtc);
        Task<ValidationResponse> CreateReservationAsync(VenueReservationCreateRequest request, Guid facilitatorId, string facilitatorName);

        // ── HOD Approval ──
        Task<IReadOnlyList<VenueReservation>> GetPendingReservationsAsync(string? campus = null, string? department = null);
        Task<ValidationResponse> ApproveReservationAsync(Guid reservationId, Guid hodId, string hodName);
        Task<ValidationResponse> RejectReservationAsync(Guid reservationId, Guid hodId, string hodName, string reason);

        // ── Stage 2: Assessment Booking ──
        Task<IReadOnlyList<VenueReservation>> GetApprovedReservationsForFacilitatorAsync(Guid facilitatorId);
        Task<ValidationResponse> CreateAssessmentBookingAsync(VenueAssessmentBookingRequest request, string createdBy);

        // ── Background ──
        Task<int> ExpireStaleReservationsAsync();
    }
}