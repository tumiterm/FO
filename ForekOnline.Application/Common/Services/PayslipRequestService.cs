// <copyright file="PayslipRequestService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 10:09:27 AM
// Purpose:         Defines the PayslipRequestService class


#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
#endregion



namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides services for managing payslip requests, including creating requests, 
    /// uploading documents, and notifying relevant parties.
    /// </summary>
    /// <remarks>
    /// This service adheres to clean architecture principles, including separation of concerns, 
    /// dependency injection, and defensive programming practices.
    /// </remarks>
    public class PayslipRequestService : IPayslipRequestService
    {
        #region Private ReadOnly
        private readonly IUnitOfWork _context;
        private readonly IHelperService _helperService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        public PayslipRequestService(IUnitOfWork context, IHelperService helperService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _helperService = helperService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Creates a new payslip request for an employee.
        /// </summary>
        /// <param name="model">The data transfer object containing payslip request details.</param>
        /// <param name="employeeId">The unique identifier of the employee.</param>
        /// <returns>A validation response indicating the result of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the model is null.</exception>
        /// <exception cref="ArgumentException">Thrown if employeeId is invalid.</exception>
        public async Task<ValidationResponse> CreateRequestAsync(PayslipRequestViewModel model, Guid employeeId)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (employeeId == Guid.Empty) throw new ArgumentException("Employee ID cannot be empty.", nameof(employeeId));

            try
            {

                var request = PayslipRequestMapper(model, employeeId);

               // await _helperService.AttachmentUploaderAsync<PayslipRequest>(request, "Uploads/Payslips", entity => entity.DocumentFile, (entity, fileName) => entity.Document = fileName);

                var createPayslip = await _context.Payslip.AddAsync(request);

                var saveResult = await _context.SaveAsync();

                if (saveResult <= 0)
                {
                    return _helperService.ErrorResponse("Error: Payslip Request not saved!");
                }

                await SendEmailNotificationAsync(model);

                return _helperService.SuccessResponse("Payslip Request created successfully.");
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occurred while creating the payslip request.", ex);
            }
        }

        /// <summary>
        /// Notifies the campus manager regarding a specific payslip request.
        /// </summary>
        /// <param name="request">The payslip request to notify about.</param>
        /// <exception cref="ArgumentNullException">Thrown if the request is null.</exception>
        public Task NotifyCampusManagerAsync(PayslipRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return Task.CompletedTask; // Placeholder until actual implementation.
        }

        /// <summary>
        /// Uploads a document for an employee's payslip request.
        /// </summary>
        /// <param name="employeeId">The unique identifier of the employee.</param>
        /// <param name="documentPath">The file path of the document to be uploaded.</param>
        /// <exception cref="ArgumentException">Thrown if employeeId or documentPath is invalid.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the employee is not found.</exception>
        public async Task UploadDocumentAsync(Guid employeeId, string document)
        {
            if (employeeId == Guid.Empty) throw new ArgumentException("Employee ID cannot be empty.", nameof(employeeId));

            if (string.IsNullOrWhiteSpace(document)) throw new ArgumentException("Document path cannot be empty or null.", nameof(document));

            var employee = await _context.Payslip.GetAsync(filter: e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                throw new KeyNotFoundException("Employee not found.");
            }

            try
            {
                employee.Document = document;

                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while uploading the document.", ex);
            }
        }


        /// <summary>
        /// Provides mapping functionality for converting between <see cref="PayslipRequest"/> entities 
        /// and their corresponding view models or DTOs.
        /// </summary>
        /// <remarks>
        /// This class ensures that the mapping logic is separated from business logic, adhering to 
        /// the single-responsibility principle.
        /// </remarks>
        private PayslipRequest PayslipRequestMapper(PayslipRequestViewModel dto, Guid employeeId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new PayslipRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                DocumentType = dto.DocumentType,
                Document = dto.Document,
                Reason = dto.Reason,
                StartMonth = dto.StartMonth,
                EndMonth = dto.EndMonth,
                PayslipUploaded = dto.PayslipUploaded,
                Status = "Pending",
                UploadDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                CreatedAt = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                UpdatedAt = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
            };
        }

        private EmailDataViewModel NotifyGeneralManager(User user, PayslipRequestViewModel model)
        {
            return new EmailDataViewModel
            {
                Subject = "Forek Online",
                Body = _helperService.OnSendPayslipRequestMail(user, model),
                From = "Forek Online",
                Header = "Forek Online",
                Recipient = "tmanyimo@forekinstitute.co.za"
            };
        }

        /// <summary>
        /// Retrieves the current user from the session data.
        /// </summary>
        /// <returns>The current <see cref="User"/> object, or null if no user is found.</returns>
        private User? OnGetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return null;
            }

            string? sessionUserJson = _httpContextAccessor.HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrWhiteSpace(sessionUserJson))
            {
                return null;
            }

            try
            {
                User? user = JsonConvert.DeserializeObject<User>(sessionUserJson);

                if (user == null)
                {
                    return null;
                }

                return user;
            }
            catch (JsonException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task SendEmailNotificationAsync(PayslipRequestViewModel model)
        {
            var mail = NotifyGeneralManager(OnGetCurrentUser(), model);

            await _helperService.SendMailNotificationAsync(mail);
        }

    }


}


