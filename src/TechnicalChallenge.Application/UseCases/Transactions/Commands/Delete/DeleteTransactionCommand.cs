using MediatR;
using TechnicalChallenge.Application.Common.Interfaces;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Delete;

public class DeleteTransactionCommand : IRequest<Result<bool>>, IUserOwnedRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public DeleteTransactionCommand(Guid id)
    {
        Id = id;
    }

    public DeleteTransactionCommand(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}
