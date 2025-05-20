using EfcDataAccess.Context;
using EfcQueries;
using QueryContracts.Queries;
using UnitTests.Fakes;

namespace IntegrationTests.QueryTests;

public class QueryHandlerTests
{
    [Fact]
    public async Task QueryHandler_ReturnsUpcomingEvents_Success()
    {
        //Arrange
        await using var context = Seeds.SetupReadContext(); 
        var _handler = new UpcomingEventsQueryHandler(context, new MockTime.SystemTime());
        Seeds.Seed(context);
        var query = new UpcomingEvents.Query(2,3);
        
        // Act
        var result = await _handler.HandleAsync(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Events);
        Assert.Equal(3,result.Events.Count);
        Assert.Equal("Orienteering", result.Events.First().Title);
    }
    
    [Fact]
    public async Task QueryHandler_ReturnsSingleEvent_Success()
    {
        //Arrange
        await using var context = Seeds.SetupReadContext(); 
        var _handler = new SingleEventQueryHandler(context);
        Seeds.Seed(context);
        var query = new SingleEvent.Query("40d73623-48d4-4862-b116-7ee7cdfda22f");
        
        // Act
        var result = await _handler.HandleAsync(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Friday Bar The Second", result.Title);
        Assert.Equal("Free watery beer the first hour!", result.Description);
        Assert.Equal("2024-03-08 15:00", result.StartTime);
        Assert.Equal("2024-03-08 21:00", result.EndTime);
        Assert.Equal("1", result.Visibility);
        Assert.Equal(21, result.GuestCount);
        Assert.Equal(50, result.MaxGuests);
        Assert.Equal(21, result.Guests.Count);
    }
    
    [Fact]
    public async Task QueryHandler_ReturnsMostActiveMembers_Success()
    {
        // Arrange
        await using var context = Seeds.SetupReadContext();
        var handler = new MostActiveMembersQueryHandler(context, new MockTime.SystemTime());
        Seeds.Seed(context);
        var query = new MostActiveMembers.Query(1, 5);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Members);
        Assert.Equal("Aaliyah Armstrong", result.Members.First().Name);
        Assert.True(result.Members.First().TotalCount > 0);
        Assert.True(result.Members.First().LastSixMonthsCount >= 0);
        Assert.True(result.Members.First().TotalCount >= 0);
    }
    
    [Fact]
    public async Task QueryHandler_ReturnsPersonalProfile_Success()
    {
        // Arrange
        await using var context = Seeds.SetupReadContext();
        var handler = new PersonalProfileQueryHandler(context, new MockTime.SystemTime());
        var guestId = "53645f68-409e-4cab-9028-dc799a27dc61";
        Seeds.Seed(context);
        var query = new PersonalProfile.Query(guestId);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Abel Adkins", result.Name);
        Assert.True(result.UpcomingEventCount >= 0);
        Assert.True(result.PendingInvitations >= 0);
    }
    
    [Fact]
    public async Task QueryHandler_Throws_WhenGuestNotFound()
    {
        // Arrange
        await using var context = Seeds.SetupReadContext();
        var handler = new PersonalProfileQueryHandler(context, new MockTime.SystemTime());
        var query = new PersonalProfile.Query("nonexistent");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await handler.HandleAsync(query));
    }
    
    [Fact]
    public async Task QueryHandler_ReturnsEditingEvents_Success()
    {
        // Arrange
        await using var context = Seeds.SetupReadContext();
        var handler = new EditingEventsQueryHandler(context);
        Seeds.Seed(context); 
        var query = new EditingEvents.Query();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Drafts);
        Assert.NotEmpty(result.Readied);
        Assert.NotEmpty(result.Cancelled);
    }
}