using TechnicalChallenge.Domain.Entities;

namespace TechnicalChallenge.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByDescriptionAsync(string description, CancellationToken cancellationToken = default);

    Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
