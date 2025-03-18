using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Core.Domain.Common.Bases;
//TODO the bases could be used a lot more to also reduce code duplication
public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id)
    {
    }
    
    protected AggregateRoot() { }
    
}