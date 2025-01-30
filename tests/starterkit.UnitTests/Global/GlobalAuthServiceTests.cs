using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using starterkit.Application.DTOs.Global.Auth;
using starterkit.Application.Services.Global;
using starterkit.Core.Entities.Global;
using starterkit.Core.Enums;
using starterkit.Core.Exceptions.Auth;
using starterkit.Core.Interfaces.Data;
using Xunit;

namespace starterkit.UnitTests.Global
{
    public class GlobalAuthServiceTests
    {
        private readonly Mock<IRootDbContext> _mockContext;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<GlobalAuthService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GlobalAuthService _service;

        public GlobalAuthServiceTests()
        {
            _mockContext = new Mock<IRootDbContext>();
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<GlobalAuthService>>();
            _mockMapper = new Mock<IMapper>();

            _service = new GlobalAuthService(
                _mockContext.Object,
                _mockConfig.Object,
                _mockLogger.Object);

            // Common configuration setup
            _mockConfig.Setup(x => x["JWT:BaseTokenSecret"]).Returns("your-256-bit-secret");
            _mockConfig.Setup(x => x["JWT:Issuer"]).Returns("your-issuer");
            _mockConfig.Setup(x => x["JWT:Audience"]).Returns("your-audience");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var request = new GlobalLoginRequestDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new GlobalUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashed_password",
                FirstName = "Test",
                LastName = "User",
                UserType = UserType.Admin
            };

            var tenantMapping = new TenantUserMapping
            {
                TenantId = Guid.NewGuid(),
                UserId = user.Id,
                IsActive = true,
                Tenant = new Tenant { Name = "Test Tenant", Status = TenantStatus.Active }
            };

            var mockUsers = new List<GlobalUser> { user }.AsQueryable();
            var mockMappings = new List<TenantUserMapping> { tenantMapping }.AsQueryable();

            var mockUsersDbSet = new Mock<DbSet<GlobalUser>>();
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.Provider).Returns(mockUsers.Provider);
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.Expression).Returns(mockUsers.Expression);
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.ElementType).Returns(mockUsers.ElementType);
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.GetEnumerator()).Returns(mockUsers.GetEnumerator());

            var mockMappingsDbSet = new Mock<DbSet<TenantUserMapping>>();
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.Provider).Returns(mockMappings.Provider);
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.Expression).Returns(mockMappings.Expression);
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.ElementType).Returns(mockMappings.ElementType);
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.GetEnumerator()).Returns(mockMappings.GetEnumerator());

            _mockContext.Setup(x => x.GlobalUsers).Returns(mockUsersDbSet.Object);
            _mockContext.Setup(x => x.TenantUserMappings).Returns(mockMappingsDbSet.Object);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal($"{user.FirstName} {user.LastName}", result.FullName);
            Assert.NotNull(result.BaseToken);
            Assert.NotNull(result.AvailableTenants);
            Assert.Single(result.AvailableTenants);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var request = new GlobalLoginRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "wrongpassword"
            };

            var mockUsers = new List<GlobalUser>().AsQueryable();
            var mockDbSet = new Mock<DbSet<GlobalUser>>();
            mockDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.Provider).Returns(mockUsers.Provider);
            mockDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.Expression).Returns(mockUsers.Expression);
            mockDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.ElementType).Returns(mockUsers.ElementType);
            mockDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.GetEnumerator()).Returns(mockUsers.GetEnumerator());

            _mockContext.Setup(x => x.GlobalUsers).Returns(mockDbSet.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _service.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_WithNoActiveTenants_ThrowsTenantAccessDeniedException()
        {
            // Arrange
            var request = new GlobalLoginRequestDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new GlobalUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashed_password"
            };

            var mockUsers = new List<GlobalUser> { user }.AsQueryable();
            var mockMappings = new List<TenantUserMapping>().AsQueryable();

            var mockUsersDbSet = new Mock<DbSet<GlobalUser>>();
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.Provider).Returns(mockUsers.Provider);
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.Expression).Returns(mockUsers.Expression);
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.ElementType).Returns(mockUsers.ElementType);
            mockUsersDbSet.As<IQueryable<GlobalUser>>().Setup(m => m.GetEnumerator()).Returns(mockUsers.GetEnumerator());

            var mockMappingsDbSet = new Mock<DbSet<TenantUserMapping>>();
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.Provider).Returns(mockMappings.Provider);
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.Expression).Returns(mockMappings.Expression);
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.ElementType).Returns(mockMappings.ElementType);
            mockMappingsDbSet.As<IQueryable<TenantUserMapping>>().Setup(m => m.GetEnumerator()).Returns(mockMappings.GetEnumerator());

            _mockContext.Setup(x => x.GlobalUsers).Returns(mockUsersDbSet.Object);
            _mockContext.Setup(x => x.TenantUserMappings).Returns(mockMappingsDbSet.Object);

            // Act & Assert
            await Assert.ThrowsAsync<TenantAccessDeniedException>(() => _service.LoginAsync(request));
        }
    }
} 