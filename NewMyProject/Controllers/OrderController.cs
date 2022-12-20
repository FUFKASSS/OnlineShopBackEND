using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.Data;
using NewMyProject.Entities;
using NewMyProject.Services;
using Newtonsoft.Json;
using System.Text.Json;

namespace NewMyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly EfContext _efContext;

        public OrderController(EfContext efContext)
        {
            _efContext = efContext ?? throw new ArgumentNullException(nameof(_efContext));
            
        }

        //получаем профиль с заказами, опираясь на токен, точнее его содержание
        [HttpGet("ProfileGet"), Authorize(AuthenticationSchemes = "Bearer ")]
        public IActionResult ProfileGet()
        {
            //Получаем с токена информацию о Claims
            try
            {
                //Дальше тут будет один антипаттерн WET - we enjoy typing. Лучше бы вынес это и передавал.
                string RoleClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.InvariantCultureIgnoreCase)).Value;
                string Email = User.Claims.FirstOrDefault(x => x.Type.Equals("Email", StringComparison.InvariantCultureIgnoreCase)).Value;
                string name = User.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", StringComparison.InvariantCultureIgnoreCase)).Value;
                string PhoneNumber = User.Claims.FirstOrDefault(x => x.Type.Equals("PhoneNumber", StringComparison.InvariantCultureIgnoreCase)).Value;
               
                //Ищем информацию в БД о профиле с помощью интерфейса запросов IQeryable
                var objectlist = _efContext.Profiles.Select(x => new
                {
                    x.id,
                    x.name,
                    x.Email,
                    x.PhoneNumber,
                    x.CreatedOn,
                    x.changeStateOrders,
                    orders = x.orders.ToList()

                }).Where(x => x.name == name)
                  .Where(x => x.Email == Email)
                  .Where(x => x.PhoneNumber.ToString() == PhoneNumber)
                  .ToList();

                return Ok(objectlist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }

        //Это хрень полная, корзина должна быть, я тут архитектуру похерил, получая данные о них с localstorage со стороны frontend
        [HttpPost("ProfilePost"), Authorize(AuthenticationSchemes = "Bearer ")]
        public IActionResult ProfilePost([FromBody] Profile profile)
        {
            //Получаем с токена информацию о Claims
            string RoleClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.InvariantCultureIgnoreCase)).Value;
            string Email = User.Claims.FirstOrDefault(x => x.Type.Equals("Email", StringComparison.InvariantCultureIgnoreCase)).Value;
            string name = User.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", StringComparison.InvariantCultureIgnoreCase)).Value;
            string PhoneNumber = User.Claims.FirstOrDefault(x => x.Type.Equals("PhoneNumber", StringComparison.InvariantCultureIgnoreCase)).Value;

            long l1 = (long)Convert.ToDouble($"{PhoneNumber}");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            //делается автогенерация id
            Order order = new Order(); 
             order.id = _efContext.SaveChanges();

            //Создаем профиль заказа
            profile = new Profile
            {
                id = profile.id,
                name = name,
                Email = Email,
                PhoneNumber = l1,
                CreatedOn = DateTime.UtcNow,
                changeStateOrders = profile.changeStateOrders,
                orders = profile.orders,
            };
            
            if(profile.orders != null)
            {
               profile.orders.Where(x => x.id == order.id);
                
            }
            
            _efContext.Profiles.Add(profile);
            _efContext.SaveChanges();
            return Ok(JsonConvert.SerializeObject(profile));
        }

        //Контроллер не закончен и не внедрен в FrontEnd
        [HttpGet("GetProfileOrders"), Authorize(AuthenticationSchemes = "Bearer ")]
        public IActionResult GetProfileOrders([FromQuery(Name = "StateOrders")] int? changeStateOrders)
        {
            if (changeStateOrders != null)
            {
                //Получаем с токена информацию о Claims
                string RoleClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.InvariantCultureIgnoreCase)).Value;
                string Email = User.Claims.FirstOrDefault(x => x.Type.Equals("Email", StringComparison.InvariantCultureIgnoreCase)).Value;
                string name = User.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", StringComparison.InvariantCultureIgnoreCase)).Value;
                string PhoneNumber = User.Claims.FirstOrDefault(x => x.Type.Equals("PhoneNumber", StringComparison.InvariantCultureIgnoreCase)).Value;
                var objectlist = _efContext.Profiles.Select(x => new
                {
                    x.id,
                    x.name,
                    x.Email,
                    x.PhoneNumber,
                    x.CreatedOn,
                    x.changeStateOrders,
                    orders = x.orders.ToList()

                }).Where(x => x.name == name)
                  .Where(x => x.Email == Email)
                  .Where(x => x.PhoneNumber.ToString() == PhoneNumber)
                  .Where(x => x.changeStateOrders == changeStateOrders)
                  .ToList();

                return Ok(objectlist);
            }
            return null;
        }
    }
}
