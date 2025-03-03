using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public class VeaEvent
    {
        public EventId Id { get; }
        public EventStatus Status { get; private set; }
        public EventTitle Title { get; private set; }
        public EventDescription Description { get; private set; }
        public EventVisibility Visibility { get; private set; }
        public int MaxGuests { get; private set; }
        public EventTimeRange? TimeRange { get; private set; }
        public HashSet<GuestId> Participants { get; private set; } = new();
        public HashSet<GuestId> InvitedGuests { get; private set; } = new();
        public List<GuestId> DeclinedGuests { get; private set; }
        private VeaEvent(EventId id, EventTitle title, EventDescription description, EventStatus status, EventVisibility visibility, int maxGuests)
        {
            Id = id;
            Title = title;
            Description = description;
            Status = status;
            Visibility = visibility;
            MaxGuests = maxGuests;
        }

        public static OperationResult<VeaEvent> Create()
        {
            var titleResult = EventTitle.Create("Working Title");
            var descriptionResult = EventDescription.Create("");

            if (!titleResult.IsSuccess)
                return OperationResult<VeaEvent>.Failure(titleResult.Errors);

            if (!descriptionResult.IsSuccess)
                return OperationResult<VeaEvent>.Failure(descriptionResult.Errors);

            return OperationResult<VeaEvent>.Success(new VeaEvent(
                EventId.New(),
                titleResult.Value,
                descriptionResult.Value,
                EventStatus.Draft,
                EventVisibility.Private,
                5
            ));
        }

        // Method to change the event's status to "Published"
        public OperationResult<Unit> Publish()
        {
            if (Status != EventStatus.Draft)
                return OperationResult<Unit>.Failure("InvalidStatus", "Event cannot be published unless it's in draft status.");

            Status = EventStatus.Published;
            return OperationResult<Unit>.Success();
        }

        // Method to update the event's description
        public OperationResult<Unit> UpdateDescription(string newDescription)
        {
            // Check if the event is in a valid state for updating the description
            if (Status != EventStatus.Draft && Status != EventStatus.Ready)
                return OperationResult<Unit>.Failure("InvalidStatus", "Description can only be updated when the event is in Draft or Ready status.");

            // Validate the new description
            var descriptionResult = EventDescription.Create(newDescription);
            if (!descriptionResult.IsSuccess)
                return OperationResult<Unit>.Failure(descriptionResult.Errors);

            // Update the description
            Description = descriptionResult.Value;

            // If the event was in Ready status, revert it to Draft
            if (Status == EventStatus.Ready)
                Status = EventStatus.Draft;

            return OperationResult<Unit>.Success();
        }
        // Method to update the event's title
        public OperationResult<Unit> UpdateTitle(string newTitle)
        {
            // Check if the event is in a valid state for updating the title
            if (Status != EventStatus.Draft && Status != EventStatus.Ready)
                return OperationResult<Unit>.Failure("InvalidStatus", "Title can only be updated when the event is in Draft or Ready status.");

            // Validate the new title
            var titleResult = EventTitle.Create(newTitle);
            if (!titleResult.IsSuccess)
                return OperationResult<Unit>.Failure(titleResult.Errors);

            // Update the title
            Title = titleResult.Value;

            // If the event was in Ready status, revert it to Draft
            if (Status == EventStatus.Ready)
                Status = EventStatus.Draft;

            return OperationResult<Unit>.Success();
        }

        public void SetStatus(EventStatus status)
        {
            Status = status;
        }
        
        public OperationResult<Unit> UpdateTimeRange(DateTime start, DateTime end)
        {
            // Check if the event is in a valid state for updating the time range
            if (Status != EventStatus.Draft && Status != EventStatus.Ready)
                return OperationResult<Unit>.Failure("InvalidStatus", "Time range can only be updated when the event is in Draft or Ready status.");

            // Validate the new time range
            var timeRangeResult = EventTimeRange.Create(start, end);
            if (!timeRangeResult.IsSuccess)
                return OperationResult<Unit>.Failure(timeRangeResult.Errors);

            // Ensure the start time is in the future
            if (start < DateTime.Now)
                return OperationResult<Unit>.Failure("InvalidStartTime", "Event cannot start in the past.");

            // Update the time range
            TimeRange = timeRangeResult.Value;

            // If the event was in Ready status, revert it to Draft
            if (Status == EventStatus.Ready)
                Status = EventStatus.Draft;

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> MakePublic()
        {
            // Check if the event is in a valid state for making it public
            if (Status == EventStatus.Cancelled)
                return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be made public.");

            // Update the visibility to Public
            Visibility = EventVisibility.Public;

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> MakePrivate()
        {
            // Check if the event is in a valid state for making it private
            if (Status == EventStatus.Active)
                return OperationResult<Unit>.Failure("InvalidStatus", "An active event cannot be made private.");

            if (Status == EventStatus.Cancelled)
                return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be modified.");

            // If the event is already private, do nothing
            if (Visibility == EventVisibility.Private)
                return OperationResult<Unit>.Success();

            // Update the visibility to Private
            Visibility = EventVisibility.Private;

            // If the event was in Ready status, revert it to Draft
            if (Status == EventStatus.Ready)
                Status = EventStatus.Draft;

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> SetMaxGuests(int maxGuests)
        {
            // Check if the event is in a valid state for updating the maximum number of guests
            if (Status == EventStatus.Cancelled)
                return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be modified.");

            // Validate the new maximum number of guests
            if (maxGuests < 5)
                return OperationResult<Unit>.Failure("InvalidMaxGuests", "The maximum number of guests cannot be less than 5.");

            if (maxGuests > 50)
                return OperationResult<Unit>.Failure("InvalidMaxGuests", "The maximum number of guests cannot exceed 50.");

            // If the event is in Active status, ensure the new value is not less than the current value
            if (Status == EventStatus.Active && maxGuests < MaxGuests)
                return OperationResult<Unit>.Failure("InvalidMaxGuests", "The maximum number of guests cannot be reduced for an active event.");

            // Update the maximum number of guests
            MaxGuests = maxGuests;

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> ReadyEvent()
        {
            // Check if the event is in a valid state for readying
            if (Status == EventStatus.Cancelled)
                return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be readied.");

            if (Status != EventStatus.Draft)
                return OperationResult<Unit>.Failure("InvalidStatus", "Only events in Draft status can be readied.");

            // Validate that all required fields are set and valid
            if (Title.Value == "Working Title" || string.IsNullOrWhiteSpace(Title.Value)) // Default title
                return OperationResult<Unit>.Failure("InvalidTitle", "The title must be changed from the default value.");

            if (string.IsNullOrWhiteSpace(Description.Value) && Description.Value == "")
                return OperationResult<Unit>.Failure("InvalidDescription", "The description must be set.");

            if (TimeRange == null)
                return OperationResult<Unit>.Failure("InvalidTimeRange", "The event times must be set.");

            if (Visibility == EventVisibility.Private && MaxGuests < 5)
                return OperationResult<Unit>.Failure("InvalidMaxGuests", "The maximum number of guests must be between 5 and 50.");

            if (TimeRange.Start < DateTime.Now)
                return OperationResult<Unit>.Failure("InvalidTimeRange", "An event in the past cannot be readied.");

            // Transition the event to Ready status
            Status = EventStatus.Ready;

            return OperationResult<Unit>.Success();
        }
        public OperationResult<Unit> ActivateEvent()
        {
            // Check if the event is in a valid state for activation
            if (Status == EventStatus.Cancelled)
                return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be activated.");

            // If the event is in Draft status, first make it Ready
            if (Status == EventStatus.Draft)
            {
                var readyResult = ReadyEvent();
                if (!readyResult.IsSuccess)
                    return readyResult; // Return the failure message from ReadyEvent
            }

            // Transition the event to Active status
            Status = EventStatus.Active;

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> Participate(GuestId guestId)
        {
            // Check if the event is in a valid state for participation
            if (Status != EventStatus.Active)
                return OperationResult<Unit>.Failure("InvalidStatus", "Only active events can be joined.");

            if (Visibility != EventVisibility.Public)
                return OperationResult<Unit>.Failure("InvalidVisibility", "Only public events can be participated.");

            if (TimeRange == null || TimeRange.Start < DateTime.Now)
                return OperationResult<Unit>.Failure("InvalidTimeRange", "Only future events can be participated.");

            if (Participants.Count >= MaxGuests)
                return OperationResult<Unit>.Failure("NoMoreRoom", "There is no more room for additional guests.");

            if (Participants.Contains(guestId))
                return OperationResult<Unit>.Failure("DuplicateParticipation", "A guest cannot participate more than once in an event.");

            // Add the guest to the participants list
            Participants.Add(guestId);

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> CancelParticipation(GuestId guestId)
        {
            // Check if the event has already started
            if (TimeRange != null && TimeRange.Start < DateTime.Now)
                return OperationResult<Unit>.Failure("InvalidTimeRange", "You cannot cancel your participation in past or ongoing events.");

            // Remove the guest from the participants list if they are participating
            if (Participants.Contains(guestId))
            {
                Participants.Remove(guestId);
                return OperationResult<Unit>.Success();
            }

            // If the guest is not participating, do nothing
            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> InviteGuest(GuestId guestId)
        {
            // Check if the event is in a valid state for inviting guests
            if (Status != EventStatus.Ready && Status != EventStatus.Active)
                return OperationResult<Unit>.Failure("InvalidStatus", "Guests can only be invited to events that are ready or active.");

            // Check if the event is full
            if (Participants.Count + InvitedGuests.Count >= MaxGuests)
                return OperationResult<Unit>.Failure("NoMoreRoom", "You cannot invite guests if the event is full.");

            // Check if the guest is already invited
            if (InvitedGuests.Contains(guestId))
                return OperationResult<Unit>.Failure("DuplicateInvitation", "The guest is already invited.");

            // Check if the guest is already participating
            if (Participants.Contains(guestId))
                return OperationResult<Unit>.Failure("AlreadyParticipating", "The guest is already participating.");

            // Add the guest to the invited guests list
            InvitedGuests.Add(guestId);

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> AcceptInvitation(GuestId guestId)
        {
            // Check if the event is cancelled
            if (Status == EventStatus.Cancelled)
                return OperationResult<Unit>.Failure("InvalidStatus", "Cancelled events cannot be joined.");

            // Check if the event is ready (cannot accept invitations for ready events)
            if (Status == EventStatus.Ready)
                return OperationResult<Unit>.Failure("InvalidStatus", "You cannot join a ready event.");

            // Check if the event is in the past
            if (TimeRange?.Start < DateTime.Now)
                return OperationResult<Unit>.Failure("InvalidTimeRange", "You cannot join a past event.");
            
            if (TimeRange == null)
                return OperationResult<Unit>.Failure("InvalidTimeRange", "You cannot join an event without a timeRange set");

            // Check if the guest has a pending invitation
            if (!InvitedGuests.Contains(guestId))
                return OperationResult<Unit>.Failure("InvitationNotFound", "You are not invited to this event.");

            // Check if the event has reached the maximum guest capacity
            if (Participants.Count + InvitedGuests.Count >= MaxGuests)
                return OperationResult<Unit>.Failure("NoMoreRoom", "The event is full, you cannot join.");

            // Accept the invitation
            InvitedGuests.Remove(guestId);  // Remove from invited list
            Participants.Add(guestId);      // Add to participants list

            return OperationResult<Unit>.Success();
        }
        
        public OperationResult<Unit> DeclineInvitation(GuestId guestId)
        {
            // Check if the event is cancelled
            if (Status == EventStatus.Cancelled)
            {
                return OperationResult<Unit>.Failure("CannotDeclineInvitation", "Invitations to cancelled events cannot be declined.");
            }

            // Check if the guest is invited to the event
            if (InvitedGuests.Contains(guestId))
            {
                InvitedGuests.Remove(guestId);
                DeclinedGuests.Add(guestId);
                return OperationResult<Unit>.Success();
            }

            // Check if the guest has already accepted the invitation (i.e., is a participant)
            if (Participants.Contains(guestId))
            {
                Participants.Remove(guestId);
                DeclinedGuests.Add(guestId);
                return OperationResult<Unit>.Success();
            }

            // If the invitation is not found, return failure
            return OperationResult<Unit>.Failure("InvitationNotFound", "The guest is not invited to the event.");
        }


    }
}