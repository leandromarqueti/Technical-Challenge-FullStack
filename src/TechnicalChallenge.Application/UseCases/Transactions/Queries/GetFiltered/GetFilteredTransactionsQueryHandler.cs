using AutoMapper;
using MediatR;
using TechnicalChallenge.Application.Common.Models;
using TechnicalChallenge.Application.UseCases.Transactions.DTOs;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Queries.GetFiltered;

public class GetFilteredTransactionsQueryHandler(
    ITransactionRepository transactionRepository, 
    IMapper mapper) : IRequestHandler<GetFilteredTransactionsQuery, Result<PagedResult<TransactionDto>>>
{
    public async Task<Result<PagedResult<TransactionDto>>> Handle(
        GetFilteredTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await transactionRepository.GetFilteredAsync(
            request.Description,
            request.StartDate,
            request.EndDate,
            request.CategoryId,
            request.PersonId,
            request.Type,
            request.UserId,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        var dtos = mapper.Map<IReadOnlyList<TransactionDto>>(items);

        return Result<PagedResult<TransactionDto>>.Success(new PagedResult<TransactionDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize));
    }
}
