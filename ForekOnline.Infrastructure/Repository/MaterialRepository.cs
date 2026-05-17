// <copyright file="MaterialRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 17:06 PM
// Purpose:         Defines the MaterialRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{

    /// <summary>
    /// Represents a repository specifically for performing operations on the Material Repository.
    /// </summary>
    public class MaterialRepository : Repository<Material>, IMaterial
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public MaterialRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Material model in the repository.
        /// </summary>
        /// <param name="applications">The Material model to be updated.</param>
        public async Task<Material> UpdateMaterialAsync(Material material)
        {
            _context.Material.Update(material);

            await _context.SaveChangesAsync();

            return material;
        }
    }
}
