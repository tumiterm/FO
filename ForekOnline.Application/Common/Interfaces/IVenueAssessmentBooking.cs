using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Repository interface for <see cref="VenueAssessmentBooking"/> entities.
    /// </summary>
    public interface IVenueAssessmentBooking : IRepository<VenueAssessmentBooking>
    {
        Task<VenueAssessmentBooking> UpdateVenueAssessmentBookingAsync(VenueAssessmentBooking venue);
    }
}