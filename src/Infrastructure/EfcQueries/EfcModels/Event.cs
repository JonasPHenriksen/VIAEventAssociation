using System;
using System.Collections.Generic;

namespace EfcDataAccess;

public partial class Event
{
    public string EventId { get; set; } = null!;

    public int Status { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Visibility { get; set; }

    public int MaxGuests { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public virtual ICollection<GuestId> GuestIds { get; set; } = new List<GuestId>();

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
