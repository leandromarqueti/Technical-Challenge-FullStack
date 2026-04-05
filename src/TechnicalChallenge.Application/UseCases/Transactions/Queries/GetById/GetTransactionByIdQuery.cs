using MediatR;
using TechnicalChallenge.Application.UseCases.Transactions.DTOs;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Queries.GetById;

public class GetTransactionByIdQuery : IRequest<Result<TransactionDto>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public GetTransactionByIdQuery(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}
