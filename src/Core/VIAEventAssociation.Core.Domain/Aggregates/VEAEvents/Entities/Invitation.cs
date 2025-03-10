using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public class Invitation
    {
        public EventId EventId { get; }
        public GuestId GuestId { get; }
        public InvitationStatus Status { get; private set; }

        private Invitation(EventId eventId, GuestId guestId, InvitationStatus status)
        {
            EventId = eventId;
            GuestId = guestId;
            Status = status;
        }

        public static OperationResult<Invitation> Create(EventId eventId, GuestId guestId)
        {
            if (eventId == null || guestId == null)
                return OperationResult<Invitation>.Failure("InvalidData", "Event ID and Guest ID cannot be null.");

            return OperationResult<Invitation>.Success(new Invitation(eventId, guestId, InvitationStatus.Pending()));
        }

        public OperationResult<Unit> Accept()
        {
            if (!Status.IsPending)
                return OperationResult<Unit>.Failure("InvalidStatus", "Only pending invitations can be accepted.");

            Status = InvitationStatus.Accepted();
            return OperationResult<Unit>.Success();
        }

        public OperationResult<Unit> Decline()
        {
            if (Status.IsDeclined)
                return OperationResult<Unit>.Failure("InvalidStatus", "Already declined invitations can be declined.");

            Status = InvitationStatus.Declined();
            return OperationResult<Unit>.Success();
        }
    }

}