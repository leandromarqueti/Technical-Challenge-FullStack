using Microsoft.EntityFrameworkCore;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Domain.Interfaces;

namespace TechnicalChallenge.Infrastructure.Persistence.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    private readonly AppDbContext _appContext;

    public TransactionRepository(AppDbContext context) : base(context)
    {
        _appContext = context;
    }

    public async Task<Transaction?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _appContext.Transactions
            .Include(t => t.Category)
            .Include(t => t.Person)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetFilteredAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _appContext.Transactions
            .Include(t => t.Category)
            .Include(t => t.Person)
            .AsQueryable();

        //Aplicar filtros
        if (!string.IsNullOrWhiteSpace(description))
        {
            query = query.Where(t => t.Description.ToLower().Contains(description.ToLower()));
        }

        if (startDate.HasValue)
        {
            query = query.Where(t => t.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.Date <= endDate.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        if (personId.HasValue)
        {
            query = query.Where(t => t.PersonId == personId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        //Total antes de paginar
        var totalCount = await query.CountAsync(cancellationToken);

        //Ordenação
        query = ApplySorting(query, sortBy, sortDescending);

        //Paginação
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        return await _appContext.Transactions
            .Where(t => t.Type == TransactionType.Revenue)
            .SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalExpensesAsync(CancellationToken cancellationToken = default)
    {
        return await _appContext.Transactions
            .Where(t => t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount, cancellationToken);
    }

    private static IQueryable<Transaction> ApplySorting(
        IQueryable<Transaction> query,
        string? sortBy,
        bool sortDescending)
    {
        var sortField = sortBy?.ToLower() ?? "date";

        return sortField switch
        {
            "description" => sortDescending
                ? query.OrderByDescending(t => t.Description)
                : query.OrderBy(t => t.Description),
            "amount" => sortDescending
                ? query.OrderByDescending(t => t.Amount)
                : query.OrderBy(t => t.Amount),
            "type" => sortDescending
                ? query.OrderByDescending(t => t.Type)
                : query.OrderBy(t => t.Type),
            "category" => sortDescending
                ? query.OrderByDescending(t => t.Category!.Name)
                : query.OrderBy(t => t.Category!.Name),
            "person" => sortDescending
                ? query.OrderByDescending(t => t.Person!.Name)
                : query.OrderBy(t => t.Person!.Name),
            _ => sortDescending
                ? query.OrderByDescending(t => t.Date)
                : query.OrderBy(t => t.Date)
        };
    }
}
