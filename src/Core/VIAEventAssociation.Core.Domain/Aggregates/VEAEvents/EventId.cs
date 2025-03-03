using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public record EventId(Guid Value)
    {
        public static EventId New() => new(Guid.NewGuid());
        
        public static EventId FromString(string id)
        {
            if (Guid.TryParse(id, out var guid))
                return new EventId(guid);

            throw new ArgumentException("Invalid GUID format.", nameof(id));
        }
    }

    public record EventTitle(string Value)
    {
        public static OperationResult<EventTitle> Create(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return OperationResult<EventTitle>.Failure("InvalidTitle", "Title cannot be empty or null.");

            if (title.Length < 3 || title.Length > 75)
                return OperationResult<EventTitle>.Failure("InvalidTitleLength", "Title must be between 3 and 75 characters.");

            return OperationResult<EventTitle>.Success(new EventTitle(title));
        }
    }

    public record EventDescription(string Value)
    {
        public static OperationResult<EventDescription> Create(string description)
        {
            if (description.Length > 250)
                return OperationResult<EventDescription>.Failure("InvalidDescriptionLength", "Description must be 250 characters or fewer.");
            
            return OperationResult<EventDescription>.Success(new EventDescription(description));
        }
    }
    
    public record EventTimeRange(DateTime Start, DateTime End)
    {
        public static OperationResult<EventTimeRange> Create(DateTime start, DateTime end)
        {
            // Ensure start time is before end time
            if (start >= end)
                return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Start time must be before end time.");

            // Ensure duration is at least 1 hour
            var duration = end - start;
            if (duration.TotalHours < 1)
                return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Event duration must be at least 1 hour.");

            // Ensure duration is no more than 10 hours
            if (duration.TotalHours > 10)
                return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Event duration cannot exceed 10 hours.");

            // Ensure start time is after 08:00
            if (start.TimeOfDay < TimeSpan.FromHours(8))
                return OperationResult<EventTimeRange>.Failure("InvalidStartTime", "Event cannot start before 08:00.");

            // Ensure end time is before 01:00 (next day)
            if (end.TimeOfDay > TimeSpan.FromHours(1) && start.TimeOfDay < TimeSpan.FromHours(1))
                return OperationResult<EventTimeRange>.Failure("InvalidEndTime", "Event cannot end after 01:00 AM on the same day.");

            // Ensure event does not span the invalid time range (01:00 to 08:00)
            if (start.TimeOfDay >= TimeSpan.FromHours(1) && end.TimeOfDay <= TimeSpan.FromHours(8))
                return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Event cannot span the time between 01:00 and 08:00.");

            return OperationResult<EventTimeRange>.Success(new EventTimeRange(start, end));
        }
    }

    public enum EventStatus
    {
        Draft,
        Published,
        Ready,
        Cancelled,
        Active
    }

    public enum EventVisibility
    {
        Private,
        Public
    }
}