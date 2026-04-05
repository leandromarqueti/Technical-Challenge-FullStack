using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
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
        //vê se a categoria existe e se bate com o tipo
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
            return Result<Guid>.Failure("Pessoas menores de 18 anos só podem ter transações do tipo 'Despesa'.");
        }

        //checa se a categoria é compatível com o tipo da transação
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

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transaction.Id, "Transação criada com sucesso.");
    }
}
