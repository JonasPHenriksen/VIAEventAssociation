using System.Reflection;
using Application.Common.CommandDispatcher;
using EfcDataAccess;
using QueryContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connString =
    @"Data Source = /home/jonas/RiderProjects/VIAEventAssociation/Infrastructure/EfcDataAccess/VEADatabaseProduction.db";

builder.Services.RegisterCommandHandlers(Assembly.GetExecutingAssembly());
builder.Services.RegisterCommandDispatching();
builder.Services.RegisterReadPersistence(connString);
builder.Services.RegisterWritePersistence(connString);
builder.Services.RegisterQueryDispatching();
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