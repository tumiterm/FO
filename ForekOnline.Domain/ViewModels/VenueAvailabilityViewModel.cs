// <copyright file="VenueAvailabilityViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a venue in the filtered availability list.
    /// </summary>
    public class VenueAvailabilityViewModel
    {
        public Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Campus { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public eVenueType VenueType { get; set; }
        public string? EquipmentChecklist { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsAvailable { get; set; }
        public string? UnavailableReason { get; set; }
        public bool IsSuggested { get; set; }
    }
}