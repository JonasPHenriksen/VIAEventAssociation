using EfcDataAccess.Context;
namespace IntegrationTests;

public class DummyDataTests
{
    [Fact]
    public async Task Seed_ShouldPopulateDatabaseWithExpectedData()
    {
        // Arrange
        await using var context = Seeds.SetupReadContext();

        // Act
        Seeds.Seed(context);

        // Assert
        Assert.NotEmpty(context.Guests);
        Assert.Equal(50, context.Guests.Count()); // Adjust this based on your GuestSeedFactory
        Assert.NotEmpty(context.Events);
        // Add more assertions as needed to verify the seeded data
    }
}