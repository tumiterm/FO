// <copyright file="ResourceRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    12-03-2025 11:54 AM
// Purpose:         Defines the ResourceRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Resource Repository.
    /// </summary>
    public class ResourceRepository : Repository<Resource>, IResource
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Resource model in the repository.
        /// </summary>
        /// <param name="resource">The Resource model to be updated.</param>
        public async Task<Resource> UpdateResourceAsync(Resource resource)
        {
            _context.Resource.Update(resource);

            await _context.SaveChangesAsync();

            return resource;
        }
    }
}
