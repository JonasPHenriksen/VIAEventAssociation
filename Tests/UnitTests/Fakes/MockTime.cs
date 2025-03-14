using VIAEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class MockTime
{
    public class SystemTime : ISystemTime
    {
        public DateTime Now => new DateTime(2020,1,1,10,5,24);
    }
    
}