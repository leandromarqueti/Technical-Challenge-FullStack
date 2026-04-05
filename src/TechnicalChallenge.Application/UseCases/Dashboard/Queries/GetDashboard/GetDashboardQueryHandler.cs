using MediatR;
using TechnicalChallenge.Application.UseCases.Dashboard.DTOs;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Dashboard.Queries.GetDashboard;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardDto>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetDashboardQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var totalRevenue = await _transactionRepository.GetTotalRevenueAsync(request.UserId, cancellationToken);
        var totalExpenses = await _transactionRepository.GetTotalExpensesAsync(request.UserId, cancellationToken);
        
        var personTotalsByRepo = await _transactionRepository.GetTotalsByPersonAsync(request.UserId, cancellationToken);
        var categoryTotalsByRepo = await _transactionRepository.GetTotalsByCategoryAsync(request.UserId, cancellationToken);

        var dashboard = new DashboardDto
        {
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            Balance = totalRevenue - totalExpenses,
            TotalsByPerson = personTotalsByRepo.Select(p => new PersonSummaryDto
            {
                Name = p.Name,
                TotalRevenue = p.TotalRevenue,
                TotalExpenses = p.TotalExpenses,
                Balance = p.TotalRevenue - p.TotalExpenses
            }).ToList(),
            TotalsByCategory = categoryTotalsByRepo.Select(c => new CategorySummaryDto
            {
                Description = c.Name, //o repositório devolve Name como padrão na tupla
                TotalRevenue = c.TotalRevenue,
                TotalExpenses = c.TotalExpenses,
                Balance = c.TotalRevenue - c.TotalExpenses
            }).ToList()
        };

        return Result<DashboardDto>.Success(dashboard);
    }
}
