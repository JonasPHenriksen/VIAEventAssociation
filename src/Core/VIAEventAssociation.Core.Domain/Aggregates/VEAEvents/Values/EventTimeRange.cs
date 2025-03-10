using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public record EventTimeRange(DateTime Start, DateTime End)
    {
        public static OperationResult<EventTimeRange> Create(DateTime start, DateTime end)
        {
            if (start >= end)
                return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Start time must be before end time.");

            var duration = end - start;
            if (duration.TotalHours < 1)
                return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Event duration must be at least 1 hour.");

            if (duration.TotalHours > 10)
                return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Event duration cannot exceed 10 hours.");

            if (start.TimeOfDay < TimeSpan.FromHours(8))
                return OperationResult<EventTimeRange>.Failure("InvalidStartTime", "Event cannot start before 08:00.");

            if (end.TimeOfDay > TimeSpan.FromHours(1) && start.TimeOfDay < TimeSpan.FromHours(1))
                return OperationResult<EventTimeRange>.Failure("InvalidEndTime", "Event cannot end after 01:00 AM on the same day.");

            if (start.TimeOfDay >= TimeSpan.FromHours(1) && end.TimeOfDay <= TimeSpan.FromHours(8))
                return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Event cannot span the time between 01:00 and 08:00.");

            return OperationResult<EventTimeRange>.Success(new EventTimeRange(start, end));
        }
    }
}