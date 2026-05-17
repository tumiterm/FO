#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    public class VenueReservationRepository : Repository<VenueReservation>, IVenueReservation
    {
        private readonly ApplicationDbContext _context;

        public VenueReservationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<VenueReservation> UpdateReservationAsync(VenueReservation reservation)
        {
            _context.VenueReservations.Update(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }
    }
}