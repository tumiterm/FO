// <copyright file="ResourceCategory.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Defines which system roles can see a specific resource.
    /// </summary>
    public class ResourceRoleAudience
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the resource.
        /// </summary>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the resource associated with the current context.
        /// </summary>
        public Resource? Resource { get; set; }

        /// <summary>
        /// Gets or sets the role assigned to the user within the system.
        /// </summary>
        public eSysRole Role { get; set; }
    }
}