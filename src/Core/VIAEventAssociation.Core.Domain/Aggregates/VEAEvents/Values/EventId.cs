using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public record EventId(Guid Value)
    {
        public static EventId New() => new(Guid.NewGuid());
        
        public static OperationResult<EventId> FromString(string id)
        {
            if (Guid.TryParse(id, out var guid))
                return OperationResult<EventId>.Success(new EventId(guid));

            return OperationResult<EventId>.Failure("guid", "Invalid guid format.");
        }
        
        public static OperationResult<EventId> FromGuid(Guid id)
        {
            return OperationResult<EventId>.Success(new EventId(id));
        }
    }
}