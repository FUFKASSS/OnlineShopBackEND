using NewMyProject.Data;
using NewMyProject.Entities;

namespace NewMyProject.Services
{
    public class ProductService : IProductService
    {
        private readonly EfContext _context;

        public ProductService(EfContext context)
        {
            _context = context;
        }

        //добавляем продукт
        public async Task<Product> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            product.Id = await _context.SaveChangesAsync();
            return product;
        }

        //Получаем все продукты, реализация для админ панели
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }
        //Получаем продукт по Id
        public Product GetProductById(int Id)
        {
            var s = _context.Products.FirstOrDefault(x => x.Id == Id);
            if (s == null)
            {
                throw new Exception("Такого Id не существует");
            }
            else
            {
                return s;
            }
        }
        //Получаем все продукты для главной страницы
        public List<Product> QueryGetAll(string? search, string? sort, int? category)
        {
            if (search != null | sort != null | category != null )
            {
                var query = (from products in _context.Products select products);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.title.Contains(search) || p.description.Contains(search));
                }
                //price
                else if(sort == "Price-asc" & category != null)
                {
                    return _context.Products.Where(x => x.category == category).OrderBy(p => p.price).ToList();
                }
                else if (sort == "Price-asc")
                {
                    query = query.OrderBy(p => p.price);
                }
                else if (sort == "Price-desc" & category != null)
                {
                    return _context.Products.Where(x => x.category == category).OrderByDescending(p => p.price).ToList();
                }
                else if (sort == "Price-desc")
                {
                    query = query.OrderByDescending(p => p.price);
                }

                //rating
                else if (sort == "Rating-asc" & category != null)
                {
                    return _context.Products.Where(x => x.category == category).OrderBy(p => p.rating).ToList();
                }
                else if (sort == "Rating-asc")
                {
                    query = query.OrderBy(p => p.rating);
                }
                else if (sort == "Rating-desc" & category != null)
                {
                    return _context.Products.Where(x => x.category == category).OrderByDescending(p => p.rating).ToList();
                }
                else if (sort == "Rating-desc")
                {
                    query = query.OrderByDescending(p => p.rating);
                }

                //alhabet
                else if (sort == "Alphabet-asc" & category != null)
                {
                    return _context.Products.Where(x => x.category == category).OrderBy(p => p.title).ToList();
                }
                else if (sort == "Alphabet-asc")
                {
                    query = query.OrderBy(p => p.title);
                }
                else if (sort == "Alphabet-desc" & category != null)
                {
                    return _context.Products.Where(x => x.category == category).OrderByDescending(p => p.title).ToList();
                }
                else if (sort == "Alphabet-desc")
                {
                    query = query.OrderByDescending(p => p.title);
                }
                return query.ToList();
            }
            
            else if (category != null)
            {
                return _context.Products.Where(x => x.category == category).ToList();
            }
            else
            {
                return _context.Products.ToList();
            }

        }
        
    }
}
