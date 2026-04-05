using AutoMapper;
using MediatR;
using TechnicalChallenge.Application.Common.Models;
using TechnicalChallenge.Application.UseCases.Transactions.DTOs;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Queries.GetFiltered;

public class GetFilteredTransactionsQueryHandler
    : IRequestHandler<GetFilteredTransactionsQuery, Result<PagedResult<TransactionDto>>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetFilteredTransactionsQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<TransactionDto>>> Handle(
        GetFilteredTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _transactionRepository.GetFilteredAsync(
            request.Description,
            request.StartDate,
            request.EndDate,
            request.CategoryId,
            request.PersonId,
            request.Type,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<TransactionDto>>(items);

        var pagedResult = new PagedResult<TransactionDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<TransactionDto>>.Success(pagedResult);
    }
}
