using NewMyProject.Entities;

namespace NewMyProject.Services
{
    public interface IProductService
    {
        //добавляем продукты для админ панели
        public Task<Product> AddProduct(Product product);
        //получаем продукты для главной страницы
        public List<Product> QueryGetAll(string? search, string? sort, int? category);
        //получаем продукты для админ панели
        public List<Product> GetAllProducts();
        //Получаем продукт по Id
        public Product GetProductById(int Id);
    }
}
