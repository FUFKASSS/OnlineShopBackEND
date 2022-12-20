using System.Text.Json.Serialization;

namespace NewMyProject.Entities
{
    //Профиль пользователя заказов
    public class Profile
    {
        public int id { get; set; }
        public string name { get; set; }
        public string? Email { get; set; }
        public long? PhoneNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public int changeStateOrders { get; set; }
        public List<Order> orders { get; set; }
    }
}
