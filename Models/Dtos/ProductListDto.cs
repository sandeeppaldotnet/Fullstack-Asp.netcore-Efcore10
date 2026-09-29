namespace Efcore10.Models.Dtos
{
    public class ProductListDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string SKU { get; set; } = string.Empty;

    }
}
