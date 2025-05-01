using QueryContracts.Contract;

namespace QueryContracts.Queries;

public class EditingEvents
{
    public record Query() : IQuery.IQuery<Answer>;

    public record Answer(
        List<EventItem> Drafts,
        List<EventItem> Readied,
        List<EventItem> Cancelled);

    public record EventItem(string Id, string Title);
}