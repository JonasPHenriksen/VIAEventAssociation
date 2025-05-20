using Application.Common.CommandDispatcher;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Tools.OperationResult;
using WebAPI.Endpoints.VeaEvents;

namespace WebAPI.Endpoints.Guests;

public class CreateGuestEndpoint()
    : EndPointBase
        .Command.WithRequest<CreateGuestRequest>
        .AndResults<Created<CreateGuestResponse>, BadRequest<OperationResult<IEnumerable<Error>>>>
{
    [HttpPost("guests/create")]
    public override async Task<Results<Created<CreateGuestResponse>, BadRequest<OperationResult<IEnumerable<Error>>>>>
        HandleAsync(CreateGuestRequest request, ICommandDispatcher dispatcher)
    {
        OperationResult<CreateGuestCommand> cmdResult = CreateGuestCommand.Create(request.Email, request.FirstName, request.LastName, request.ProfilePictureUrl);
        if (!cmdResult.IsSuccess)
        {
            return TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(cmdResult.Errors));
        }

        OperationResult<Guest> result = await dispatcher.DispatchAsync<CreateGuestCommand, OperationResult<Guest>>(cmdResult.Value);

        return result.Match<Results<Created<CreateGuestResponse>, BadRequest<OperationResult<IEnumerable<Error>>>>>(
            onSuccess: guest => TypedResults.Created($"/guests/{guest.Id}", new CreateGuestResponse(guest.GuestId.Value.ToString())),
            onFailure: errors => TypedResults.BadRequest(OperationResult<IEnumerable<Error>>.Failure(errors))
        );
    }
}

public record CreateGuestRequest(
    [FromBody] string Email,
    [FromBody] string FirstName,
    [FromBody] string LastName,
    [FromBody] string? ProfilePictureUrl
);

public record CreateGuestResponse(string Id);