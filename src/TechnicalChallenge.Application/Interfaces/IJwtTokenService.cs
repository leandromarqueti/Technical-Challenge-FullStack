using TechnicalChallenge.Domain.Entities;

namespace TechnicalChallenge.Application.Interfaces;

public interface IJwtTokenService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
