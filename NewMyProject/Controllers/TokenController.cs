using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.Data;
using NewMyProject.DTO;
using NewMyProject.Entities;
using NewMyProject.Services;

namespace NewMyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly EfContext _efContext;
        private readonly ITokenService _tokenService;

        public TokenController(EfContext efContext, ITokenService tokenService)
        {
            this._efContext = efContext ?? throw new ArgumentNullException(nameof(_efContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        //Делаем рефреш токена
        [HttpPost("refresh")]
        public IActionResult Refresh(TokenApiDto tokenApidto)
        {
            if (tokenApidto is null)
                return BadRequest("Invalid client request");
            //определяем токены, которые прислали на сервер
            string accessToken = tokenApidto.AccessToken;
            string refreshToken = tokenApidto.RefreshToken;
            //проверяем содержит ли Cookies jwtRefreshToken
            var checkRefreshToken = Request.Cookies.ContainsKey("jwtRefreshtoken");
            if (checkRefreshToken == true)
            {
                refreshToken = Request.Cookies["jwtRefreshtoken"];
            }
            
            
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            //определяем по токену имя пользователя
            var username = principal.Identity.Name;

            var user = _efContext.LoginModels.SingleOrDefault(u => u.UserName == username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);//тут генерируем новый токен, исходя от полученного токена по прошлой его ролью на веб приложении
            _efContext.SaveChanges();
            return Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken,
            });
        }

        //Revoke = log out, отзываем токен доступа
        [HttpPost("revoke"), Authorize(AuthenticationSchemes = "Bearer ")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;

            var user = _efContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();
            var checkRefreshToken = Request.Cookies.ContainsKey("jwtRefreshtoken");
            if(checkRefreshToken == true)
            {
                Response.Cookies.Delete("jwtRefreshtoken");
            }

            //присваиваем токену нулевое значение
            user.RefreshToken = null;

            _efContext.SaveChanges();

            return Ok("all successfull");
        }
    }
}
