using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public class EventDescription : ValueObject
    {
        public string Get { get; }

        public EventDescription(string description) => Get = description;
        


        public static OperationResult<EventDescription> Create(string description)
        {
            if (description.Length > 250)
            {
                return OperationResult<EventDescription>.Failure("InvalidDescription", "Description must be 250 characters or fewer.");
            }
            return OperationResult<EventDescription>.Success(new EventDescription(description));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Get;
        }
    }
}