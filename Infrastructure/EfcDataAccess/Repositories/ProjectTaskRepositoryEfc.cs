using DCAExamples.Core.Domain.Common.Repositories;
using EfcMappingExamples;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace EfcDataAccess.Repositories;

public class ProjectTaskRepositoryEfc :
    RepositoryBase<VeaEvent>, IVeaEventRepository
{
    public ProjectTaskRepositoryEfc(MyDbContext context)
        : base(context)
    {
    }

    public override Task<VeaEvent> GetAsync(Guid id)
    {
        // load the aggregate, include all dependent entities.
        return null;
    }
}