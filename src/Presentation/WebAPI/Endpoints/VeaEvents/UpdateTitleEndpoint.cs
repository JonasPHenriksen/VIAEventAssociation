using Application.Common.CommandDispatcher;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace WebAPI.Endpoints.VeaEvents;

public class UpdateTitleEndpoint()
    : EndPointBase
        .Command.WithRequest<UpdateEventTitleRequest>
        .AndResults<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>
{
        [HttpPost("events/{Id}/update-title")]
        public override async Task<Results<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>>
            HandleAsync(UpdateEventTitleRequest request, ICommandDispatcher dispatcher)
        {
        OperationResult<UpdateEventTitleCommand> cmdResult = UpdateEventTitleCommand.Create(request.Id, request.RequestBody.Title);
        if (!cmdResult.IsSuccess)
        {
            var errors = new List<Error>();
            return TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(errors));
        }
        OperationResult<Unit> result = await dispatcher.DispatchAsync<UpdateEventTitleCommand, Unit>(cmdResult.Value);        
        return result.Match<Results<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>>(
            onSuccess: (none) => TypedResults.NoContent(),
            onFailure: errors => TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(errors)) 
        );
    }
}
public record UpdateEventTitleRequest(
    [FromRoute] string Id,
    [FromBody] UpdateEventTitleRequest.Body RequestBody)
{
    public record Body(string Title);
};