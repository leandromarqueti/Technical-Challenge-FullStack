using TechnicalChallenge.Domain.Common;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Entities;

public class Category : AggregateRoot
{
    public string Description { get; private set; } = string.Empty;

    public Guid UserId { get; private set; }

    public CategoryPurpose Purpose { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    //Para o EF Core
    protected Category() { }

    public Category(string description, Guid userId, CategoryPurpose purpose = CategoryPurpose.Both)
    {
        SetDescription(description);
        Purpose = purpose;
        UserId = userId;
    }

    public void Update(string description, Guid userId, CategoryPurpose purpose)
    {
        SetDescription(description);
        Purpose = purpose;
        UserId = userId;
        UpdateTimestamp();
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição da categoria é obrigatória.");

        if (description.Length > 400)
            throw new DomainException("A descrição da categoria deve ter no máximo 400 caracteres.");

        Description = description.Trim();
    }
}
