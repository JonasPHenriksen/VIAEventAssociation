namespace VIAEventAssociation.Core.Domain.Common.Contracts;

public interface IGuestRepository
{
    Task<Guest?> GetByEmailAsync(string email);
    Task SaveAsync(Guest guest);
}
