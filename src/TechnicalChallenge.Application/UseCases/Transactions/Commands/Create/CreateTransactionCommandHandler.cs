using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Create;

public class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IPersonRepository personRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTransactionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
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

        //Regra de negócio: menores de 18 anos restritos a despesas
        if (person.IsMinor && request.Type == TransactionType.Revenue)
        {
            return Result<Guid>.Failure("Pessoas menores de 18 anos só podem ter transações do tipo 'Despesa'.");
        }

        //Validação de compatibilidade entre o propósito da categoria e o tipo da transação
        if (category.Purpose != CategoryPurpose.Both && (int)category.Purpose != (int)request.Type)
        {
            return Result<Guid>.Failure("A categoria selecionada não é compatível com este tipo de transação.");
        }

        var transaction = new Transaction(
            request.Description,
            request.Amount,
            request.Date,
            request.Type,
            request.CategoryId,
            request.PersonId,
            request.UserId);

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transaction.Id, "Transação criada com sucesso.");
    }
}
