using Efcore10.Data;
using Efcore10.Models;
using Efcore10.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Efcore10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDBContext _context;
        public ProductsController(ProductDBContext productDBContext)
        {
            _context = productDBContext;
        }

        /// <summary>
        /// Get all products from the database.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await _context.Products.
                AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProductDetail).


                ToListAsync();
            return Ok(products);
        }
        /// <summary>
        /// Get a product by its ID from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Create a new product in the database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(
        Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = true;

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product);
        }


        /// <summary>
        /// Update an existing product in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(
        int id,
        Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct is null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.IsActive = product.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Delete a product from the database by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Update an existing product in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("updateProductDto/{id:int}")]
        public async Task<IActionResult> updateProductDto(
        int id,
        UpdateProductDto product)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct is null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{productId:int}/tags")]
        public async Task<IActionResult> AddTags(
    int productId,
    List<int> tagIds)
        {
            var product = await _context.Products
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product is null)
            {
                return NotFound("Product not found.");
            }

            var tags = await _context.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();

            if (tags.Count != tagIds.Count)
            {
                return BadRequest("One or more tags do not exist.");
            }

            foreach (var tag in tags)
            {
                if (!product.Tags.Any(t => t.Id == tag.Id))
                {
                    product.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Tags assiged successfully.");
        }

        [HttpGet("{id:int}/tags")]
        public async Task<IActionResult> GetProductWithTags(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("{id:int}/summary")]
        public async Task<IActionResult> GetProductSummary(int id)
        {













            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProductSummaryDto
                {
                    Id = p.Id,

                    Name = p.Name,

                    Price = p.Price,

                    CategoryName = p.Category != null
                        ? p.Category.Name
                        : null,

                    Brand = p.ProductDetail != null
                        ? p.ProductDetail.Brand
                        : null,

                    Tags = p.Tags
                        .Select(t => t.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }


    }
}
