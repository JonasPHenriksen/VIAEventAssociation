using AppEntry;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.CommandDispatcher;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    public Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command)
    {
        ICommandHandler<TCommand, TResult> service = serviceProvider
                            .GetRequiredService<ICommandHandler<TCommand, TResult>>();

        //TODO ERROR IS HERE!
        
        return service.HandleAsync(command);
    }
}