namespace NewMyProject.Entities
{
    //Пользователь
    //Можно было бы наставить атрибуты Required и Password, это тоже понимаю
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
