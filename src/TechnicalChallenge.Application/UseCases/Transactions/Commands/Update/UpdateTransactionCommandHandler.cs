using MediatR;
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
        var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (transaction is null)
        {
            throw new NotFoundException("Transação", request.Id);
        }

        //Valida se a categoria existe
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new NotFoundException("Categoria", request.CategoryId);
        }

        //Valida se a pessoa existe
        var personExists = await _personRepository.ExistsAsync(request.PersonId, cancellationToken);
        if (!personExists)
        {
            throw new NotFoundException("Pessoa", request.PersonId);
        }

        transaction.Update(
            request.Description,
            request.Amount,
            request.Date,
            request.Type,
            request.CategoryId,
            request.PersonId);

        await _transactionRepository.UpdateAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Transação atualizada com sucesso.");
    }
}
