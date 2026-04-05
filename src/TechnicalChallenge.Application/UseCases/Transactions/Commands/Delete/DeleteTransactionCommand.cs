using MediatR;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Delete;

public class DeleteTransactionCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }

    public DeleteTransactionCommand(Guid id)
    {
        Id = id;
    }
}
