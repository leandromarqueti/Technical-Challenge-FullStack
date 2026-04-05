using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;

namespace TechnicalChallenge.Domain.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetFilteredAsync(
        string? description,
        DateTime? startDate,
        DateTime? endDate,
        Guid? categoryId,
        Guid? personId,
        TransactionType? type,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalExpensesAsync(CancellationToken cancellationToken = default);

    Task<Transaction?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);
}
