using MediatR;
using TechnicalChallenge.Application.UseCases.Dashboard.DTOs;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Dashboard.Queries.GetDashboard;

public class GetDashboardQuery : IRequest<Result<DashboardDto>>
{
}
