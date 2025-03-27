using Application.Common.CommandDispatcher;
using Moq;

namespace UnitTests;

public class LoggingDispatcherDecoratorTests
{
    [Fact]
    public async Task DispatchAsync_LogsCommandDispatch()
    {
        // Arrange
        var mockInnerDispatcher = new Mock<ICommandDispatcher>();
        var mockLogger = new Mock<ICustomLogger>();
        var command = new TestCommand();
        var expectedResult = new TestResult();

        mockInnerDispatcher
            .Setup(d => d.DispatchAsync<TestCommand, TestResult>(command))
            .ReturnsAsync(expectedResult);

        var decorator = new LoggingDispatcherDecorator(mockInnerDispatcher.Object, mockLogger.Object);

        // Act
        var result = await decorator.DispatchAsync<TestCommand, TestResult>(command);

        // Assert
        Assert.Equal(expectedResult, result);
        mockLogger.Verify(l => l.LogInformation("Dispatching command: {CommandType}", typeof(TestCommand).Name), Times.Once);
        mockLogger.Verify(l => l.LogInformation("Command {CommandType} dispatched successfully", typeof(TestCommand).Name), Times.Once);
        mockInnerDispatcher.Verify(d => d.DispatchAsync<TestCommand, TestResult>(command), Times.Once);
    }
}

public class TestCommand { }
public class TestResult { }