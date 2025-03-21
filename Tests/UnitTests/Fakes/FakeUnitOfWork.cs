using VIAEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}