using MediatR;
using TechnicalChallenge.Application.UseCases.Auth;

namespace TechnicalChallenge.Application.UseCases.Auth.Commands.Register;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<AuthResponse>;
