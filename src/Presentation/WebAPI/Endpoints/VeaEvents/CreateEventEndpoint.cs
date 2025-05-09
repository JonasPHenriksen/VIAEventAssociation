using Application.Common.CommandDispatcher;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace WebAPI.Endpoints.VeaEvents;

public class CreateEventEndPoint()
    : EndPointBase
        .Command.WithRequest<CreateEventRequest>
        .WithResponse<CreateEventResponse>
{
    [HttpPost("events/create")]
    public override async Task<ActionResult<CreateEventResponse>>
        HandleAsync(CreateEventRequest request, ICommandDispatcher dispatcher)
    {
        OperationResult<CreateEventCommand> cmdResult = CreateEventCommand.Create();
        if (!cmdResult.IsSuccess)
        {
            var errors = new List<Error>();
            return BadRequest(OperationResult<IEnumerable<Error>>.Failure(errors));
        }

        OperationResult<VeaEvent> result = await dispatcher.DispatchAsync<CreateEventCommand, OperationResult<VeaEvent>>(cmdResult.Value);
        return result.Match<ActionResult<CreateEventResponse>>(
            onSuccess: (none) => 
            {
                var response = new CreateEventResponse(result.Value.EventId.ToString());
                return Ok(response);
            },
            onFailure: errors => BadRequest(OperationResult<IEnumerable<Error>>.Failure(errors))
        );
    }
}

public record CreateEventRequest();

public record CreateEventResponse(string Id);

