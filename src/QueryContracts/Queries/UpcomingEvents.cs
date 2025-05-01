using QueryContracts.Contract;

namespace QueryContracts.Queries;

public class UpcomingEvents
{
    public record Query(int Page, int PageSize) : IQuery.IQuery<Answer>;
    public record Answer(List<UpcomingEvent> Events);
    public record UpcomingEvent(string Title, string Description, int MaxGuests, int AcceptedCount, string Visibility, string StartTime, string EndTime);
}
