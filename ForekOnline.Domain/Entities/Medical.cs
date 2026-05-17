// <copyright file="Medical.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the Medical class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a medical record associated with a specific application.
    /// </summary>
    [SkipAuditInterceptor]
    public class Medical
    {
        /// <summary>
        /// Gets or sets the unique identifier for the medical record.
        /// </summary>
        [Key]
        public Guid MedicalId { get; set; }

        /// <summary>
        /// Gets or sets the name of the medical provider or service associated with the record.
        /// </summary>
        public string? MedicalName { get; set; }

        /// <summary>
        /// Gets or sets the member number of the person in the medical system.
        /// </summary>
        public string? MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the telephone number associated with the medical record.
        /// </summary>
        public string? Telephone { get; set; }

        /// <summary>
        /// Gets or sets the disability status, if applicable, related to the individual.
        /// </summary>
        public string? Disability { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the application associated with this medical record.
        /// </summary>
        [ForeignKey(nameof(Application))]
        public Guid ApplicationId { get; set; }
    }
}
