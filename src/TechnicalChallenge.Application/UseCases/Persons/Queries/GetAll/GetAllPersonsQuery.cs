using MediatR;
using TechnicalChallenge.Application.UseCases.Persons.DTOs;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Persons.Queries.GetAll;

public class GetAllPersonsQuery : IRequest<Result<IReadOnlyList<PersonDto>>>
{
}
