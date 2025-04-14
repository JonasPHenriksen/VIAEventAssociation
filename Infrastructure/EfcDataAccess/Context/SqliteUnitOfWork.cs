using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;

namespace EfcMappingExamples;

public class SqliteUnitOfWork(DbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync()
        => context.SaveChangesAsync();
}
