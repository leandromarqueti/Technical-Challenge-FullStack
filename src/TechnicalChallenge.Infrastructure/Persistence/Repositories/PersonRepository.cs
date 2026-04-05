using Microsoft.EntityFrameworkCore;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;

namespace TechnicalChallenge.Infrastructure.Persistence.Repositories;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Person?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Document == document, cancellationToken);
    }

    public async Task<bool> ExistsByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.Document == document, cancellationToken);
    }

    public async Task<bool> HasTransactionsAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Transaction>()
            .AnyAsync(t => t.PersonId == personId, cancellationToken);
    }
}
