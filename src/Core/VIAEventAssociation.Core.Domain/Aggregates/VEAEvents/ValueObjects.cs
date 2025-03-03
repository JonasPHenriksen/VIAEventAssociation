using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public record ValueObjects(Guid Value)
    {
        public static ValueObjects New() => new(Guid.NewGuid());
        
        public static ValueObjects FromString(string id)
        {
            if (Guid.TryParse(id, out var guid))
                return new ValueObjects(guid);

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
            if (string.IsNullOrWhiteSpace(description))
                return OperationResult<EventDescription>.Failure("InvalidDescription", "Description cannot be empty.");

            return OperationResult<EventDescription>.Success(new EventDescription(description));
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