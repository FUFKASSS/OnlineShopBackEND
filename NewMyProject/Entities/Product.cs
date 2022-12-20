namespace NewMyProject.Entities
{
    //сущность продуктов
    public class Product
    {
        public int Id { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Types[] types { get; set; }
        public Sizes[] sizes { get; set; }
        public decimal price { get; set; }
        public int category { get; set; }
        public int rating { get; set; }
    }

    public enum Types
    {
        BunSoup,
        Container
    }

    public enum Sizes
    {
        Small,
        Medium,
        High
    }
}

