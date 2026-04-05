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

    public async Task<Transaction?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _appContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);
    }

    public async Task<Transaction?> GetByIdWithRelationsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _appContext.Transactions
            .Include(t => t.Category)
            .Include(t => t.Person)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetFilteredAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _appContext.Transactions
            .Include(t => t.Category)
            .Include(t => t.Person)
            .Where(t => t.UserId == userId)
            .AsQueryable();

        //filtros iniciais
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

        //conta tudo antes de paginar
        var totalCount = await query.CountAsync(cancellationToken);

        //ordem dos dados
        query = ApplySorting(query, sortBy, sortDescending);

        //corte da página
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<decimal> GetTotalRevenueAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var total = await _appContext.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Revenue)
            .Select(t => (double)t.Amount)
            .SumAsync(cancellationToken);

        return (decimal)total;
    }

    public async Task<decimal> GetTotalExpensesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var total = await _appContext.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense)
            .Select(t => (double)t.Amount)
            .SumAsync(cancellationToken);

        return (decimal)total;
    }

    public async Task<IEnumerable<(string Name, decimal TotalRevenue, decimal TotalExpenses)>> GetTotalsByPersonAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var totals = await _appContext.Transactions
            .Include(t => t.Person)
            .Where(t => t.UserId == userId)
            .GroupBy(t => new { t.PersonId, t.Person!.Name })
            .Select(g => new
            {
                Name = g.Key.Name ?? "N/A",
                TotalRevenue = (decimal)g.Where(x => x.Type == TransactionType.Revenue).Sum(x => (double)x.Amount),
                TotalExpenses = (decimal)g.Where(x => x.Type == TransactionType.Expense).Sum(x => (double)x.Amount)
            })
            .ToListAsync(cancellationToken);

        return totals.Select(t => (t.Name, t.TotalRevenue, t.TotalExpenses));
    }

    public async Task<IEnumerable<(string Name, decimal TotalRevenue, decimal TotalExpenses)>> GetTotalsByCategoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var totals = await _appContext.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .GroupBy(t => new { t.CategoryId, t.Category!.Description })
            .Select(g => new
            {
                Name = g.Key.Description ?? "N/A",
                TotalRevenue = (decimal)g.Where(x => x.Type == TransactionType.Revenue).Sum(x => (double)x.Amount),
                TotalExpenses = (decimal)g.Where(x => x.Type == TransactionType.Expense).Sum(x => (double)x.Amount)
            })
            .ToListAsync(cancellationToken);

        return totals.Select(t => (t.Name, t.TotalRevenue, t.TotalExpenses));
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
                ? query.OrderByDescending(t => t.Category!.Description)
                : query.OrderBy(t => t.Category!.Description),
            "person" => sortDescending
                ? query.OrderByDescending(t => t.Person!.Name)
                : query.OrderBy(t => t.Person!.Name),
            _ => sortDescending
                ? query.OrderByDescending(t => t.Date)
                : query.OrderBy(t => t.Date)
        };
    }
}
