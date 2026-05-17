// <copyright file="IContactPerson.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:13 PM
// Purpose:         Defines the IContactPerson interface.


using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IContactPerson : IRepository<ContactPerson>
    {
        Task<ContactPerson> UpdateContactPersonAsync(ContactPerson contactPerson);

    }
}
