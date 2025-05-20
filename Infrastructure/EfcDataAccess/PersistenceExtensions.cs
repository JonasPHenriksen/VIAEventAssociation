using EfcDataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfcDataAccess;

public static class PersistenceExtensions
{
    public static IServiceCollection RegisterReadPersistence(this IServiceCollection services, string connString)
    {
        services.AddDbContext<DbContext, MyDbContext>(options => options.UseSqlite(connString), contextLifetime: ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection RegisterWritePersistence(this IServiceCollection services, string connString)
    {
        services.AddDbContext<DbContext, MyDbContext>(options => options.UseSqlite(connString), contextLifetime: ServiceLifetime.Scoped);
        return services;
    }

}