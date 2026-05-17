// <copyright file="CategoryRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-02-2025 11:54 AM
// Purpose:         Defines the CategoryRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion



namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Category Repository.
    /// </summary>
    public class CategoryRepository : Repository<ResourceCategory>, ICategory
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing ResourceCategory model in the repository.
        /// </summary>
        /// <param name="resourceCategory">The ResourceCategory model to be updated.</param>
        public async Task<ResourceCategory> UpdateCategoryAsync(ResourceCategory resourceCategory)
        {
            _context.ResourceCategory.Update(resourceCategory);

            await _context.SaveChangesAsync();

            return resourceCategory;
        }
    }
}
