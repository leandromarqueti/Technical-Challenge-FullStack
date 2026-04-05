using MediatR;
using TechnicalChallenge.Application.UseCases.Persons.DTOs;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Persons.Queries.GetById;

public class GetPersonByIdQuery : IRequest<Result<PersonDto>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public GetPersonByIdQuery(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}
