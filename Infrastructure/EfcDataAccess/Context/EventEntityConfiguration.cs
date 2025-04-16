using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents.Values;

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
            .Property("Title") 
            .HasConversion(
                new ValueConverter<EventTitle, string>(
                    v => v.Value, 
                    v => EventTitle.Create(v).Value 
                )
            );
        
        builder
            .Property("Description") 
            .HasConversion(
                new ValueConverter<EventDescription, string>(
                    v => v.Get, 
                    v => new EventDescription(v) 
                )
            );
        
        builder
            .Property("Visibility");
        
        builder
            .Property("MaxGuests");
        
        builder
            .Property<DateTime>("StartTime")
            .HasColumnName("StartTime");

        builder
            .Property<DateTime>("EndTime")
            .HasColumnName("EndTime");
        
        /*
        builder.OwnsMany<GuestId>("Participants", valueBuilder =>
        {
            valueBuilder.WithOwner().HasForeignKey("VeaEventId");

            valueBuilder.Property<GuestId>("GuestId")
                .HasConversion(
                    guestId => guestId.Value,
                    guid => new GuestId(guid)
                )
                .HasColumnName("GuestId");

            valueBuilder.HasKey("VeaEventId", "GuestId"); //TODO Find a way to remove value field in db table

            valueBuilder.HasOne<Guest>()
                .WithMany()
                .HasForeignKey("GuestId")
                .HasPrincipalKey(g => g.GuestId);
            
            valueBuilder.ToTable("Participants");
        });
        */
        
        builder.OwnsMany<Invitation>("_invitations", valueBuilder =>
        {
            valueBuilder.WithOwner().HasForeignKey("VeaEventId");
            
            valueBuilder.OwnsOne(x => x.Status, statusBuilder =>
            {
                statusBuilder.Property(s => s.Value)
                    .HasConversion<string>() 
                    .HasColumnName("Status"); 
            });
            
            valueBuilder.HasKey("VeaEventId", "GuestId");
            
            valueBuilder.HasOne<Guest>() 
                .WithMany() 
                .HasForeignKey("GuestId"); 
        });
    }
}