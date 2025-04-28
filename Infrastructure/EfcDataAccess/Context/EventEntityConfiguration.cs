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
        builder.HasKey(e => e.EventId);

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
        
        builder.OwnsOne(typeof(EventTimeRange), "TimeRange", b =>
        {
            b.Property(typeof(DateTime), "Start").HasColumnName("StartTime");
            b.Property(typeof(DateTime), "End").HasColumnName("EndTime");
            b.Ignore("SystemTime");
        });
        
        builder.OwnsMany<GuestId>("Participants", participantBuilder =>
        {
            participantBuilder.Property<int>("Id").ValueGeneratedOnAdd();
            participantBuilder.HasKey("Id");
            participantBuilder.Property(x => x.Value);
        });
        
        builder.OwnsMany(typeof(Invitation), "_invitations", valueBuilder =>
        {
            valueBuilder.WithOwner().HasForeignKey("VeaEventId");

            valueBuilder.OwnsOne(typeof(InvitationStatus), "Status", statusBuilder =>
            {
                statusBuilder.Property(typeof(InvitationStatus.InvitationStatusEnum), "Value")
                    .HasConversion<string>()
                    .HasColumnName("Status");
            });

            valueBuilder.HasKey("VeaEventId", "GuestId");

            valueBuilder.HasOne(typeof(Guest))
                .WithMany()
                .HasForeignKey("GuestId");
        });
    }
}