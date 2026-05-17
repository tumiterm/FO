// <copyright file="Resource.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/03/2025 16:00 AM
// Purpose:         Defines the Resource class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a resource entity, including metadata and storage/link details.
    /// </summary>
    public class Resource /*: EntityBase<Guid>*/
    {
        /// <summary>
        /// Gets or sets the type of resource (File or ExternalLink).
        /// </summary>
        public eResourceType Type { get; set; }

        /// <summary>
        /// Gets or sets the URL for legacy compatibility and UI navigation.
        /// For files: can be a provider URL (if available) or a generated presigned URL.
        /// For links: equals <see cref="ExternalUrl"/>.
        /// </summary>
        public string FileURL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the external URL for link-type resources.
        /// </summary>
        public string? ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets the stored file identifier returned by <c>IFileUploadService</c>.
        /// </summary>
        public string? StoredFileId { get; set; }

        /// <summary>
        /// Gets or sets the storage provider name used to store the file.
        /// </summary>
        public string? StoredFileProvider { get; set; }

        /// <summary>
        /// Gets or sets tags as comma-separated values for basic search/filter.
        /// </summary>
        public string? TagsCsv { get; set; }

        /// <summary>
        /// Gets or sets the number of times the resource has been viewed.
        /// </summary>
        public int ViewsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of times the resource has been downloaded/opened.
        /// </summary>
        public int DownloadsCount { get; set; }

        /// <summary>
        /// Gets or sets the category ID associated with the resource.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the category details of the resource.
        /// </summary>
        public ResourceCategory? Category { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this resource is visible to all users.
        /// If false, then Role/User audience rules apply.
        /// </summary>
        public bool IsPublic { get; set; } = true;

        /// <summary>
        /// Gets or sets role-based audience targeting rules.
        /// </summary>
        public ICollection<ResourceRoleAudience> RoleAudiences { get; set; } = new List<ResourceRoleAudience>();

        /// <summary>
        /// Gets or sets user-based audience targeting rules.
        /// </summary>
        public ICollection<ResourceUserAudience> UserAudiences { get; set; } = new List<ResourceUserAudience>();

        #region IEntity

        /// <summary>
        /// Gets or sets the unique code of the resource.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the resource was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date and time the resource was last modified.
        /// </summary>
        public DateTimeOffset DateModified { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the resource.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the name (title) of the resource.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the resource.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user who created the resource.
        /// </summary>
        public string UserCreated { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user who last modified the resource.
        /// </summary>
        public string UserModified { get; set; } = string.Empty;

        #endregion
    }
}