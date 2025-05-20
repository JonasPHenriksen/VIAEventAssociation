using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents.Values;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public class Invitation : Entity
    {
        private EventId EventId { get; }
        internal GuestId GuestId { get; }
        internal InvitationStatus Status { get; private set; }
        private Invitation() {} //EFC
        private Invitation(EventId eventId, GuestId guestId, InvitationStatus status)
        {
            EventId = eventId;
            GuestId = guestId;
            Status = status;
        }
        
        public static OperationResult<Invitation> Create(EventId eventId, GuestId guestId)
        {
            var invitation = new Invitation(eventId, guestId, InvitationStatus.Pending());
            return OperationResult<Invitation>.Success(invitation);
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