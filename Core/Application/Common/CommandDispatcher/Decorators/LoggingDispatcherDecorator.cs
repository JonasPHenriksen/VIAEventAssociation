namespace Application.Common.CommandDispatcher;

public class LoggingDispatcherDecorator : ICommandDispatcher
{
    private readonly ICommandDispatcher _innerDispatcher;
    private readonly ICustomLogger _logger;

    public LoggingDispatcherDecorator(ICommandDispatcher innerDispatcher, ICustomLogger logger)
    {
        _innerDispatcher = innerDispatcher;
        _logger = logger;
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command)
    {
        _logger.LogInformation("Dispatching command: {CommandType}", typeof(TCommand).Name);
        TResult result = await _innerDispatcher.DispatchAsync<TCommand, TResult>(command);
        _logger.LogInformation("Command {CommandType} dispatched successfully", typeof(TCommand).Name);
        return result;
    }
}
public interface ICustomLogger
{
    void LogInformation(string message, params object[] args);
}

public class ConsoleLogger : ICustomLogger
{
    public void LogInformation(string message, params object[] args)
    {
        Console.WriteLine(message, args);
    }
}