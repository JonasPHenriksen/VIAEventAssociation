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
        Assert.Single(result.Events);
        Assert.Equal("Event 1", result.Events.First().Title);
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
    }
    
    [Fact]
    public async Task QueryHandler_MostActiveMembers_Success()
    {
        //Arrange
        await using var context = Seeds.SetupReadContext(); 
        var _handler = new MostActiveMembersQueryHandler(context,new MockTime.SystemTime());
        Seeds.Seed(context);
        var query = new MostActiveMembers.Query(1,40);
        
       
        var result = await _handler.HandleAsync(query);
        
        Assert.NotNull(result);
    }
}