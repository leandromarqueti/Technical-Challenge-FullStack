using MediatR;
using TechnicalChallenge.Application.Interfaces;
using TechnicalChallenge.Application.UseCases.Auth;
using Microsoft.Extensions.Logging;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Application.UseCases.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtService, ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tentativa de login para o e-mail: {Email}", request.Email);
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Login falhou: E-mail não encontrado: {Email}", request.Email);
            throw new DomainException("Credenciais inválidas.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login falhou: Senha incorreta para o e-mail: {Email}", request.Email);
            throw new DomainException("Credenciais inválidas.");
        }

        _logger.LogInformation("Login bem-sucedido para o e-mail: {Email}", request.Email);
        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return new AuthResponse(token, user.Name, user.Email, expiresAt);
    }
}
