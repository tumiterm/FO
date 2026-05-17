// <copyright file="StudentRelatedDataViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/03/2025 20:44 PM
// Purpose:         Defines the StudentRelatedDataViewModel class

using ForekOnline.Domain.Entities;

namespace ForekOnline.Domain.ViewModels
{
    public record StudentRelatedDataViewModel
    {
        public Student Student { get; set; }
        public ICollection<EnrollmentHistory> EnrollmentHistory { get; set; }
    }
}
