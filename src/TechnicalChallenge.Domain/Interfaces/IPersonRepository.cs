using TechnicalChallenge.Domain.Entities;

namespace TechnicalChallenge.Domain.Interfaces;

public interface IPersonRepository : IRepository<Person>
{
    Task<Person?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentAsync(string document, CancellationToken cancellationToken = default);

    Task<bool> HasTransactionsAsync(Guid personId, CancellationToken cancellationToken = default);
}
