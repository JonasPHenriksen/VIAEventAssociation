namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public record EventId(Guid Value)
    {
        public static EventId New() => new(Guid.NewGuid());
        
        public static EventId FromString(string id)
        {
            if (Guid.TryParse(id, out var guid))
                return new EventId(guid);

            throw new ArgumentException("Invalid GUID format.", nameof(id));
        }
    }
}