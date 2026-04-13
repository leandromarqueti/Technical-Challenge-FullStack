using Microsoft.EntityFrameworkCore;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;

namespace TechnicalChallenge.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : Repository<Category>(context), ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByDescriptionAsync(string description, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Description.ToLower() == description.ToLower() && c.UserId == userId, cancellationToken);
    }

    public async Task<bool> HasTransactionsAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Transaction>()
            .AnyAsync(t => t.CategoryId == categoryId && t.UserId == userId, cancellationToken);
    }
}
