using System.Text.Json.Serialization;

namespace NewMyProject.Entities
{
    //сущность заказов
    public class Order
    {
        public int id { get; set; } 
        public string title { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public int price { get; set; }
        public int count { get; set; }
        public string image { get; set; }
    }
}
