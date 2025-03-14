using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Services
{
    public interface IEventService
    {
        OperationResult<Unit> PublishEvent(VeaEvent veaEvent);
        OperationResult<Unit> UpdateEventDescription(VeaEvent veaEvent, EventDescription newDescription);
        OperationResult<Unit> UpdateEventTitle(VeaEvent veaEvent, EventTitle newTitle);
        OperationResult<Unit> UpdateEventTimeRange(VeaEvent veaEvent, EventTimeRange newTimeRange);
        OperationResult<Unit> MakeEventPublic(VeaEvent veaEvent);
        OperationResult<Unit> MakeEventPrivate(VeaEvent veaEvent);
        OperationResult<Unit> SetEventMaxGuests(VeaEvent veaEvent, int maxGuests);
        OperationResult<Unit> ReadyEvent(VeaEvent veaEvent);
        OperationResult<Unit> ActivateEvent(VeaEvent veaEvent);
        OperationResult<Unit> ParticipateInEvent(VeaEvent veaEvent, GuestId guestId);
        OperationResult<Unit> CancelParticipationInEvent(VeaEvent veaEvent, GuestId guestId);
        OperationResult<Unit> InviteGuestToEvent(VeaEvent veaEvent, GuestId guestId);
        OperationResult<Unit> AcceptInvitationToEvent(VeaEvent veaEvent, GuestId guestId);
        OperationResult<Unit> DeclineInvitationToEvent(VeaEvent veaEvent, GuestId guestId);
    }
}