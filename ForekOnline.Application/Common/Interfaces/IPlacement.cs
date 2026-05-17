// <copyright file="IPlacement.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:21 PM
// Purpose:         Defines the IPlacement interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IPlacement : IRepository<Placement>
    {
        Task<Placement> UpdatePlacementAsync(Placement placement);
    }
}
