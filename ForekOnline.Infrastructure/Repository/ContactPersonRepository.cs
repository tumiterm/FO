// <copyright file="ContactPersonRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:36 PM
// Purpose:         Defines the ContactPersonRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the ContactPerson Repository.
    /// </summary>
    public class ContactPersonRepository : Repository<ContactPerson>, IContactPerson
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ContactPersonRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Application model in the repository.
        /// </summary>
        /// <param name="applications">The Application model to be updated.</param>
        public async Task<ContactPerson> UpdateContactPersonAsync(ContactPerson contactPerson)
        {
            _context.ContactPerson.Update(contactPerson);

            await _context.SaveChangesAsync();

            return contactPerson;
        }
    }
}
