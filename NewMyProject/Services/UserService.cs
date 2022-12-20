using NewMyProject.Data;
using NewMyProject.Entities;

namespace NewMyProject.Services
{
    public class UserService : IUserService
    {
        private readonly EfContext _context;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dbContext">Контекст БД</param>
        public UserService(EfContext context)
        {
            _context = context;
        }

        //создание пользователя
        public async Task<User> CreateUser(User user)
        {
            await _context.LoginModels.AddAsync(user);
            user.Id = await _context.SaveChangesAsync();
            return user;
        }

        //поиск пользователя по имени
        public User GetByUsername(string username)
        {
             return  _context.LoginModels.FirstOrDefault(u => u.UserName == username );
        }
    }
}
