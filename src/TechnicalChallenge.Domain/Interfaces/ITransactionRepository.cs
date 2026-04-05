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
        Guid userId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalRevenueAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalExpensesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<(string Name, decimal TotalRevenue, decimal TotalExpenses)>> GetTotalsByPersonAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<(string Name, decimal TotalRevenue, decimal TotalExpenses)>> GetTotalsByCategoryAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Transaction?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<Transaction?> GetByIdWithRelationsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
