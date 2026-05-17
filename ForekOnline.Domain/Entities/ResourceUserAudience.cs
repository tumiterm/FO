// <copyright file="ResourceCategory.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Defines which specific users can see a specific resource.
    /// </summary>
    public class ResourceUserAudience
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
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the current context.
        /// </summary>
        public User? User { get; set; }
    }
}