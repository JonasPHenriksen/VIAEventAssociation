namespace VIAEventAssociation.Core.Domain.Contracts;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}