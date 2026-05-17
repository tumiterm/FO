// <copyright file="SyncMetadata.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 21:16 AM
// Purpose:         Tracks API sync status for the SQLite cache.

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Tracks the last successful synchronization of student data from the external API.
    /// </summary>
    public class SyncMetadata
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity associated with this instance.
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the last synchronization occurred.
        /// </summary>
        public DateTime LastSyncUtc { get; set; }

        /// <summary>
        /// Gets or sets the total number of records in the collection.
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool WasSuccessful { get; set; }
    }
}