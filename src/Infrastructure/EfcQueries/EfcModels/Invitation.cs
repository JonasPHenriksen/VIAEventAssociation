using System;
using System.Collections.Generic;

namespace EfcDataAccess;

public partial class Invitation
{
    public string GuestId { get; set; } = null!;

    public string VeaEventId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Guest Guest { get; set; } = null!;

    public virtual Event VeaEvent { get; set; } = null!;
}
