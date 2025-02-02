using System;
using System.Collections.Generic;

namespace starterkit.Application.Modules.Tenant.RoleManagement.DTOs
{
    /// <summary>
    /// Request DTO for removing roles from a user
    /// </summary>
    public class RemoveUserRolesRequest
    {
        /// <summary>
        /// List of role IDs to remove from the user
        /// </summary>
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
} 