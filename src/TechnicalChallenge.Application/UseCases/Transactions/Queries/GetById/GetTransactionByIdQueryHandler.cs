using AutoMapper;
using MediatR;
using TechnicalChallenge.Application.UseCases.Transactions.DTOs;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Queries.GetById;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<Result<TransactionDto>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdWithRelationsAsync(request.Id, cancellationToken);

        if (transaction is null)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        var dto = _mapper.Map<TransactionDto>(transaction);
        return Result<TransactionDto>.Success(dto);
    }
}
