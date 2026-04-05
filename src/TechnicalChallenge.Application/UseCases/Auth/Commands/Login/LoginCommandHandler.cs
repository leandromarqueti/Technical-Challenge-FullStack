using MediatR;
using TechnicalChallenge.Application.Interfaces;
using TechnicalChallenge.Application.UseCases.Auth;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Application.UseCases.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtService;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Credenciais inválidas.");

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return new AuthResponse(token, user.Name, user.Email, expiresAt);
    }
}
