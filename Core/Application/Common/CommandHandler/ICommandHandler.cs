using System.Windows.Input;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace AppEntry;

public interface ICommandHandler<TCommand, TResult>
{ 
        Task<TResult> HandleAsync(TCommand command);        
}

