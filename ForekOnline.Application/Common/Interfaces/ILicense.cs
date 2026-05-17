// <copyright file="ILicense.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 15:35 PM
// Purpose:         Defines the ILicense interface.


using ForekOnline.Domain.Entities;


namespace ForekOnline.Application.Common.Interfaces
{
    public interface ILicense : IRepository<License>
    {
        Task<License> UpdateLicenseAsync(License License);

    }
}
