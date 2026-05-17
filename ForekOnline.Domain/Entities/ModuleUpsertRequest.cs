// <copyright file="ModuleUpsertRequest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 19:09:27 PM
// Purpose:         Defines the ModuleUpsertRequest class

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a request to create or update a module, including its identifying information and attributes.
    /// </summary>
    /// <remarks>Use this class to supply the necessary data when creating a new module or updating an
    /// existing one. If <see cref="ModuleId"/> is null, a new module will be created; otherwise, the specified module
    /// will be updated. All properties should be set according to the requirements of the module being created or
    /// modified.</remarks>
    public class ModuleUpsertRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the module.
        /// </summary>
        /// <remarks>If the value is <see langword="null"/>, a new module will be created. Otherwise, the
        /// specified module will be used or updated.</remarks>
        public Guid? ModuleId { get; set; }  
        
        /// <summary>
        /// Gets or sets the name of the module associated with this instance.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the National Qualifications Framework (NQF) level associated with the qualification.
        /// </summary>
        public eNQF? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the credit amount associated with the account.
        /// </summary>
        public double? Credit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
