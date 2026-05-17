// <copyright file="IMaterial.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 15:37 PM
// Purpose:         Defines the IMaterial interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IMaterial : IRepository<Material>
    {
        Task<Material> UpdateMaterialAsync(Material material);
    }
}
