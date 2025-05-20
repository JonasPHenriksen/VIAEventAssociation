using Microsoft.AspNetCore.Mvc;
using ObjectMapper;
using QueryContracts.Queries;
using QueryContracts.QueryDispatching;

namespace WebAPI.Endpoints.VeaEvents;

public class ViewSingleEventEndpoint(IMapper mapper) //TODO Change the mapper injection?
    : EndPointBase
        .Query.WithRequest<ViewSingleEventRequest>
        .WithResponse<ViewSingleEventResponse>
{
    [HttpGet("events/{Id}")]
    public override async Task<ActionResult<ViewSingleEventResponse>> HandleAsync([FromRoute] ViewSingleEventRequest request, IQueryDispatcher dispatcher)
    {
        SingleEvent.Query query = mapper.Map<SingleEvent.Query>(request);
        SingleEvent.Answer answer = await dispatcher.DispatchAsync(query);
        ViewSingleEventResponse response = mapper.Map<ViewSingleEventResponse>(answer);
        return Ok(response);
    }
}

public record ViewSingleEventRequest([FromRoute] string EventId);

public record ViewSingleEventResponse(
    string Title,
    string Description,
    string StartTime,
    string EndTime,
    string Visibility,
    int GuestCount,
    int MaxGuests,
    List<Guest> Guests
);

public record Guest(
    string Id,
    string Name,
    string ProfileImageUrl
);

