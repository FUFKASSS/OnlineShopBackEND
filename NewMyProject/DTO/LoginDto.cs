namespace NewMyProject.DTO
{
    //логированеи пользователя
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; } //у нас он содержит claim в логине, но при логировании в DTO не нужен
        public long? PhoneNumber { get; set; }//у нас он содержит claim в логине, но при логировании в DTO не нужен
    }
}
