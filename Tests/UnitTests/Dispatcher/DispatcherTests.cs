using AppEntry;
using Application.Common.CommandDispatcher;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace UnitTests;

public class DispatcherTests
{
    [Fact]
    public async Task DispatchAsync_NoHandlerRegistered_ThrowsException()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new Dispatcher(serviceProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync<string, string>("test"));
    }

    [Fact]
    public async Task DispatchAsync_IncorrectHandlerRegistered_ThrowsException()
    {
        var services = new ServiceCollection();
        var mockHandler = new Mock<ICommandHandler<int, int>>();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new Dispatcher(serviceProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync<string, string>("test"));
    }

    [Fact]
    public async Task DispatchAsync_CorrectHandlerRegistered_CallsHandleAsync()
    {
        var mockHandler = new Mock<ICommandHandler<string, string>>();
        mockHandler.Setup(h => h.HandleAsync("test")).ReturnsAsync("result");

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new Dispatcher(serviceProvider);

        var result = await dispatcher.DispatchAsync<string, string>("test");

        Assert.Equal("result", result);
        mockHandler.Verify(h => h.HandleAsync("test"), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_MultipleHandlersIncludingCorrect_OnlyCorrectCalled()
    {
        var correctMock = new Mock<ICommandHandler<string, string>>();
        correctMock.Setup(h => h.HandleAsync("test")).ReturnsAsync("result");

        var incorrectMock = new Mock<ICommandHandler<int, int>>();
        incorrectMock.Setup(h => h.HandleAsync(It.IsAny<int>())).ReturnsAsync(0);

        var services = new ServiceCollection();
        services.AddSingleton(correctMock.Object);
        services.AddSingleton(incorrectMock.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new Dispatcher(serviceProvider);

        var result = await dispatcher.DispatchAsync<string, string>("test");

        Assert.Equal("result", result);
        correctMock.Verify(h => h.HandleAsync("test"), Times.Once);
        incorrectMock.Verify(h => h.HandleAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_MultipleHandlersExcludingCorrect_ThrowsException()
    {
        var services = new ServiceCollection();
        var mockHandler = new Mock<ICommandHandler<int, int>>();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new Dispatcher(serviceProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync<string, string>("test"));
    }

    [Fact]
    public async Task DispatchAsync_CorrectHandlerCalledExactlyOnce()
    {
        var mockHandler = new Mock<ICommandHandler<string, string>>();
        mockHandler.Setup(h => h.HandleAsync("test")).ReturnsAsync("result");

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new Dispatcher(serviceProvider);

        await dispatcher.DispatchAsync<string, string>("test");

        mockHandler.Verify(h => h.HandleAsync("test"), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_CorrectHandlerNotCalledWhenNotDispatched()
    {
        var mockHandler = new Mock<ICommandHandler<string, string>>();
        mockHandler.Setup(h => h.HandleAsync("test")).ReturnsAsync("result");

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new Dispatcher(serviceProvider);

        // No dispatch call

        mockHandler.Verify(h => h.HandleAsync(It.IsAny<string>()), Times.Never);
    }
}