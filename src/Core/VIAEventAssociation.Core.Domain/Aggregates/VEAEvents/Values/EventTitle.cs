using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
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
}