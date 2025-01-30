using starterkit.starterkit.Core.Modules.Global;

namespace starterkit.starterkit.Application.Modules.Global.TenantManagement.Interfaces
{
    public interface ITenantDatabaseInitializer
    {
        Task InitializeTenantDatabaseAsync(Core.Modules.Global.Tenant tenant);
        Task InitializeTenantDatabaseAsync(string databaseName);
    }
}