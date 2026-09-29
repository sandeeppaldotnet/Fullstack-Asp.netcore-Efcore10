namespace Efcore10.Models.Dtos
{

    public class ProductSummaryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? CategoryName { get; set; }

        public string? Brand { get; set; }

        public List<string> Tags { get; set; }
            = new();
    }

}
