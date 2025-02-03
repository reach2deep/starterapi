namespace starterkit.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for password hashing operations
    /// </summary>
    public interface IPasswordHashService
    {
        /// <summary>
        /// Hashes a password
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        bool VerifyPassword(string password, string passwordHash);
    }
} 