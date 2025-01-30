using Microsoft.AspNetCore.Mvc;
using starterkit.starterkit.Core.Modules.Common;

namespace starterkit.starterkit.API.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected const string UserIdClaimType = System.Security.Claims.ClaimTypes.NameIdentifier;

        /// <summary>
        /// Gets the current user's ID from claims
        /// </summary>
        /// <returns>User ID if authenticated, null if not</returns>
        protected Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(UserIdClaimType)?.Value;
            return userIdString != null ? Guid.Parse(userIdString) : null;
        }

        /// <summary>
        /// Creates an unauthorized response with standardized format
        /// </summary>
        protected ActionResult<ApiResponse<T>> Unauthorized<T>(string message = "User not authenticated")
        {
            return base.Unauthorized(ApiResponse<T>.CreateError(
                message: message,
                code: "UNAUTHORIZED"
            ));
        }
    }
}