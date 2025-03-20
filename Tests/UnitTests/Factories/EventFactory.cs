using System.Reflection;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class EventFactory
{
    private VeaEvent _event;

    public EventFactory()
    {
        _event = VeaEvent.Create().Value; // Initialize with default event creation values
    }

    public static EventFactory Init()
    {
        return new EventFactory();
    }

    public EventFactory WithStatus(EventStatus status)
    {
        _event.Status = status;
        return this;
    }
    
    public EventFactory WithTitle(EventTitle title)
    {
        _event.Title = title;
        return this;
    }
    
    public EventFactory WithDescription(EventDescription description)
    {
        _event.Description = description;
        return this;
    }


    public EventFactory WithTimeRange(DateTime start, DateTime end)
    {
        var mockSystemTime = new MockTime.SystemTime();
        _event.TimeRange = new EventTimeRange(start, end, mockSystemTime);
        return this;
    }

    public EventFactory WithMaxGuests(int maxGuests)
    {
        _event.MaxGuests = maxGuests;
        return this;
    }

    public EventFactory WithVisibility(EventVisibility visibility)
    {
        _event.Visibility = visibility;
        return this;
    }

    public EventFactory AddInvitation(GuestId guestId)
    {
        var result = _event.InviteGuest(guestId);
        if (!result.IsSuccess)
            throw new InvalidOperationException($"Failed to add invitation: {result.Errors.FirstOrDefault()?.Message}");
        return this;
    }
    public VeaEvent Build()
    {
        return _event;
    }
}