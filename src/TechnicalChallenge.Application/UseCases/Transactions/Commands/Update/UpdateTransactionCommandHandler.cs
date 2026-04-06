using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Update;

public class UpdateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IPersonRepository personRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTransactionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);
        if (transaction is null)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, request.UserId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Categoria", request.CategoryId);
        }

        var person = await personRepository.GetByIdAsync(request.PersonId, request.UserId, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException("Pessoa", request.PersonId);
        }

        if (person.IsMinor && request.Type == TransactionType.Revenue)
        {
            return Result<bool>.Failure("Pessoas menores de 18 anos só podem ter transações do tipo 'Despesa'.");
        }

        if (category.Purpose != CategoryPurpose.Both && (int)category.Purpose != (int)request.Type)
        {
            return Result<bool>.Failure("A categoria selecionada não é compatível com este tipo de transação.");
        }

        transaction.Update(
            request.Description,
            request.Amount,
            request.Date,
            request.Type,
            request.CategoryId,
            request.PersonId,
            request.UserId);

        await transactionRepository.UpdateAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Transação atualizada com sucesso.");
    }
}
