using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

public class AcceptInvitationCommand
{
    public EventId EventId { get; private set; }
    public GuestId GuestId { get; private set; }

    public static OperationResult<AcceptInvitationCommand> Create(string eventId, string guestId)
    {
        var errors = new List<Error>();

        var eventIdResult = EventId.FromString(eventId);
        if (!eventIdResult.IsSuccess)
            errors.AddRange(eventIdResult.Errors);

        var guestIdResult = GuestId.FromString(guestId);
        if (!guestIdResult.IsSuccess)
            errors.AddRange(guestIdResult.Errors);

        if (errors.Any())
            return OperationResult<AcceptInvitationCommand>.Failure(errors);

        return OperationResult<AcceptInvitationCommand>.Success(new AcceptInvitationCommand
        {
            EventId = eventIdResult.Value,
            GuestId = guestIdResult.Value
        });
    }

    private AcceptInvitationCommand() { }
}