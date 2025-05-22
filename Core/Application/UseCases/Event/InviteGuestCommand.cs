using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

public class InviteGuestCommand
{
    public EventId newEventId { get; private set; }
    public GuestId GuestId { get; private set; }

    public static OperationResult<InviteGuestCommand> Create(string eventId, string guestId)
    {
        
        var errors = new List<Error>();
        
        var idResult = EventId.FromString(eventId);
        if (!idResult.IsSuccess)
        {
            errors.AddRange(idResult.Errors);
        }

        if (errors.Any())
        {
            return OperationResult<InviteGuestCommand>.Failure(errors);
        }
        
        var idResult2 = GuestId.FromString(guestId);
        if (!idResult.IsSuccess)
        {
            errors.AddRange(idResult.Errors);
        }

        if (errors.Any())
        {
            return OperationResult<InviteGuestCommand>.Failure(errors);
        }

        return OperationResult<InviteGuestCommand>.Success(new InviteGuestCommand
        {
            newEventId = idResult.Value,
            GuestId = idResult2.Value
        });
    }

    private InviteGuestCommand() { }
}