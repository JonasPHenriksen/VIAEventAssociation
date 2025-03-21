using System.Collections.Concurrent;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class FakeEventRepository : IEventRepository
{
    private readonly ConcurrentDictionary<EventId, VeaEvent> _events = new();

    public Task<VeaEvent?> GetByIdAsync(EventId id)
    {
        _events.TryGetValue(id, out var veaEvent);
        return Task.FromResult(veaEvent);
    }

    public Task AddAsync(VeaEvent veaEvent)
    {
        _events[veaEvent.Id] = veaEvent;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(VeaEvent veaEvent)
    {
        _events[veaEvent.Id] = veaEvent;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EventId id)
    {
        _events.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
