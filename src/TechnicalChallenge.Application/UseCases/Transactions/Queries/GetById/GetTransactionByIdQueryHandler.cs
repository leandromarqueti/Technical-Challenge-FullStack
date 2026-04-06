using AutoMapper;
using MediatR;
using TransactionEntity = TechnicalChallenge.Domain.Entities.Transaction;
using TechnicalChallenge.Application.UseCases.Transactions.DTOs;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Queries.GetById;

public class GetTransactionByIdQueryHandler(
    ITransactionRepository transactionRepository, 
    IMapper mapper) : IRequestHandler<GetTransactionByIdQuery, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdWithRelationsAsync(request.Id, request.UserId, cancellationToken);

        if (transaction is null || transaction.UserId != request.UserId)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        return Result<TransactionDto>.Success(mapper.Map<TransactionDto>(transaction));
    }
}
