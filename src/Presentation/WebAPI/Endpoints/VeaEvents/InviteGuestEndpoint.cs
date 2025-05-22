using Application.Common.CommandDispatcher;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace WebAPI.Endpoints.VeaEvents;

public class InviteGuestEndpoint()
    : EndPointBase
        .Command.WithRequest<InviteGuestRequest>
        .AndResults<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>
{
    [HttpPost("events/{EventId}/guests/{GuestId}/invite")]
    public override async Task<Results<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>>
        HandleAsync([FromRoute] InviteGuestRequest request, ICommandDispatcher dispatcher)
    {
        var cmdResult = InviteGuestCommand.Create(request.EventId, request.GuestId);
        if (!cmdResult.IsSuccess)
        {
            return TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(cmdResult.Errors));
        }

        OperationResult<Unit> result = await dispatcher.DispatchAsync<InviteGuestCommand, OperationResult<Unit>>(cmdResult.Value);

        return result.Match<Results<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>>(
            onSuccess: _ => TypedResults.NoContent(),
            onFailure: errors => TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(errors))
        );
    }
}

public record InviteGuestRequest(
    [FromRoute] string EventId,
    [FromRoute] string GuestId);