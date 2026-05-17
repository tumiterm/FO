// <copyright file="PlacementRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 17:49 PM
// Purpose:         Defines the PlacementRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{

    /// <summary>
    /// Represents a repository specifically for performing operations on the Placement Repository.
    /// </summary>
    public class PlacementRepository : Repository<Placement>, IPlacement
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlacementRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public PlacementRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Placement model in the repository.
        /// </summary>
        /// <param name="placement">The Placement model to be updated.</param>
        public async Task<Placement> UpdatePlacementAsync(Placement placement)
        {
            _context.Placements.Update(placement);

            await _context.SaveChangesAsync();

            return placement;
        }


    }
}
