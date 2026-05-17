// <copyright file="ReportSubReportRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-03-2026 21:09 PM
// Purpose:         Defines the ReportSubReportRepository Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the ReportSubReport Repository.
    /// </summary>
    public class ReportSubReportRepository : Repository<ReportSubReport>, IReportSubReport
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportSubReportRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ReportSubReportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing ReportSubReport model in the repository.
        /// </summary>
        /// <param name="reportSubReport">The ReportSubReport model to be updated.</param>
        public async Task<ReportSubReport> UpdateReportSubReportAsync(ReportSubReport reportSubReport)
        {
            _context.ReportSubReport.Update(reportSubReport);

            await _context.SaveChangesAsync();

            return reportSubReport;
        }
    }
}
