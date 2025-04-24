using DCAExamples.Core.Domain.Common.Repositories;
using EfcDataAccess.Context;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Contracts;

namespace EfcDataAccess.Repositories;

public class EventRepositoryEfc (MyDbContext context) : RepositoryBaseEfc<VeaEvent, EventId> (context), IEventRepository
{ 
    
}