using System.Reflection;
using AppEntry;
using Application.Common.CommandDispatcher;
using EfcDataAccess;
using EfcDataAccess.Context;
using EfcDataAccess.Repositories;
using EfcMappingExamples;
using EfcQueries;
using Microsoft.EntityFrameworkCore;
using ObjectMapper;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connString =
    @"Data Source = /home/jonas/RiderProjects/VIAEventAssociation/Infrastructure/EfcDataAccess/VEADatabaseProduction.db";

builder.Services.RegisterCommandHandlers(typeof(ICommandHandler<,>).Assembly).RegisterCommandDispatching();
builder.Services.RegisterQueryHandlers(typeof(SingleEventQueryHandler).Assembly).RegisterQueryDispatching();
//builder.Services.RegisterReadPersistence(connString);
//builder.Services.RegisterWritePersistence(connString);
builder.Services.AddScoped<IEventRepository, EventRepositoryEfc>();
builder.Services.AddScoped<IGuestRepository, GuestRepositoryEfc>(); //TODO cleanup and better injection is possible
builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlite(connString), ServiceLifetime.Scoped);
builder.Services.AddDbContext<VeadatabaseProductionContext>(options => options.UseSqlite(connString), ServiceLifetime.Scoped);
builder.Services.AddScoped<ISystemTime, SystemTime>();
builder.Services.AddScoped<IUnitOfWork, SqliteUnitOfWork>();
builder.Services.AddScoped<IMapper, DefaultObjectMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();