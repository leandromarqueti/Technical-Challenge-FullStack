using AutoMapper;
using MediatR;
using TechnicalChallenge.Application.UseCases.Persons.DTOs;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Persons.Queries.GetAll;

public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, Result<IReadOnlyList<PersonDto>>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    public GetAllPersonsQueryHandler(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<PersonDto>>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
    {
        var persons = await _personRepository.GetAllAsync(request.UserId, cancellationToken);
        var dtos = _mapper.Map<IReadOnlyList<PersonDto>>(persons);

        return Result<IReadOnlyList<PersonDto>>.Success(dtos);
    }
}
