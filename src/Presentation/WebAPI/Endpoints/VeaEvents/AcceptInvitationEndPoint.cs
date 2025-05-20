using Application.Common.CommandDispatcher;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace WebAPI.Endpoints.VeaEvents;

public class AcceptInvitationEndPoint()
    : EndPointBase
        .Command.WithRequest<AcceptInvitationRequest>
        .AndResults<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>
{
    [HttpPost("events/{EventId}/guests/{GuestId}/accept")]
    public override async Task<Results<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>>
        HandleAsync(AcceptInvitationRequest request, ICommandDispatcher dispatcher)
    {
        var cmdResult = AcceptInvitationCommand.Create(request.EventId, request.GuestId);
        if (!cmdResult.IsSuccess)
        {
            return TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(cmdResult.Errors));
        }

        OperationResult<Unit> result = await dispatcher.DispatchAsync<AcceptInvitationCommand, Unit>(cmdResult.Value);

        return result.Match<Results<NoContent, BadRequest<OperationResult<IEnumerable<Error>>>>>(
            onSuccess: _ => TypedResults.NoContent(),
            onFailure: errors => TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(errors))
        );
    }
}

public record AcceptInvitationRequest(
    string EventId,
    string GuestId);