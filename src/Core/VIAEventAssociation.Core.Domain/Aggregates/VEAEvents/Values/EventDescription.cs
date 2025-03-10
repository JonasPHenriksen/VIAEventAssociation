using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public record EventDescription(string Value)
    {
        public static OperationResult<EventDescription> Create(string description)
        {
            if (description.Length > 250)
                return OperationResult<EventDescription>.Failure("InvalidDescriptionLength", "Description must be 250 characters or fewer.");
            
            return OperationResult<EventDescription>.Success(new EventDescription(description));
        }
    }
}