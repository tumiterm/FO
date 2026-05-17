// <copyright file="WorkplaceModulesRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:52 PM
// Purpose:         Defines the WorkplaceModulesRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the WorkplaceModules Repository.
    /// </summary>
    public class WorkplaceModulesRepository : Repository<LearnerWorkplaceModules>, ILearnerWorkplaceModule
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkplaceModulesRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public WorkplaceModulesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing LearnerWorkplaceModules model in the repository.
        /// </summary>
        /// <param name="learnerWorkplaceModules">The LearnerWorkplaceModules model to be updated.</param>
        public async Task<LearnerWorkplaceModules> UpdateWorkPlaceModuleAsync(LearnerWorkplaceModules learnerWorkplaceModules)
        {
            _context.WorkplaceModules.Update(learnerWorkplaceModules);

            await _context.SaveChangesAsync();

            return learnerWorkplaceModules;
        }
    }
}
