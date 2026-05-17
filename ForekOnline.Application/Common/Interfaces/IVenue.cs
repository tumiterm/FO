// <copyright file="IVenue.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    15/03/2026
// Purpose:         Defines the IVenue repository interface

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Repository interface for <see cref="Venue"/> entities.
    /// </summary>
    public interface IVenue : IRepository<Venue>
    {
        Task<Venue> UpdateVenueAsync(Venue venue);
    }
}