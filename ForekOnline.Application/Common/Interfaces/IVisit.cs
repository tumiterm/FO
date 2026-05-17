// <copyright file="IVisit.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 15:46 PM
// Purpose:         Defines the IVisit interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IVisit : IRepository<Visit>
    {
        Task<Visit> UpdateVisitAsync(Visit visit);

    }
}
