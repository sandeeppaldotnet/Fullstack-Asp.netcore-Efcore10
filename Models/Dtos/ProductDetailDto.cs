namespace Efcore10.Models.Dtos
{
    public class ProductDetailDto
    {
        
        public int ProductId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public string Warranty { get; set; } = string.Empty;

    }
}
