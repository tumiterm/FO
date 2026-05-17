
// <copyright file="IPayslip.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    26-06-2025 22:32 PM
// Purpose:         Defines the IPayslip interface.

using ForekOnline.Domain.Entities;


namespace ForekOnline.Application.Common.Interfaces
{
    public interface IPayslip : IRepository<PayslipRequest>
    {
        Task<PayslipRequest> UpdatePayslipAsync(PayslipRequest application);

    }
}
