using System;
using System.Collections.Generic;

namespace starterkit.Core.Modules.Common
{
    /// <summary>
    /// Base response DTO for lookup/dropdown data
    /// </summary>
    public class LookupDto
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Display text/label for the lookup item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Optional value if different from Id
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Optional group for grouping items in dropdowns
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Optional additional data as key-value pairs
        /// </summary>
        public Dictionary<string, string>? AdditionalData { get; set; }
    }

    /// <summary>
    /// Request DTO for lookup queries with optional filtering and pagination
    /// </summary>
    public class LookupRequest
    {
        /// <summary>
        /// Optional search term to filter results
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Optional page number for pagination, defaults to 1 if pagination is used
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Optional page size for pagination, defaults to 10 if pagination is used
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Optional filter criteria as key-value pairs
        /// </summary>
        public Dictionary<string, string>? Filters { get; set; }

        /// <summary>
        /// Optional flag to include inactive items, defaults to false
        /// </summary>
        public bool IncludeInactive { get; set; } = false;

        /// <summary>
        /// Optional sort field
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Optional sort direction (asc/desc), defaults to asc
        /// </summary>
        public string? SortDirection { get; set; }
    }

    /// <summary>
    /// Response DTO wrapping lookup results with pagination info
    /// </summary>
    public class LookupResponse<T> where T : LookupDto
    {
        /// <summary>
        /// List of lookup items
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Total number of items (for pagination)
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number if pagination was requested
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Items per page if pagination was requested
        /// </summary>
        public int? PageSize { get; set; }
    }
} 