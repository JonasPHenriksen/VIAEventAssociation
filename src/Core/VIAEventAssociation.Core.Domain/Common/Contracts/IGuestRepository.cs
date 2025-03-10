namespace VIAEventAssociation.Core.Domain.Contracts;

public interface IGuestRepository
{
    Task<Guest?> GetByEmailAsync(string email);
    Task SaveAsync(Guest guest);
}
