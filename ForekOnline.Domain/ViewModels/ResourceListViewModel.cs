// <copyright file="ResourceListViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    15/03/2025 09:09 AM
// Purpose:         Defines the ResourceListViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion


namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a list of resources, including available categories.
    /// </summary>
    public record ResourceListViewModel
    {
        /// <summary>
        /// Gets or sets the list of uploaded resources.
        /// </summary>
        public List<ResourceUploadViewModel> Resources { get; set; }

        /// <summary>
        /// Gets or sets the list of available resource categories.
        /// </summary>
        public List<SelectListItem> Categories { get; set; }
    }

}
