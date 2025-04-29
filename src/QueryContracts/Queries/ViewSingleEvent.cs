using QueryContracts.Contract;

namespace QueryContracts.Queries;

public class ViewSingleEvent
{
    public record Query(string EventId) : IQuery.IQuery<Answer>; //TODO maybe change Guid

    public record Answer(
        string Title,
        string Description,
        string StartTime,
        string EndTime,
        string Visibility,
        int GuestCount,
        int MaxGuests,
        List<Guest> Guests);

    public record Guest(string Id, string Name, string ProfileImageUrl);
}
