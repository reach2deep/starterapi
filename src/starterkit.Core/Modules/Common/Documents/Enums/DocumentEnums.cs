using System;

namespace starterkit.Core.Modules.Common.Documents.Enums
{
    /// <summary>
    /// Defines the type of module a document belongs to
    /// </summary>
    public enum ModuleType
    {
        /// <summary>
        /// User profile related documents
        /// </summary>
        Profile = 0,

        /// <summary>
        /// Block related documents
        /// </summary>
        Block = 1,

        /// <summary>
        /// Unit related documents
        /// </summary>
        Unit = 2,

        /// <summary>
        /// Community related documents
        /// </summary>
        Community = 3,

        /// <summary>
        /// Other documents
        /// </summary>
        Other = 99
    }

    /// <summary>
    /// Defines the status of a document
    /// </summary>
    public enum DocumentStatus
    {
        /// <summary>
        /// Document is active and available
        /// </summary>
        Active = 0,

        /// <summary>
        /// Document has been deleted (soft delete)
        /// </summary>
        Deleted = 1,

        /// <summary>
        /// Document is archived
        /// </summary>
        Archived = 2
    }

    /// <summary>
    /// Defines the type of document
    /// </summary>
    public enum DocumentType
    {
        /// <summary>
        /// Image file
        /// </summary>
        Image = 0,

        /// <summary>
        /// Document file (Word, text, etc.)
        /// </summary>
        Document = 1,

        /// <summary>
        /// Spreadsheet file
        /// </summary>
        Spreadsheet = 2,

        /// <summary>
        /// Presentation file
        /// </summary>
        Presentation = 3,

        /// <summary>
        /// PDF file
        /// </summary>
        PDF = 4,

        /// <summary>
        /// Other file type
        /// </summary>
        Other = 99
    }

    /// <summary>
    /// Defines the type of access for document logging
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// Document was viewed
        /// </summary>
        View = 0,

        /// <summary>
        /// Document was downloaded
        /// </summary>
        Download = 1,

        /// <summary>
        /// Document was deleted
        /// </summary>
        Delete = 2,

        /// <summary>
        /// Document was updated
        /// </summary>
        Update = 3
    }
} 