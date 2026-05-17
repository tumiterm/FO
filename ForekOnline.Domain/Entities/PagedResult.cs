// <copyright file="PagedResult.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the PagedResult class

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a paged result set containing a subset of items and pagination metadata.
    /// </summary>
    /// <remarks>Use this class to encapsulate the results of a paginated query, including the current page of
    /// items and information needed to navigate between pages. The class provides properties to determine the current
    /// page, total number of items, total pages, and whether there are additional pages available.</remarks>
    /// <typeparam name="T">The type of the items contained in the paged result.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Gets or sets the current page number in a paginated result set.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the number of items to include on each page of results.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of items available.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets the total number of pages based on the total item count and the page size.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Gets or sets the collection of items contained in the list.
        /// </summary>
        public IReadOnlyList<T> Items { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a next page available in the paginated sequence.
        /// </summary>
        public bool HasNext => Page < TotalPages;

        /// <summary>
        /// Gets a value indicating whether there is a previous page available in the paginated sequence.
        /// </summary>
        public bool HasPrevious => Page > 1;
    }
}
