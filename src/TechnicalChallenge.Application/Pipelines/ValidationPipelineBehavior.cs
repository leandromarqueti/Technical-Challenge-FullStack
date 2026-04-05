using FluentValidation;
using MediatR;

namespace TechnicalChallenge.Application.Pipelines;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failuresList = new List<Shared.Exceptions.FluentValidation.ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);

            if (!result.IsValid)
            {
                failuresList.AddRange(result.Errors.Select(f => new Shared.Exceptions.FluentValidation.ValidationFailure
                {
                    PropertyName = f.PropertyName,
                    ErrorMessage = f.ErrorMessage,
                    AttemptedValue = f.AttemptedValue
                }));
            }
        }

        if (failuresList.Any())
        {
            throw new Shared.Exceptions.ValidationException(
                "Falha de validação nos dados enviados.",
                failuresList.GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray())
            );
        }

        return await next();
    }
}
