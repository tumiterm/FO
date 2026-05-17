//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2025 22:16 PM
// Purpose:         Defines the ReportRepository class.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Report Repository.
    /// </summary>
    public class ReportRepository : Repository<Report>, IReports
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ReportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Report model in the repository.
        /// </summary>
        /// <param name="report">The Report model to be updated.</param>
        public async Task<Report> UpdateReportAsync(Report report)
        {
            _context.Reports.Update(report);

            await _context.SaveChangesAsync();

            return report;
        }
    }
}
