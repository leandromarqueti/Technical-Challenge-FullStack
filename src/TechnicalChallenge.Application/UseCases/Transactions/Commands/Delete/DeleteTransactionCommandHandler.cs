using MediatR;
using TransactionEntity = TechnicalChallenge.Domain.Entities.Transaction;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Delete;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Result<bool>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        TransactionEntity? transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (transaction is null || transaction.UserId != request.UserId)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        await _transactionRepository.DeleteAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Transação excluída com sucesso.");
    }
}
