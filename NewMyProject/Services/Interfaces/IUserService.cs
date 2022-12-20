using NewMyProject.Entities;

namespace NewMyProject.Services
{
    public interface IUserService
    {
        //Создание пользователя
        public Task<User> CreateUser(User user);
        //Получение пользователя по идентификатору
        public User GetByUsername(string username);
        
    }
}
