using MediatR;
using TechnicalChallenge.Application.Interfaces;
using TechnicalChallenge.Application.UseCases.Auth;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Application.UseCases.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtService;

    public RegisterCommandHandler(IUserRepository userRepository, IJwtTokenService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
            throw new DomainException("Este e-mail já está em uso.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(request.Name, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return new AuthResponse(token, user.Name, user.Email, expiresAt);
    }
}
