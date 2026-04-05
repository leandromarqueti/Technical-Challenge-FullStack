using MediatR;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Persons.Commands.Create;

public class CreatePersonCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Document { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
