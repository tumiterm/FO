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
    /// Repository interface for <see cref="VenueReservation"/> entities.
    /// </summary>
    public interface IVenueReservation : IRepository<VenueReservation>
    {
        /// <summary>
        /// Asynchronously updates an existing venue reservation with the specified details.
        /// </summary>
        /// <param name="reservation">The reservation to update. Must contain a valid reservation identifier and the updated reservation details.
        /// Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated venue reservation.</returns>
        Task<VenueReservation> UpdateReservationAsync(VenueReservation reservation);
    }
}