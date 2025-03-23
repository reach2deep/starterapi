using System;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Common.Documents.Enums;

namespace starterkit.Core.Modules.Common.Documents.Entities
{
    /// <summary>
    /// Logs access to documents for auditing and analytics
    /// </summary>
    public class DocumentAccessLog : BaseEntity
    {
        /// <summary>
        /// ID of the document that was accessed
        /// </summary>
        public Guid DocumentId { get; set; }
        
        /// <summary>
        /// ID of the user who accessed the document (null if anonymous)
        /// </summary>
        public Guid? AccessedById { get; set; }
        
        /// <summary>
        /// IP address of the requester
        /// </summary>
        public string IpAddress { get; set; }
        
        /// <summary>
        /// Date and time when the document was accessed
        /// </summary>
        public DateTime AccessedAt { get; set; }
        
        /// <summary>
        /// Type of access performed
        /// </summary>
        public AccessType AccessType { get; set; }
        
        /// <summary>
        /// Navigation property to the document
        /// </summary>
        public virtual Document Document { get; set; }
    }
} 