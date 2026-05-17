// <copyright file="IResource.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    12-03-2025 16:05 AM
// Purpose:         Defines the IResource interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IResource : IRepository<Resource>
    {
        Task<Resource> UpdateResourceAsync(Resource resource);

    }
}
