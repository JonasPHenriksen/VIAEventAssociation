using AppEntry;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.CommandDispatcher;

public class Dispatcher : ICommandDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    // I can use GetRequiredService because extension method, see using above
    public Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command)
    {
        ICommandHandler<TCommand, TResult> service = serviceProvider
                            .GetRequiredService<ICommandHandler<TCommand, TResult>>();

        return service.HandleAsync(command);
    }
}