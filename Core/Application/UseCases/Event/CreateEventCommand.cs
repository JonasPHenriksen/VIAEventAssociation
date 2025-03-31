using System;
using System.Collections.Generic;
using System.Linq;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateEventCommand
{
    public static OperationResult<CreateEventCommand> Create()
    {
        return OperationResult<CreateEventCommand>.Success(new CreateEventCommand{});
    }

    private CreateEventCommand() { }
}