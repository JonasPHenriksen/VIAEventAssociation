using AppEntry;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateGuestCommandHandlerTest
{
    [Fact]
    public async Task CanCreateGuestWithValidData()
    {
        // Arrange
        var newGuest = Guest.Create(new Email("330943@via.dk"), new Name("Jane"), new Name("Smith"), new Uri("https://example.com/profile.jpg")).Value;
        FakeGuestRepository repo = CreateHandler(newGuest, out var handler);
        var command = CreateGuestCommand.Create("330943@via.dk", "Jane", "Smith", "https://example.com/profile.jpg");

        // Act
        var handledResult = await handler.HandleAsync(command.Value);

        // Assert
        Assert.True(handledResult.IsSuccess);
        await repo.AddAsync(handledResult.Value);
        var guest = await repo.GetByEmailAsync(new Email("330943@via.dk"));
        Assert.Equal(handledResult.Value, guest);
    }
    
    [Fact]
    public async Task CannotCreateGuestIfEmailAlreadyExists()
    {
        // Arrange
        var newGuest = Guest.Create(new Email("330943@via.dk"), new Name("Jane"), new Name("Smith"), new Uri("https://example.com/profile.jpg")).Value;
        FakeGuestRepository repo = CreateHandler(newGuest, out var handler);
        var command = CreateGuestCommand.Create("330943@via.dk", "Jane", "Smith", "https://example.com/profile.jpg");
        
        // Act
        var handledResult = await handler.HandleAsync(command.Value);
        var handledResult2 = await handler.HandleAsync(command.Value);
        await repo.AddAsync(handledResult.Value);
        await repo.AddAsync(handledResult.Value);

        Assert.False(handledResult2.IsSuccess);
        Assert.Equal("EmailAlreadyExists", handledResult2.Errors[0].Code);
    }
    
    private static FakeGuestRepository CreateHandler(Guest guest, out ICommandHandler<CreateGuestCommand, OperationResult<Guest>> handler) //TODO maybe change unit return type
    {
        FakeGuestRepository repo = new FakeGuestRepository();
        repo.Aggregate = guest;

        IUnitOfWork uow = new FakeUnitOfWork();

        handler = new CreateGuestCommandHandler(repo, uow);
        return repo;
    }
}