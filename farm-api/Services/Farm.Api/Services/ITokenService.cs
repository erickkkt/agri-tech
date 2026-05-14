using Farm.Domain.Entities;

namespace Farm.Api.Services
{
    /// <summary>
    /// Issues short-lived JWT access tokens for end users who sign in via the local
    /// /auth/login and /auth/register endpoints. The tokens are validated by the
    /// "Local" JwtBearer scheme configured in Program.cs.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>Generate a signed JWT for the given user. Returns (token, expiresAtUtc).</summary>
        (string token, DateTime expiresAtUtc) GenerateAccessToken(User user);
    }
}
