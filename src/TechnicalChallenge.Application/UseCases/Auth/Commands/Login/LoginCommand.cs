using MediatR;
using TechnicalChallenge.Application.UseCases.Auth;

namespace TechnicalChallenge.Application.UseCases.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
