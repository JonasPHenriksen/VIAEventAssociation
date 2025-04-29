using QueryContracts.Contract;

namespace QueryContracts.Queries;

public class UpcomingEvents
{
    public record Query() : IQuery.IQuery<UpcomingEvents.Answer>; //TODO add pagination

    public record Answer(List<UpcomingEvent> UpcomingEvents);
    
    public record UpcomingEvent(string title, string firstPartOfDescription, int NumberOfGuests, string Visibility, string startTime, string endTime);
}