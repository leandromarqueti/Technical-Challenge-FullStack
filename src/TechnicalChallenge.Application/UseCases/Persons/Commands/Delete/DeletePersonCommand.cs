using MediatR;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Persons.Commands.Delete;

public class DeletePersonCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public DeletePersonCommand(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}
