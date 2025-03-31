using System.Collections.Concurrent;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Contracts;

namespace UnitTests.Fakes;

public class FakeGuestRepository : IGuestRepository
{
    private readonly ConcurrentDictionary<GuestId, Guest> _guestsById = new();
    private readonly ConcurrentDictionary<Email, Guest> _guestsByEmail = new();
    
    public Guest Aggregate { get; set; }

    public Task<Guest?> GetByEmailAsync(Email email)
    {
        _guestsByEmail.TryGetValue(email, out var guest);
        return Task.FromResult(guest);
    }

    public Task<Guest?> GetByIdAsync(GuestId id)
    {
        _guestsById.TryGetValue(id, out var guest);
        return Task.FromResult(guest);
    }

    public Task SaveAsync(Guest guest)
    {
        _guestsById[guest.GuestId] = guest;
        _guestsByEmail[guest.Email] = guest;
        return Task.CompletedTask;
    }

    public Task AddAsync(Guest guest)
    {
        _guestsById[guest.GuestId] = guest;
        _guestsByEmail[guest.Email] = guest;
        return Task.CompletedTask;
    }
}