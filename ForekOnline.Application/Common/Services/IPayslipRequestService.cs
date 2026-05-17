
// <copyright file="IPayslipRequestService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 10:09:27 AM
// Purpose:         Defines the IPayslipRequestService class

#region Usings
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Service for handling payslip and IRP5 requests.
    /// </summary>
    public interface IPayslipRequestService
    {
        /// <summary>
        /// Creates a new payslip or IRP5 request.
        /// </summary>
        /// <param name="model">The request details.</param>
        /// <param name="employeeId">The ID of the employee making the request.</param>
        Task<ValidationResponse> CreateRequestAsync(PayslipRequestViewModel model, Guid employeeId);

        /// <summary>
        /// Sends an email notification to the Campus Manager about the request.
        /// </summary>
        /// <param name="request">The request details.</param>
        Task NotifyCampusManagerAsync(PayslipRequest request);

        /// <summary>
        /// Uploads the requested document to the employee's profile.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="documentPath">The path to the uploaded document.</param>
        Task UploadDocumentAsync(Guid employeeId, string documentPath);
    }

}
