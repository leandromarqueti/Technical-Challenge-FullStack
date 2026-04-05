using TechnicalChallenge.Domain.Common;
using TechnicalChallenge.Domain.Validators;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Entities;

public class Person : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public DateTime BirthDate { get; private set; }

    public string Document { get; private set; } = string.Empty;

    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    //EF Core precisa de um construtor sem parâmetros
    private Person() { }

    public Person(string name, DateTime birthDate, string document)
    {
        ValidateAndSet(name, birthDate, document);
    }

    public void Update(string name, DateTime birthDate, string document)
    {
        ValidateAndSet(name, birthDate, document);
        UpdateTimestamp();
    }

    private void ValidateAndSet(string name, DateTime birthDate, string document)
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

        Name = name.Trim();
        BirthDate = birthDate;
        Document = cleanDoc;
    }
}
