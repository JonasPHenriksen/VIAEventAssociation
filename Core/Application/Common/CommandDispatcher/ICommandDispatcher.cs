namespace Application.Common.CommandDispatcher;

public interface ICommandDispatcher
{
    public Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command);
}