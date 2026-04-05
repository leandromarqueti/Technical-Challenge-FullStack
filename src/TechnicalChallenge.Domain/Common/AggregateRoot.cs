namespace TechnicalChallenge.Domain.Common;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }
}
