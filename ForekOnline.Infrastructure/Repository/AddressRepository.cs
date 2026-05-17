// <copyright file="AddressRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-02-2025 11:54 AM
// Purpose:         Defines the Address Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Address Repository.
    /// </summary>
    public class AddressRepository : Repository<Address>, IAddress
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Address model in the repository.
        /// </summary>
        /// <param name="address">The Address model to be updated.</param>
        public async Task<Address> UpdateAddressAsync(Address address)
        {
            _context.Address.Update(address);

            await _context.SaveChangesAsync();

            return address;
        }
    }
}
