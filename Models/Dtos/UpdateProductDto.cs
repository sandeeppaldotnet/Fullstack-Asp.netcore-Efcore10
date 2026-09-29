namespace Efcore10.Models.Dtos
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string SKU { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
    }
}
