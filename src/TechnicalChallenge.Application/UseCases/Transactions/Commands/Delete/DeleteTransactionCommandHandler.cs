using MediatR;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Delete;

public class DeleteTransactionCommandHandler(
    ITransactionRepository transactionRepository, 
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTransactionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);
        if (transaction is null)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        await transactionRepository.DeleteAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Transação excluída com sucesso.");
    }
}
