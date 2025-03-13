using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public record EventTimeRange(DateTime Start, DateTime End)
    {
        private static readonly TimeSpan RestrictedStartTime = new TimeSpan(1, 1, 0); // 01:01 AM
        private static readonly TimeSpan RestrictedEndTime = new TimeSpan(7, 59, 0);   // 07:59 AM
        
        public static OperationResult<EventTimeRange> Create(DateTime start, DateTime end)
        {
            if (DateTime.Now > end || DateTime.Now > start)
                return OperationResult<EventTimeRange>.Failure("InvalidDuration","Events cannot be started in the past.");
            
            var duration = end - start;
            
            if (duration.TotalSeconds < 0)
            {
                if (start.Date != end.Date)
                {
                    return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Start date must be before end date.");
                }
                return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Start time must be before end time.");
            }
            
            if (duration.TotalMinutes < 60)
                return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Event duration must be at least 1 hour.");

            if (start >= end)
                return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Start time must be before end time.");

            if (duration.TotalHours > 10)
                return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Event duration cannot exceed 10 hours.");
            
            // Check if the event starts or ends within the restricted time range (01:01 AM to 07:59 AM)
            if (IsTimeInRestrictedRange(start.TimeOfDay) || IsTimeInRestrictedRange(end.TimeOfDay) || DoesEventSpanRestrictedRange(start, end))
            {
                return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Event cannot occur between 01:01 AM and 07:59 AM.");
            }

            return OperationResult<EventTimeRange>.Success(new EventTimeRange(start, end));
        }

        private static bool IsTimeInRestrictedRange(TimeSpan time)
        {
            return time >= RestrictedStartTime && time <= RestrictedEndTime;
        }

        private static bool DoesEventSpanRestrictedRange(DateTime start, DateTime end)
        {
            // Check if the event spans across the restricted time range (01:01 AM to 07:59 AM)
            var startTime = start.TimeOfDay;
            var endTime = end.TimeOfDay;

            // If the event spans across midnight (i.e. starts one day and ends the next)
            if (start.Date != end.Date)
            {
                // Check if it overlaps the restricted time range
                return (startTime < RestrictedStartTime && endTime > RestrictedEndTime);
            }

            return false;
        }
    }
}
