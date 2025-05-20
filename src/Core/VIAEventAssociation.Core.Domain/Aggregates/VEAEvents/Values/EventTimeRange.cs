using System.ComponentModel.DataAnnotations.Schema;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public record EventTimeRange
{
    private static readonly TimeSpan RestrictedStartTime = new TimeSpan(1, 1, 0); // 01:01 AM
    private static readonly TimeSpan RestrictedEndTime = new TimeSpan(7, 59, 0);   // 07:59 AM
    
    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    [NotMapped]
    public ISystemTime SystemTime { get; init; } = null!;

    private EventTimeRange() { }

    public EventTimeRange(DateTime start, DateTime end, ISystemTime systemTime)
    {
        Start = start;
        End = end;
        SystemTime = systemTime;
    }
    public static OperationResult<EventTimeRange> Create(DateTime start, DateTime end, ISystemTime systemTime)
    {
        // Use the systemTime stored within the object for checking times
        if (systemTime.Now > end || systemTime.Now > start)
            return OperationResult<EventTimeRange>.Failure("InvalidDuration", "Events cannot be started in the past.");

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

        if (start.TimeOfDay < new TimeSpan(8, 0, 0))
            return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Start time cannot be before 08:00.");

        if (IsTimeInRestrictedRange(start.TimeOfDay) || IsTimeInRestrictedRange(end.TimeOfDay) || OverlapsRestrictedRange(start, end))
            return OperationResult<EventTimeRange>.Failure("InvalidTimeRange", "Event cannot occur between 01:01 and 07:59.");

        return OperationResult<EventTimeRange>.Success(new EventTimeRange(start, end, systemTime));
    }
    public bool IsEventInPast()
    {
        return SystemTime.Now > End; 
    }
    
    private static bool IsTimeInRestrictedRange(TimeSpan time)
    {
        return time >= RestrictedStartTime && time <= RestrictedEndTime;
    }

    private static bool OverlapsRestrictedRange(DateTime start, DateTime end)
    {
        var current = start;
        while (current < end)
        {
            var time = current.TimeOfDay;
            if (time >= RestrictedStartTime && time <= RestrictedEndTime)
                return true;

            current = current.AddMinutes(1);
        }
        return false;
    }

    public DateTime GetCurrentTime()
    {
        return SystemTime.Now;
    }
}
