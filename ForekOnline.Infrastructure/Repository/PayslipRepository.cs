//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    26/Feb/2025 18:48 PM
// Purpose:         Defines the PayslipRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{

    /// <summary>
    /// Represents a repository specifically for performing operations on the Payslip Repository.
    /// </summary>
    public class PayslipRepository : Repository<PayslipRequest>, IPayslip
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayslipRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public PayslipRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing PayslipRequest model in the repository.
        /// </summary>
        /// <param name="payslipRequest">The PayslipRequest model to be updated.</param>
        public async Task<PayslipRequest> UpdatePayslipAsync(PayslipRequest payslipRequest)
        {
            _context.PayslipRequest.Update(payslipRequest);

            await _context.SaveChangesAsync();

            return payslipRequest;
        }
    }
}
