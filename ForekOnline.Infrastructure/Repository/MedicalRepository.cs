// <copyright file="MedicalRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 17:12 PM
// Purpose:         Defines the MedicalRepository interface.


#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Medical Repository.
    /// </summary>
    public class MedicalRepository : Repository<Medical>, IMedical
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MedicalRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public MedicalRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Medical model in the repository.
        /// </summary>
        /// <param name="medical">The Medical model to be updated.</param>
        public async Task<Medical> UpdateMedicalRecordAsync(Medical medical)
        {
            _context.Medicals.Update(medical);

            await _context.SaveChangesAsync();

            return medical;
        }
    }
}
