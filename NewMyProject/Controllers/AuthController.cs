using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.Data;
using NewMyProject.DTO;
using NewMyProject.Entities;
using NewMyProject.Services;
using System.Security.Claims;
using System.Text.Json;

namespace NewMyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly EfContext _efContext;
        private readonly ITokenService _tokenService;

        public AuthController(EfContext efContext, ITokenService tokenService, IUserService userService)
        {
            _efContext = efContext ?? throw new ArgumentNullException(nameof(tokenService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userService = userService;
        }

        //Регистрация пользователя
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Определяем при создании пользователя - его роль юзер и другие параметры передаем с Dto
            var user = new User
            {
                UserName = dto.UserName,
                Role = "User",
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Password =BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };
            await _userService.CreateUser(user);
            _efContext.SaveChanges();
            return Ok("Your Registered!");
            
        }

        //Получаем роль пользователя
        [HttpGet("GetUserRole"), Authorize(AuthenticationSchemes = "Bearer ")]
        public IActionResult GetUserRole()
        {
            //Получаем токен, а по нему идентифицируем роль
            var RoleClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.InvariantCultureIgnoreCase));
            if (RoleClaim != null)
            {
                return Ok($"{RoleClaim.Value}");
            }
            return BadRequest();
        }

        
        //Логирование пользователя
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginDto loginModel)
        {
            var username = _userService.GetByUsername(loginModel.UserName);
            if (loginModel is null)
            {
                return BadRequest("Invalid client request1");
            }
            if(username is null)
            {
                return BadRequest("Invalid client request2");
            }
            //Для хеширования пороля я использовал библиотеку BCrypt, вместо солей 
            if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, username.Password))
            {
                throw new Exception("invalid credentials");
            }
            if (username is null) 
                return Unauthorized();
            //Определяем Claims для пользователя при логировании, Email и PhoneNumber можно не указывать при логировании из-за символа "?", значащие возможность быть null
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginModel.UserName),
                new Claim(ClaimTypes.Role, username.Role),
                new Claim("Email", username.Email),
                new Claim("PhoneNumber", username.PhoneNumber.ToString()),
            };
            //Генерируем токены
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            //Внедряем в куки refresh токен, но его изменить нельзя нигде, кроме бэкенда т.к HttpOnly
            Response.Cookies.Append("jwtRefreshToken", refreshToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
            });
            username.RefreshToken = refreshToken; //выдаем пользователю токен
            username.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);//Определяем время жизни токена

            //По скольку мы генерируем refresh токен в БД нам надо сохранить изменения
            _efContext.SaveChanges();

            return Ok(new AuthenticatedResponse
            {
                Token = accessToken,
            });
        }
    }
}
