// <copyright file="ModuleRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 17:24 PM
// Purpose:         Defines the ModuleRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Module Repository.
    /// </summary>
    public class ModuleRepository : Repository<Module>, IModule
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ModuleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Module model in the repository.
        /// </summary>
        /// <param name="module">The Module model to be updated.</param>
        public async Task<Module> UpdateModuleAsync(Module module)
        {
            _context.Module.Update(module);

            await _context.SaveChangesAsync();

            return module;
        }


    }
}
