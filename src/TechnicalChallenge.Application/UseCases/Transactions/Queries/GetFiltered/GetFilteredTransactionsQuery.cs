using MediatR;
using TechnicalChallenge.Application.Common.Models;
using TechnicalChallenge.Application.UseCases.Transactions.DTOs;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Queries.GetFiltered;

public class GetFilteredTransactionsQuery : IRequest<Result<PagedResult<TransactionDto>>>
{
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? PersonId { get; set; }

    public TransactionType? Type { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }

    public Guid UserId { get; set; }
}
