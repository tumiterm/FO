
#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Repository interface for <see cref="VenueReservationAudit"/> entities.
    /// </summary>
    public interface IVenueReservationAudit : IRepository<VenueReservationAudit>
    {
        /// <summary>
        /// Asynchronously updates an existing venue reservation audit record.
        /// </summary>
        /// <param name="venueReservationAudit">The venue reservation audit entity to update. Must not be null and should contain the updated audit
        /// information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated venue reservation
        /// audit entity.</returns>
        Task<VenueReservationAudit> UpdateVenueReservationAuditAsync(VenueReservationAudit venueReservationAudit);
    }
}