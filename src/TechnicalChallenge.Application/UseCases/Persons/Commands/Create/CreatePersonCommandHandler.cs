using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Domain.Validators;
using TechnicalChallenge.Shared.Results;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Application.UseCases.Persons.Commands.Create;

public class CreatePersonCommandHandler(
    IPersonRepository personRepository, 
    IUnitOfWork unitOfWork) : IRequestHandler<CreatePersonCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var cleanDocument = DocumentValidator.CleanDocument(request.Document);

        var documentExists = await personRepository.ExistsByDocumentAsync(cleanDocument, request.UserId, cancellationToken);
        if (documentExists)
        {
            return Result<Guid>.Failure(ResourceErrorMessages.DOCUMENT_ALREADY_EXISTS);
        }

        var person = new Person(request.Name, request.BirthDate, request.Document, request.UserId);

        await personRepository.AddAsync(person, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(person.Id, "Pessoa cadastrada com sucesso.");
    }
}
