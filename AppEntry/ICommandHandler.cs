namespace AppEntry;

public interface ICommandHandler<TCommand>
{ 
    Task HandleAsync(TCommand command);
}

