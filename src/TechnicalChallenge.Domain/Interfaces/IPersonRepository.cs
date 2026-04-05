using TechnicalChallenge.Domain.Entities;

namespace TechnicalChallenge.Domain.Interfaces;

public interface IPersonRepository : IRepository<Person>
{
    Task<Person?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Person>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Person?> GetByDocumentAsync(string document, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentAsync(string document, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> HasTransactionsAsync(Guid personId, Guid userId, CancellationToken cancellationToken = default);
}
