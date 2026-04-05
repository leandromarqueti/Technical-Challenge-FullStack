using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Update;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Result<bool>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);
        if (transaction is null)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        //vê se a categoria existe e se o tipo dela bate
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, request.UserId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Categoria", request.CategoryId);
        }

        //vê se a pessoa realmente existe
        var person = await _personRepository.GetByIdAsync(request.PersonId, request.UserId, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException("Pessoa", request.PersonId);
        }

        //menores de 18 só podem registrar despesas
        if (person.IsMinor && request.Type == TransactionType.Revenue)
        {
            return Result<bool>.Failure("Pessoas menores de 18 anos só podem ter transações do tipo 'Despesa'.");
        }

        //checa se a categoria é amigável com esse tipo de transação
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

        await _transactionRepository.UpdateAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Transação atualizada com sucesso.");
    }
}
