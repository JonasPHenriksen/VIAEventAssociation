using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Domain.Services;
using VIAEventAssociation.Core.Tools.OperationResult;

public class UpdateEventTimeRangeCommand
{
    public EventId NewEventId { get; private set; }
    public EventTimeRange NewTimeRange { get; private set; }

    private static ISystemTime _systemTime;
    
    public UpdateEventTimeRangeCommand(ISystemTime systemTime)
    {
        _systemTime = systemTime;
    }
    public static OperationResult<UpdateEventTimeRangeCommand> Create(string eventId, string dateTimeFrom, string dateTimeTo)
    {
        var errors = new List<Error>();

        if (!DateTime.TryParse(dateTimeFrom, out var parsedDateTimeFrom))
        {
            errors.Add(new Error("InvalidDateTimeFrom", "The 'dateTimeFrom' string is not a valid DateTime."));
        }

        if (!DateTime.TryParse(dateTimeTo, out var parsedDateTimeTo))
        {
            errors.Add(new Error("InvalidDateTimeTo", "The 'dateTimeTo' string is not a valid DateTime."));
        }
        
        var timeRangeResult = EventTimeRange.Create(parsedDateTimeFrom, parsedDateTimeTo, _systemTime);

        if (!timeRangeResult.IsSuccess)
        {
            errors.AddRange(timeRangeResult.Errors);
        }
        
        var idResult = EventId.FromString(eventId);
        if (!idResult.IsSuccess)
        {
            errors.AddRange(idResult.Errors);
        }

        if (errors.Any())
        {
            return OperationResult<UpdateEventTimeRangeCommand>.Failure(errors);
        }
        
        return OperationResult<UpdateEventTimeRangeCommand>.Success(new UpdateEventTimeRangeCommand
        {
            NewEventId = idResult.Value,
            NewTimeRange = timeRangeResult.Value
        });
    }


    private UpdateEventTimeRangeCommand() { }
}