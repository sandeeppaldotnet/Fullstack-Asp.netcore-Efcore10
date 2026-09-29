namespace Efcore10.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        //navigational property for the relationship with Product
        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
