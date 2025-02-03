using starterkit.Core.Modules.Tenant;

namespace starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing users in tenant context
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by ID with profile and address
        /// </summary>
        Task<User> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all users with profiles and addresses
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Gets a paged list of users with profiles and addresses
        /// </summary>
        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new user with profile and address
        /// </summary>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Updates an existing user with profile and address
        /// </summary>
        Task<User> UpdateAsync(User user);

        /// <summary>
        /// Deletes a user (soft delete)
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a user exists by email
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);

        /// <summary>
        /// Gets a user by email
        /// </summary>
        Task<User> GetByEmailAsync(string email);

        /// <summary>
        /// Updates user password
        /// </summary>
        Task UpdatePasswordAsync(Guid id, string passwordHash);
    }
} 