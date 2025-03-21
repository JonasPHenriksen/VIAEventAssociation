using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace VIAEventAssociation.Core.Domain.Contracts
{
    public interface IEventRepository
    {
        Task<VeaEvent?> GetByIdAsync(EventId id);
        Task AddAsync(VeaEvent veaEvent);
        Task UpdateAsync(VeaEvent veaEvent);
        Task DeleteAsync(EventId id);
    }
}