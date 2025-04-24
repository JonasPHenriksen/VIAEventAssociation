using DCAExamples.Core.Domain.Common.Repositories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace VIAEventAssociation.Core.Domain.Contracts
{
    public interface IEventRepository : IGenericRepository<VeaEvent, EventId>
    {
    }

}