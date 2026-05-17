//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2025 22:25 PM
// Purpose:         Defines the ApplicationsRepository.


#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Infrastructure.Repository
{

    /// <summary>
    /// Represents a repository specifically for performing operations on the Application Repository.
    /// </summary>
    public class ApplicationsRepository : Repository<Domain.Entities.Application>, IApplications
    {

        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ApplicationsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Application model in the repository.
        /// </summary>
        /// <param name="applications">The Application model to be updated.</param>
        public async Task<Domain.Entities.Application> UpdateApplicationAsync(Domain.Entities.Application application)
        {

            if(application.Status == Domain.Enums.EnumRegistry.ApplicationStatus.AptituteTest)
            {
                _context.Applications.Update(application);
            }

            await _context.SaveChangesAsync();

            return application;
        }
    }
}
