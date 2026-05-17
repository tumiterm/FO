// <copyright file="VenueRepository.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    15/03/2026
// Purpose:         Defines the Venue Repository

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides data access methods for managing <see cref="Venue"/> entities.
    /// </summary>
    public class VenueRepository : Repository<Venue>, IVenue
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the VenueRepository class using the specified database context.
        /// </summary>
        /// <param name="context">The ApplicationDbContext instance to be used for data access operations. Cannot be null.</param>
        public VenueRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates the specified venue in the data store asynchronously.
        /// </summary>
        /// <remarks>The method saves all changes made to the context. If the venue does not exist in the
        /// data store, an exception may be thrown when saving changes. The returned entity reflects the state after the
        /// update is committed.</remarks>
        /// <param name="venue">The venue entity to update. The entity must have a valid identifier corresponding to an existing venue.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated venue entity.</returns>
        public async Task<Venue> UpdateVenueAsync(Venue venue)
        {
            _context.Venues.Update(venue);
            await _context.SaveChangesAsync();
            return venue;
        }
    }
}