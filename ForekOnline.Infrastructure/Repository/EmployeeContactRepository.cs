//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    26/Feb/2025 18:53 PM
// Purpose:         Defines the EmployeeContactRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Course Repository.
    /// </summary>
    public class EmployeeContactRepository : Repository<EmployeeContact>, IEmployeeContact
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactRepository"/> class.
        /// </summary>
        /// <param name="context">The EmployeeContact database context.</param>
        public EmployeeContactRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing EmployeeContact model in the repository.
        /// </summary>
        /// <param name="employeeContact">The EmployeeContact model to be updated.</param>
        public async Task<EmployeeContact> UpdateEmployeeContactAsync(EmployeeContact employeeContact)
        {
            _context.EmployeeContact.Update(employeeContact);

            await _context.SaveChangesAsync();

            return employeeContact;
        }
    }
}
