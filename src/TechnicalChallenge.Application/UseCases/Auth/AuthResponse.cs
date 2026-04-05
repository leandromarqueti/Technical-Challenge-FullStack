namespace TechnicalChallenge.Application.UseCases.Auth;

public record AuthResponse(string Token, string Name, string Email, DateTime ExpiresAt);
