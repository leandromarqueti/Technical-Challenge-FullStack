using TechnicalChallenge.Domain.Common;
using TechnicalChallenge.Domain.Validators;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Entities;

public class Person : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public DateTime BirthDate { get; private set; }

    public string Document { get; private set; } = string.Empty;

    public bool IsMinor => BirthDate.AddYears(18) > DateTime.UtcNow;

    public Guid UserId { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    //o ef core precisa de um construtor sem parâmetros
    protected Person() { }

    public Person(string name, DateTime birthDate, string document, Guid userId)
    {
        ValidateAndSet(name, birthDate, document, userId);
    }

    public void Update(string name, DateTime birthDate, string document, Guid userId)
    {
        ValidateAndSet(name, birthDate, document, userId);
        UpdateTimestamp();
    }

    private void ValidateAndSet(string name, DateTime birthDate, string document, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(ResourceErrorMessages.NAME_EMPTY);

        if (name.Length > 200)
            throw new DomainException("O nome deve ter no máximo 200 caracteres.");

        if (birthDate > DateTime.UtcNow)
            throw new DomainException("A data de nascimento não pode ser no futuro.");

        var cleanDoc = DocumentValidator.CleanDocument(document);

        if (!DocumentValidator.IsValid(cleanDoc))
            throw new DomainException(ResourceErrorMessages.DOCUMENT_INVALID);

        if (userId == Guid.Empty)
            throw new DomainException("O usuário é obrigatório.");

        Name = name.Trim();
        BirthDate = birthDate;
        Document = cleanDoc;
        UserId = userId;
    }
}
