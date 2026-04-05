using MediatR;
using TransactionEntity = TechnicalChallenge.Domain.Entities.Transaction;
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
        TransactionEntity? transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (transaction is null || transaction.UserId != request.UserId)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        //Valida se a categoria existe e sua finalidade
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Categoria", request.CategoryId);
        }

        //Valida se a pessoa existe
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException("Pessoa", request.PersonId);
        }

        //Regra: Compatibilidade Categoria x Tipo Transação
        bool isCompatible = category.Purpose == CategoryPurpose.Both || 
                           (request.Type == TransactionType.Revenue && category.Purpose == CategoryPurpose.Revenue) ||
                           (request.Type == TransactionType.Expense && category.Purpose == CategoryPurpose.Expense);

        if (!isCompatible)
        {
            var purposeStr = category.Purpose == CategoryPurpose.Revenue ? "receitas" : "despesas";
            return Result<bool>.Failure($"Esta categoria é permitida apenas para {purposeStr}.");
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
