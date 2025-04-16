using EfcDataAccess.Context;
using EfcDataAccess.Repositories;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit.Abstractions;

namespace IntegrationTests.Repositories;

public class GuestRepositoryEfcTests
{
    private const string EmailAddress = "330943@via.dk";
    private const string FirstName = "John";
    private const string LastName = "Doe";
    private const string ProfilePictureUrl = "https://example.com/profile.jpg";
    private static readonly Uri ValidProfilePictureUrl = new Uri(ProfilePictureUrl);
    
    private readonly ITestOutputHelper _output;

    public GuestRepositoryEfcTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static Guest GetGuest()
    {
        var factory = GuestFactory.Init()
            .WithEmail(Email.Create(EmailAddress).Value)
            .WithFirstName(Name.Create(FirstName).Value)
            .WithLastName(Name.Create(LastName).Value)
            .WithProfilePicture(ValidProfilePictureUrl);
        
        return factory.Build().Value;
    }
    
    [Fact]
    public async Task AddAndGetGuest_ShouldReturnSameGuest()
    {
        using var context = MyDbContext.SetupContext();
        var repo = new GuestRepositoryEfc(context);
        var unitOfWork = new SqliteUnitOfWork(context);
        
        var guest = GetGuest();

        await repo.AddAsync(guest);
        await unitOfWork.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var loaded = await repo.GetByGuestIdAsync(guest.GuestId);
        Assert.Equal(guest.Id, loaded?.Id);
    }
    
    [Fact]
    public async Task AddAndGet_EventWithParticipantsAndInvitations_ShouldBeSame()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var guestRepo = new GuestRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var guest = GetGuest();
        var guest2 = GetGuest();

        await guestRepo.AddAsync(guest);
        await guestRepo.AddAsync(guest2);
        await uow.SaveChangesAsync();  

        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithTimeRange(DateTime.Now.AddDays(5), DateTime.Now.AddDays(5).AddHours(2))
            .Build();
        
        //veaEvent.Participate(guest.GuestId);
        veaEvent.InviteGuest(guest2.GuestId);
        
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        await LogDatabaseContentsAsync(context);
        
        context.ChangeTracker.Clear();
        
        var loaded = await eventRepo.GetAsync(veaEvent.Id);
        //var participants = context.Entry(loaded!).Collection("Participants");
        var invitations = context.Entry(loaded!).Collection("_invitations");

        //await participants.LoadAsync();
        await invitations.LoadAsync();

        Assert.Equal(veaEvent.Id, loaded.Id);
        //Assert.Single(context.Entry(loaded).Collection("Participants").CurrentValue as IEnumerable<object>);
        Assert.Single(context.Entry(loaded).Collection("_invitations").CurrentValue as IEnumerable<object>);
    }
    

    private async Task LogDatabaseContentsAsync(MyDbContext context)
    {
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        // Get the table names
        var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
        
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0);
                // Log data from each table
                await LogTableContentsAsync(connection, tableName);
            }
        }
    }
    private async Task LogTableContentsAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        
        using (var reader = await command.ExecuteReaderAsync())
        {
            _output.WriteLine($"Table: {tableName}");
            
            // Read and log each row of data
            while (await reader.ReadAsync())
            {
                var row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString();
                    row.Add($"{columnName}: {value}");
                }
                _output.WriteLine(string.Join(", ", row));
            }
        }
    }
    
    /*
    [Fact]
    public async Task AddAndGet_EventWithParticipantsAndInvitations_ShouldBeSame_Command_Dispatcher()
    {
        using var context = MyDbContext.SetupContext();
        var dispatcher = new CommandDispatcher(context);

        var guest = GetGuest();
        var guest2 = GetGuest();

        await dispatcher.Dispatch(new AddGuestCommand(guest));
        await dispatcher.Dispatch(new AddGuestCommand(guest2));

        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithTimeRange(DateTime.Now.AddDays(5), DateTime.Now.AddDays(5).AddHours(2))
            .Build();

        await dispatcher.Dispatch(new AddEventCommand(veaEvent));
        await dispatcher.Dispatch(new ParticipateInEventCommand(veaEvent.Id, guest.GuestId));
        await dispatcher.Dispatch(new InviteGuestCommand(veaEvent.Id, guest2.GuestId));

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var eventRepo = new EventRepositoryEfc(context);
        var loaded = await eventRepo.GetAsync(veaEvent.Id);

        var participants = context.Entry(loaded!).Collection("Participants");
        var invitations = context.Entry(loaded!).Collection("_invitations");

        await participants.LoadAsync();
        await invitations.LoadAsync();

        Assert.Equal(veaEvent.Id, loaded.Id);
        Assert.Single(participants.CurrentValue as IEnumerable<object>);
        Assert.Single(invitations.CurrentValue as IEnumerable<object>);
    }
    */

}