using System.Reflection;
using AppEntry;
using Application.Common.CommandDispatcher;
using EfcDataAccess;
using EfcDataAccess.Context;
using EfcDataAccess.Repositories;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using QueryContracts;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connString =
    @"Data Source = /home/jonas/RiderProjects/VIAEventAssociation/Infrastructure/EfcDataAccess/VEADatabaseProduction.db";

builder.Services.RegisterCommandHandlers(typeof(ICommandHandler<,>).Assembly);
builder.Services.RegisterCommandDispatching();
//builder.Services.RegisterReadPersistence(connString);
//builder.Services.RegisterWritePersistence(connString);
builder.Services.RegisterQueryDispatching();
builder.Services.AddScoped<IEventRepository, EventRepositoryEfc>();
builder.Services.AddScoped<IGuestRepository, GuestRepositoryEfc>();
builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlite(connString), ServiceLifetime.Scoped);
builder.Services.AddScoped<IUnitOfWork, SqliteUnitOfWork>();


//TODO Mapper config missing if it is needed

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