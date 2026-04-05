using MediatR;
using TechnicalChallenge.Application.UseCases.Categories.DTOs;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Queries.GetById;

public class GetCategoryByIdQuery : IRequest<Result<CategoryDto>>
{
    public Guid Id { get; set; }

    public GetCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}
