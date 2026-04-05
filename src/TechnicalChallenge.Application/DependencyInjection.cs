using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using TechnicalChallenge.Application.Pipelines;
using AutoMapper;
using TechnicalChallenge.Application.Common.Mappings;

namespace TechnicalChallenge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        //mediatr + pipeline de validacao
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        });

        //validators do assembly
        services.AddValidatorsFromAssembly(assembly);

        //automapper
        services.AddAutoMapper(assembly);

        return services;
    }
}
