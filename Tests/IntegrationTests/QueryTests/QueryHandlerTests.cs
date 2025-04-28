using EfcDataAccess.Context;
using EfcQueries;
using QueryContracts.Queries;

namespace IntegrationTests.QueryTests;

public class QueryHandlerTests
{
    [Fact]
    public async Task DispatchAsync_ReturnsUpcomingEvents()
    {
        await using var context = Seeds.SetupReadContext(); 
        var _handler = new UpcomingEventsQueryHandler(context);

        // Act
        Seeds.Seed(context);

        var query = new UpcomingEvents.Query();

        var result = await _handler.HandleAsync(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.UpcomingEvents);
        Assert.Single(result.UpcomingEvents);
        Assert.Equal("Event 1", result.UpcomingEvents.First().title);
    }
}