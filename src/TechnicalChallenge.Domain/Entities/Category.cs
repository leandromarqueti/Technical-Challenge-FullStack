using TechnicalChallenge.Domain.Common;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Entities;

public class Category : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    //EF Core
    private Category() { }

    public Category(string name)
    {
        SetName(name);
    }

    public void Update(string name)
    {
        SetName(name);
        UpdateTimestamp();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome da categoria é obrigatório.");

        if (name.Length > 200)
            throw new DomainException("O nome da categoria deve ter no máximo 200 caracteres.");

        Name = name.Trim();
    }
}
