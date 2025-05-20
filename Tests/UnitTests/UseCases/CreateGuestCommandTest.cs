using AppEntry;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateGuestCommandTest
{
    [Fact]
    public async Task CanCreateGuestCommandWithValidData()
    {
        var command = CreateGuestCommand.Create("330943@via.dk", "Jane", "Smith", "https://example.com/profile.jpg");

        Assert.True(command.IsSuccess);
        Assert.NotNull(command.Value);
        Assert.Equal("330943@via.dk", command.Value.Email.Value);
        Assert.Equal("Jane", command.Value.FirstName.Value);
        Assert.Equal("Smith", command.Value.LastName.Value);
        Assert.Equal("https://example.com/profile.jpg", command.Value.ProfilePictureUrl.AbsoluteUri);
    }
}