// <copyright file="IMedical.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 15:39 PM
// Purpose:         Defines the IMedical interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IMedical : IRepository<Medical>
    {
        Task<Medical> UpdateMedicalRecordAsync(Medical medical);
    }
}
