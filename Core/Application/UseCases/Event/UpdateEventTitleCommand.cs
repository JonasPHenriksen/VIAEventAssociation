using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

public class UpdateEventTitleCommand
{
    public EventTitle NewTitle { get; private set; }
    public EventId NewEventId { get; private set; }

    public static OperationResult<UpdateEventTitleCommand> Create(string eventId, string newTitle)
    {
        var errors = new List<Error>();
        var titleResult = EventTitle.Create(newTitle);
        if (!titleResult.IsSuccess)
        {
            errors.AddRange(titleResult.Errors);
        }
        
        var idResult = EventId.FromString(eventId);
        if (!idResult.IsSuccess)
        {
            errors.AddRange(idResult.Errors);
        }
        
        if (errors.Any())
            return OperationResult<UpdateEventTitleCommand>.Failure(errors);
        
        return OperationResult<UpdateEventTitleCommand>.Success(new UpdateEventTitleCommand
        {
            NewEventId = new EventId(new Guid(eventId)),
            NewTitle = titleResult.Value
        });
    }

    private UpdateEventTitleCommand() { }
}