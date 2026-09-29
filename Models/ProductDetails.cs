namespace Efcore10.Models
{
    public class ProductDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Brand { get; set; }
        public string ModelNumber { get; set; }
        public string Warranty { get; set; }

        //navigation on product
        public Product Product { get; set; }
    }
}
