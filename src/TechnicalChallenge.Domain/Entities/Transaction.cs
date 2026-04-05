using TechnicalChallenge.Domain.Common;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Entities;

public class Transaction : AggregateRoot
{
    public string Description { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public DateTime Date { get; private set; }

    public TransactionType Type { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }
    public Guid UserId { get; private set; }

    //necessário pro ef core
    protected Transaction() { }

    public Transaction(
        string description,
        decimal amount,
        DateTime date,
        TransactionType type,
        Guid categoryId,
        Guid personId,
        Guid userId)
    {
        ValidateAndSet(description, amount, date, type, categoryId, personId, userId);
    }

    public void Update(
        string description,
        decimal amount,
        DateTime date,
        TransactionType type,
        Guid categoryId,
        Guid personId,
        Guid userId)
    {
        ValidateAndSet(description, amount, date, type, categoryId, personId, userId);
        UpdateTimestamp();
    }

    private void ValidateAndSet(
        string description,
        decimal amount,
        DateTime date,
        TransactionType type,
        Guid categoryId,
        Guid personId,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição da transação é obrigatória.");

        if (description.Length > 400)
            throw new DomainException("A descrição deve ter no máximo 400 caracteres.");

        if (amount <= 0)
            throw new DomainException("O valor da transação deve ser maior que zero.");

        if (!Enum.IsDefined(typeof(TransactionType), type))
            throw new DomainException("Tipo de transação inválido.");

        if (categoryId == Guid.Empty)
            throw new DomainException("A categoria é obrigatória.");

        if (personId == Guid.Empty)
            throw new DomainException("A pessoa é obrigatória.");

        if (userId == Guid.Empty)
            throw new DomainException("O usuário é obrigatório.");

        Description = description.Trim();
        Amount = amount;
        Date = date;
        Type = type;
        CategoryId = categoryId;
        PersonId = personId;
        UserId = userId;
    }
}
