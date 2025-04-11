using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;

public class GuestEntityConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasKey(e => e.GuestId);
        
        builder
            .Property(m => m.GuestId)
            .HasConversion(
                mId => mId.Value,
                dbValue => GuestId.FromGuid(dbValue).Value
            );
    }
}