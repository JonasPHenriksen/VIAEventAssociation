namespace VIAEventAssociation.Core.Domain.Aggregates.Guests
{
    public record GuestId(Guid Value)
    {
        public static GuestId New() => new(Guid.NewGuid());
    }
}