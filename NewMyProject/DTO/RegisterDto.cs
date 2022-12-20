namespace NewMyProject.DTO
{
    //создание пользователя
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }

    }
}
