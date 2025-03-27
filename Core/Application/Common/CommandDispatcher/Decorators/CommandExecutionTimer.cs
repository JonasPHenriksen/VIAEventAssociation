using System.Diagnostics;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace Application.Common.CommandDispatcher;

public class CommandExecutionTimer : ICommandDispatcher
{
    private readonly ICommandDispatcher next;

    public CommandExecutionTimer(ICommandDispatcher next) => this.next = next;

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        TResult result = await next.DispatchAsync<TCommand, TResult>(command);

        TimeSpan elapsedTime = stopwatch.Elapsed;
        // do something with the time, log it, store in DB, whatever

        return result; 
    }
}