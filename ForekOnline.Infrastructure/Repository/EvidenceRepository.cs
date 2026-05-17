// <copyright file="EvidenceRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:44 PM
// Purpose:         Defines the EvidenceRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Application Repository.
    /// </summary>
    public class EvidenceRepository : Repository<Evidence>, IEvidence
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvidenceRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public EvidenceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Evidence model in the repository.
        /// </summary>
        /// <param name="evidence">The Evidence model to be updated.</param>
        public async Task<Evidence> UpdateEvidenceAsync(Evidence evidence)
        {
            _context.Evidence.Update(evidence);

            await _context.SaveChangesAsync();

            return evidence;
        }
    }
}
