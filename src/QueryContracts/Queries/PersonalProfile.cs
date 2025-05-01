using QueryContracts.Contract;

namespace QueryContracts.Queries;

public class PersonalProfile
{
    public record Query(string GuestId) : IQuery.IQuery<Answer>;
    public record Answer(
        string Name,
        string Email,
        string ProfileImageUrl,
        int UpcomingEventCount,
        int PendingInvitations,
        List<UpcomingEvent> UpcomingEvents,
        List<PastEvent> PastEvents);

    public record UpcomingEvent(string Title, int AttendeeCount, string DateTime);
    public record PastEvent(string Title);
}