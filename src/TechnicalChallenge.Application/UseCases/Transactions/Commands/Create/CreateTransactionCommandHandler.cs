using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Create;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Result<Guid>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransactionCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
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

        var transaction = new Transaction(
            request.Description,
            request.Amount,
            request.Date,
            request.Type,
            request.CategoryId,
            request.PersonId);

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transaction.Id, "Transação criada com sucesso.");
    }
}
