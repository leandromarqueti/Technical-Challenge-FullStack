using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnicalChallenge.Application.Interfaces;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Infrastructure.Persistence;
using TechnicalChallenge.Infrastructure.Persistence.Repositories;
using TechnicalChallenge.Infrastructure.Services;

namespace TechnicalChallenge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //banco de dados sqlite - sem precisar instalar nada
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString ?? "Data Source=technical-challenge.db");
        });

        //unit of work via contexto direto
        services.AddScoped<IUnitOfWork>(provider =>
        {
            var context = provider.GetRequiredService<AppDbContext>();
            return new UnitOfWork(context);
        });

        //repositorios
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        //servicos
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IUserRepository, UserRepository>();

        //redis opcional, só registra se tiver connection string
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
            });
        }

        return services;
    }
}
