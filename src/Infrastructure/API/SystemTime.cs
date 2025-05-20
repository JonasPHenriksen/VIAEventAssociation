using VIAEventAssociation.Core.Domain.Contracts;

namespace VIAEventAssociation.Core.Domain.Services;

public class SystemTime : ISystemTime
    {
        public DateTime Now => DateTime.Now;
    }
