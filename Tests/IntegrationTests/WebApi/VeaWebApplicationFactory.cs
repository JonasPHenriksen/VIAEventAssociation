using EfcDataAccess;
using EfcDataAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Contracts;

namespace IntegrationTests.WebApi;

internal class VeaWebApplicationFactory : WebApplicationFactory<Program>
{
    private IServiceCollection serviceCollection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
// setup extra test services.
        builder.ConfigureTestServices(services =>
        {
            serviceCollection = services;
// Remove the existing DbContexts and Options
            services.RemoveAll(typeof(DbContextOptions<MyDbContext>));
            services.RemoveAll(typeof(DbContextOptions<VeadatabaseProductionContext>));
            services.RemoveAll<MyDbContext>();
            services.RemoveAll<VeadatabaseProductionContext>();
            string connString = GetConnectionString();
            services.AddDbContext<MyDbContext>(options => { options.UseSqlite(connString); });
            services.AddDbContext<VeadatabaseProductionContext>(options => { options.UseSqlite(connString); });
            services.AddScoped<ISystemTime, MockTime.SystemTime>(); 
            SetupCleanDatabase(services);
        });
    }

    private void SetupCleanDatabase(IServiceCollection services)
    {
        MyDbContext dmContext = services.BuildServiceProvider().GetService<MyDbContext>()!;
        dmContext.Database.EnsureDeleted();
        dmContext.Database.EnsureCreated();
// could seed database here?
    }

    private string GetConnectionString()
    {
        string testDbName = "Test" + Guid.NewGuid() + ".db";
        return "Data Source = " + testDbName;
    }

    protected override void Dispose(bool disposing)
    {
// clean up the database
        MyDbContext dmContext = serviceCollection.BuildServiceProvider().GetService<MyDbContext>()!;
        dmContext.Database.EnsureDeleted();
        base.Dispose(disposing);
    }
}