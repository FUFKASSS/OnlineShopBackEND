using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.Data;
using NewMyProject.Entities;
using NewMyProject.Services;

namespace NewMyProject.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        //Добавление продукта, что принадлежит админу, знаю, что лучше было бы вынести в отдельный контроллер админ панели, но руки не дошли
        [HttpPost("AddProduct"), Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin")]
        public async Task<Product> Add(Product product)
        {
            return await _service.AddProduct(product);
        }

        //Поиск продуктов по запросу FromQuery - удобная вещь :)
        [HttpGet("Queryproducts")]
        public List<Product> QueryGetAll(
                 [FromQuery(Name = "search")] string? s,
                 [FromQuery(Name = "sort")] string? sort,
                 [FromQuery(Name = "category")] int? category)
        {
            return _service.QueryGetAll(s, sort, category).ToList();
        }

        //Получение всех продуктов, хотел реализовать на админ панели для контроля всего дела, но не дошли руки
        [HttpGet("AllProducts")]
        public List<Product> GetAllProducts()
        {
            return _service.GetAllProducts();
        }

        //получение продукта по его идентификатору
        [HttpGet("GetProductById/{id}")]
        public Product GetProductById([FromRoute] int id)
        {
            return _service.GetProductById(id);
        }

        
    }
}
