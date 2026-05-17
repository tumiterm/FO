// <copyright file="VisitRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 17:30 PM
// Purpose:         Defines the VisitRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{
    public class VisitRepository : Repository<Visit>, IVisit
    {
        
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public VisitRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Visit model in the repository.
        /// </summary>
        /// <param name="visit">The Visit model to be updated.</param>
        public async Task<Visit> UpdateVisitAsync(Visit visit)
        {
            _context.Visits.Update(visit);

            await _context.SaveChangesAsync();

            return visit;
        }
    }
}
