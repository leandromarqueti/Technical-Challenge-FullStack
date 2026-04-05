using TechnicalChallenge.Domain.Entities;

namespace TechnicalChallenge.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Category?> GetByDescriptionAsync(string description, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> HasTransactionsAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken = default);
}
