using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class EventEntityConfiguration : IEntityTypeConfiguration<VeaEvent>
{
    public void Configure(EntityTypeBuilder<VeaEvent> builder)
    {
        builder.HasKey(e => e.Id);
    }
}