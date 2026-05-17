// <copyright file="LicenseRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:59 PM
// Purpose:         Defines the LicenseRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the License Repository.
    /// </summary>
    public class LicenseRepository : Repository<License>, ILicense
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public LicenseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing License model in the repository.
        /// </summary>
        /// <param name="license">The License model to be updated.</param>
        public async Task<License> UpdateLicenseAsync(License license)
        {
            _context.Licenses.Update(license);

            await _context.SaveChangesAsync();

            return license;
        }
    }
}
