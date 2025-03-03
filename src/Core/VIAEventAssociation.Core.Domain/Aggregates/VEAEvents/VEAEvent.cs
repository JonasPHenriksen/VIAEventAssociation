using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public class VEAEvent
    {
        public ValueObjects Id { get; }
        public EventStatus Status { get; private set; }
        public EventTitle Title { get; private set; }
        public EventDescription Description { get; private set; }
        public EventVisibility Visibility { get; private set; }
        public int MaxGuests { get; private set; }

        // Constructor to create an Event
        private VEAEvent(ValueObjects id, EventTitle title, EventDescription description, EventStatus status, EventVisibility visibility, int maxGuests)
        {
            Id = id;
            Title = title;
            Description = description;
            Status = status;
            Visibility = visibility;
            MaxGuests = maxGuests;
        }

        public static OperationResult<VEAEvent> Create(string title, string description)
        {
            var titleResult = EventTitle.Create(title);
            var descriptionResult = EventDescription.Create(description);

            if (!titleResult.IsSuccess)
                return OperationResult<VEAEvent>.Failure(titleResult.Errors);

            if (!descriptionResult.IsSuccess)
                return OperationResult<VEAEvent>.Failure(descriptionResult.Errors);

            return OperationResult<VEAEvent>.Success(new VEAEvent(
                ValueObjects.New(),
                titleResult.Value,
                descriptionResult.Value,
                EventStatus.Draft,
                EventVisibility.Private,
                5 // Default max guests
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
            var descriptionResult = EventDescription.Create(newDescription);
            if (!descriptionResult.IsSuccess)
            {
                return OperationResult<Unit>.Failure(descriptionResult.Errors);
            }

            Description = descriptionResult.Value;
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
    }
}