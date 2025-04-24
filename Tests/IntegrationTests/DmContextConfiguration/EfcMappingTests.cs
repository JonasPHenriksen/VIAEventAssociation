using Microsoft.EntityFrameworkCore;
using EfcDataAccess.Context;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;


namespace IntegrationTests.DmContextConfiguration;

public class EfcMappingTests
{
    private class TestDbContext : DbContext
    {
        public DbSet<VeaEvent> VeaEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
        }
    }

    [Fact]
    public void Event_Entity_Maps_Correctly()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: "MappingTest")
            .Options;

        var veaEvent = VeaEvent.Create().Value;
        var title = EventTitle.Create("Sample Title").Value;
        var description = EventDescription.Create("Some description").Value;
        var timeRange = EventTimeRange.Create(DateTime.Parse("2023-08-25T19:00:00"),
            DateTime.Parse("2023-08-25T23:59:00"), new MockTime.SystemTime()).Value;

        using (var context = new MyDbContext(options))
        {
            veaEvent.UpdateTitle(title);
            veaEvent.UpdateDescription(description);
            veaEvent.UpdateTimeRange(timeRange);
            veaEvent.SetMaxGuests(10);
            context.Events.Add(veaEvent);
            context.SaveChanges();
        }

        using (var context = new MyDbContext(options))
        {
            var dbEvent = context.Events.AsNoTracking().First();

            Assert.Equal(veaEvent.EventId, dbEvent.EventId);
            Assert.Equal(title.Value, dbEvent.Title.Value);
            Assert.Equal(description.Get, dbEvent.Description.Get);
            Assert.Equal(EventStatus.Draft, dbEvent.Status);
            Assert.Equal(EventVisibility.Private, dbEvent.Visibility);
            Assert.Equal(10, dbEvent.MaxGuests);
            Assert.NotNull(dbEvent.TimeRange);
        }
    }
    
    [Fact]
    public void Guest_Entity_Maps_Correctly()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase("GuestMappingTest")
            .Options;

   
        var email = Email.Create("330943@via.dk").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var profilePic = new Uri("https://example.com/profile.jpg");
        var guest = Guest.Create(email,firstName,lastName,profilePic);

        using (var context = new MyDbContext(options))
        {
            context.Guests.Add(guest.Value);
            context.SaveChanges();
        }

        using (var context = new MyDbContext(options))
        {
            var dbGuest = context.Guests.AsNoTracking().First();

            Assert.Equal(guest.Value.GuestId, dbGuest.GuestId);
            Assert.Equal(email.Value, dbGuest.Email.Value);
            Assert.Equal(firstName.Value, dbGuest.FirstName.Value);
            Assert.Equal(lastName.Value, dbGuest.LastName.Value);
            Assert.Equal(profilePic.AbsoluteUri, dbGuest.ProfilePictureUrl.AbsoluteUri);
        }
    }
}