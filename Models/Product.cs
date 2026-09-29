using System.Drawing;

namespace Efcore10.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string SKU { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public string Size { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        // Foreign key for Category
        public int? CategoryId { get; set; }

        // Navigation property for Category
        public Category? Category { get; set; }

        //Navigation property for ProductDetails

      public  ProductDetail? ProductDetail { get; set; }

        // Navigation property for Tags (many-to-many relationship)
        public ICollection<Tag> Tags { get; set; }
    = new List<Tag>();

    }
}
