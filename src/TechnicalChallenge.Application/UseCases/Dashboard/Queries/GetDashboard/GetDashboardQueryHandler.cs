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
        var totalRevenue = await _transactionRepository.GetTotalRevenueAsync(cancellationToken);
        var totalExpenses = await _transactionRepository.GetTotalExpensesAsync(cancellationToken);

        var dashboard = new DashboardDto
        {
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            Balance = totalRevenue - totalExpenses
        };

        return Result<DashboardDto>.Success(dashboard);
    }
}
