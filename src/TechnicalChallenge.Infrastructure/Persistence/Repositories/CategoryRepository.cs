using Microsoft.EntityFrameworkCore;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;

namespace TechnicalChallenge.Infrastructure.Persistence.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByDescriptionAsync(string description, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Description.ToLower() == description.ToLower(), cancellationToken);
    }

    public async Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Transaction>()
            .AnyAsync(t => t.CategoryId == categoryId, cancellationToken);
    }
}
