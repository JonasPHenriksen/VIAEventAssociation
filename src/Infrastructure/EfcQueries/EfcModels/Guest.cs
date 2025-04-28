using System;
using System.Collections.Generic;

namespace EfcDataAccess;

public partial class Guest
{
    public string GuestId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ProfilePictureUrl { get; set; } = null!;

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
