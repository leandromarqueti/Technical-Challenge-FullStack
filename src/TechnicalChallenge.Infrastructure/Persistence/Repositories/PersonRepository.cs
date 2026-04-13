using Microsoft.EntityFrameworkCore;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;

namespace TechnicalChallenge.Infrastructure.Persistence.Repositories;

public class PersonRepository(AppDbContext context) : Repository<Person>(context), IPersonRepository
{
    public async Task<Person?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Person>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Person?> GetByDocumentAsync(string document, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Document == document && p.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByDocumentAsync(string document, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.Document == document && p.UserId == userId, cancellationToken);
    }

    public async Task<bool> HasTransactionsAsync(Guid personId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Transaction>()
            .AnyAsync(t => t.PersonId == personId && t.UserId == userId, cancellationToken);
    }
}
