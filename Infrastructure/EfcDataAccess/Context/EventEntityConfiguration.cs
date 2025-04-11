using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class EventEntityConfiguration : IEntityTypeConfiguration<VeaEvent>
{
    public void Configure(EntityTypeBuilder<VeaEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(m => m.EventId)
            .HasConversion(
                mId => mId.Value,
                dbValue => EventId.FromGuid(dbValue).Value
            );
        
        builder
            .Property("Status");

        builder
            .Property("Description") 
            .HasConversion(
                new ValueConverter<EventDescription, string>(
                    v => v.Get, 
                    v => new EventDescription(v) 
                )
            );
    }
}