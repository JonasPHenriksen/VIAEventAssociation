using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

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
        
        builder
            .Property("Email") 
            .HasConversion(
                new ValueConverter<Email, string>(
                    v => v.Value, 
                    v => Email.Create(v).Value 
                )
            );
        
        builder
            .Property("FirstName") 
            .HasConversion(
                new ValueConverter<Name, string>(
                    v => v.Value, 
                    v => Name.Create(v).Value 
                )
            );
        
        builder
            .Property("LastName") 
            .HasConversion(
                new ValueConverter<Name, string>(
                    v => v.Value, 
                    v => Name.Create(v).Value 
                )
            );
        
        builder
            .Property("ProfilePictureUrl") 
            .HasConversion(
                new ValueConverter<Uri, string>(
                    v => v.AbsoluteUri, 
                    v => new Uri(v)
                )
            );
    }
}