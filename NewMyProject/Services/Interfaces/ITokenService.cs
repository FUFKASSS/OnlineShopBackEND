using System.Security.Claims;

namespace NewMyProject.Services
{
    public interface ITokenService
    {
        //Генерируем Access Token
        string GenerateAccessToken(IEnumerable<Claim> claims);
        //Генерируем Refresh Token
        string GenerateRefreshToken();
        //Помогает обновить refresh и access token
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
