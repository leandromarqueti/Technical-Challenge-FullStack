using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Domain.Validators;
using TechnicalChallenge.Shared.Results;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Application.UseCases.Persons.Commands.Create;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Result<Guid>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePersonCommandHandler(IPersonRepository personRepository, IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var cleanDocument = DocumentValidator.CleanDocument(request.Document);

        //Verifica se já existe uma pessoa com este documento para este usuário
        var documentExists = await _personRepository.ExistsByDocumentAsync(cleanDocument, request.UserId, cancellationToken);
        if (documentExists)
        {
            return Result<Guid>.Failure(ResourceErrorMessages.DOCUMENT_ALREADY_EXISTS);
        }

        var person = new Person(request.Name, request.BirthDate, request.Document, request.UserId);

        await _personRepository.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(person.Id, "Pessoa cadastrada com sucesso.");
    }
}
