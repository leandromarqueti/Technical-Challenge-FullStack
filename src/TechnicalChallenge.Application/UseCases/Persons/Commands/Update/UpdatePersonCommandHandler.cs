using MediatR;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Domain.Validators;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Persons.Commands.Update;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Result<bool>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePersonCommandHandler(IPersonRepository personRepository, IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException("Pessoa", request.Id);
        }

        //Verifica se o novo documento já pertence a outra pessoa para este usuário
        var cleanDocument = DocumentValidator.CleanDocument(request.Document);
        var existingPerson = await _personRepository.GetByDocumentAsync(cleanDocument, request.UserId, cancellationToken);

        if (existingPerson is not null && existingPerson.Id != request.Id)
        {
            return Result<bool>.Failure(ResourceErrorMessages.DOCUMENT_ALREADY_EXISTS);
        }

        person.Update(request.Name, request.BirthDate, request.Document, request.UserId);

        await _personRepository.UpdateAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Pessoa atualizada com sucesso.");
    }
}
