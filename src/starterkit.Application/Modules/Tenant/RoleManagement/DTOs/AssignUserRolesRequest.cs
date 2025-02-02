using System;
using System.Collections.Generic;

namespace starterkit.Application.Modules.Tenant.RoleManagement.DTOs
{
    /// <summary>
    /// Request DTO for assigning roles to a user
    /// </summary>
    public class AssignUserRolesRequest
    {
        /// <summary>
        /// List of role IDs to assign to the user
        /// </summary>
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
} 