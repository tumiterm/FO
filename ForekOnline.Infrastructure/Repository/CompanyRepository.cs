// <copyright file="CompanyRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:31 PM
// Purpose:         Defines the CompanyRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Company Repository.
    /// </summary>
    public class CompanyRepository : Repository<Company>, ICompany
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Company model in the repository.
        /// </summary>
        /// <param name="company">The Company model to be updated.</param>
        public async Task<Company> UpdateCompanyAsync(Company company)
        {
            _context.Company.Update(company);

            await _context.SaveChangesAsync();

            return company;
        }

    }
}
