using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Guests
{
    public record GuestId(Guid Value)
    {
        public static GuestId New() => new(Guid.NewGuid());
        
        public static OperationResult<GuestId> FromString(string id)
        {
            if (Guid.TryParse(id, out var guid))
                return OperationResult<GuestId>.Success(new GuestId(guid));

            return OperationResult<GuestId>.Failure("guid", "Invalid guid format.");
        }
    }
}