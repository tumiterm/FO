// <copyright file="VenueAssessmentBookingRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:31 PM
// Purpose:         Defines the VenueAssessmentBookingRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the VenueAssessmentBooking.
    /// </summary>
    public class VenueAssessmentBookingRepository : Repository<VenueAssessmentBooking>, IVenueAssessmentBooking
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueAssessmentBookingRepository"/> class.
        /// </summary>
        /// <param name="context">The VenueAssessmentBooking database context.</param>
        public VenueAssessmentBookingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing VenueReservationAudit model in the repository.
        /// </summary>
        /// <param name="venueAssessmentBooking">The VenueReservationAudit model to be updated.</param>
        public async Task<VenueAssessmentBooking> UpdateVenueAssessmentBookingAsync(VenueAssessmentBooking venueAssessmentBooking)
        {
            _context.VenueAssessmentBookings.Update(venueAssessmentBooking);

            await _context.SaveChangesAsync();

            return venueAssessmentBooking;
        }

    }
}
