using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Services
{
    public class EventService : IEventService
    {
        public OperationResult<Unit> PublishEvent(VeaEvent veaEvent)
        {
            return veaEvent.Publish();
        }

        public OperationResult<Unit> UpdateEventDescription(VeaEvent veaEvent, EventDescription newDescription)
        {
            return veaEvent.UpdateDescription(newDescription);
        }

        public OperationResult<Unit> UpdateEventTitle(VeaEvent veaEvent, EventTitle newTitle)
        {
            return veaEvent.UpdateTitle(newTitle);
        }

        public OperationResult<Unit> UpdateEventTimeRange(VeaEvent veaEvent, EventTimeRange newTimeRange)
        {
            return veaEvent.UpdateTimeRange(newTimeRange);
        }

        public OperationResult<Unit> MakeEventPublic(VeaEvent veaEvent)
        {
            return veaEvent.MakePublic();
        }

        public OperationResult<Unit> MakeEventPrivate(VeaEvent veaEvent)
        {
            return veaEvent.MakePrivate();
        }

        public OperationResult<Unit> SetEventMaxGuests(VeaEvent veaEvent, int maxGuests)
        {
            return veaEvent.SetMaxGuests(maxGuests);
        }

        public OperationResult<Unit> ReadyEvent(VeaEvent veaEvent)
        {
            return veaEvent.ReadyEvent();
        }

        public OperationResult<Unit> ActivateEvent(VeaEvent veaEvent)
        {
            return veaEvent.ActivateEvent();
        }

        public OperationResult<Unit> ParticipateInEvent(VeaEvent veaEvent, GuestId guestId)
        {
            return veaEvent.Participate(guestId);
        }

        public OperationResult<Unit> CancelParticipationInEvent(VeaEvent veaEvent, GuestId guestId)
        {
            return veaEvent.CancelParticipation(guestId);
        }

        public OperationResult<Unit> InviteGuestToEvent(VeaEvent veaEvent, GuestId guestId)
        {
            return veaEvent.InviteGuest(guestId);
        }

        public OperationResult<Unit> AcceptInvitationToEvent(VeaEvent veaEvent, GuestId guestId)
        {
            return veaEvent.AcceptInvitation(guestId);
        }

        public OperationResult<Unit> DeclineInvitationToEvent(VeaEvent veaEvent, GuestId guestId)
        {
            return veaEvent.DeclineInvitation(guestId);
        }
    }
}