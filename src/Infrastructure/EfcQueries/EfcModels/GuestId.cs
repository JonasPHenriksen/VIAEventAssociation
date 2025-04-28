using System;
using System.Collections.Generic;

namespace EfcDataAccess;

public partial class GuestId
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public string VeaEventEventId { get; set; } = null!;

    public virtual Event VeaEventEvent { get; set; } = null!;
}
