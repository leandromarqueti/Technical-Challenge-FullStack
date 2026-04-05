using MediatR;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Persons.Commands.Update;

public class UpdatePersonCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Document { get; set; } = string.Empty;
}
