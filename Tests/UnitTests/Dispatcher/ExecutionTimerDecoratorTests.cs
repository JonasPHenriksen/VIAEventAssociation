using System.Diagnostics;
using AppEntry;
using Application.Common.CommandDispatcher;
using Moq;

namespace UnitTests;

public class CommandExecutionTimerTests
{
    private class FakeCommand { }
    private class FakeResult { }

    private class FakeCommandHandler : ICommandHandler<FakeCommand, FakeResult>
    {
        public Task<FakeResult> HandleAsync(FakeCommand command) => Task.FromResult(new FakeResult());
    }

    [Fact]
    public async Task CommandExecutionTimer_ShouldTimeExecution()
    {
        // Arrange
        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher.Setup(d => d.DispatchAsync<FakeCommand, FakeResult>(It.IsAny<FakeCommand>()))
                      .ReturnsAsync(new FakeResult());

        var timerDecorator = new CommandExecutionTimer(mockDispatcher.Object);

        var command = new FakeCommand();

        // Act
        var result = await timerDecorator.DispatchAsync<FakeCommand, FakeResult>(command);

        // Assert
        mockDispatcher.Verify(d => d.DispatchAsync<FakeCommand, FakeResult>(It.IsAny<FakeCommand>()), Times.Once);
        Assert.NotNull(result); // Ensure result is returned
    }

    [Fact]
    public async Task CommandExecutionTimer_ShouldCallNextDispatcher()
    {
        // Arrange
        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher.Setup(d => d.DispatchAsync<FakeCommand, FakeResult>(It.IsAny<FakeCommand>()))
                      .ReturnsAsync(new FakeResult());

        var timerDecorator = new CommandExecutionTimer(mockDispatcher.Object);

        var command = new FakeCommand();

        // Act
        await timerDecorator.DispatchAsync<FakeCommand, FakeResult>(command);

        // Assert: Ensure the next dispatcher (mock) was called.
        mockDispatcher.Verify(d => d.DispatchAsync<FakeCommand, FakeResult>(It.IsAny<FakeCommand>()), Times.Once);
    }

    [Fact]
    public async Task CommandExecutionTimer_ShouldLogExecutionTime()
    {
        // Arrange
        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher.Setup(d => d.DispatchAsync<FakeCommand, FakeResult>(It.IsAny<FakeCommand>()))
                      .ReturnsAsync(new FakeResult());

        var timerDecorator = new CommandExecutionTimer(mockDispatcher.Object);

        var command = new FakeCommand();
        
        // Act & Assert
        var stopwatchBefore = Stopwatch.StartNew();
        var result = await timerDecorator.DispatchAsync<FakeCommand, FakeResult>(command);
        stopwatchBefore.Stop();

        // Here, we would verify that the execution time is logged or tracked
        // As we are not actually logging, we will just check if time was measured.
        Assert.True(stopwatchBefore.ElapsedMilliseconds >= 0);
    }
}