using EfcDataAccess.Context;
using EfcQueries;
using QueryContracts.Queries;

namespace IntegrationTests.QueryTests;

public class QueryHandlerTests
{
    [Fact]
    public async Task QueryHandler_ReturnsUpcomingEvents_Success()
    {
        //Arrange
        await using var context = Seeds.SetupReadContext(); 
        var _handler = new UpcomingEventsQueryHandler(context);
        Seeds.Seed(context);
        var query = new UpcomingEvents.Query();
        
        // Act
        var result = await _handler.HandleAsync(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.UpcomingEvents);
        Assert.Single(result.UpcomingEvents);
        Assert.Equal("Event 1", result.UpcomingEvents.First().title);
    }
}