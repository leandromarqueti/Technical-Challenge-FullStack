using MediatR;
using TechnicalChallenge.Application.UseCases.Categories.DTOs;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Queries.GetAll;

public class GetAllCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryDto>>>
{
}
