using Efcore10.Data;
using Efcore10.Models;
using Efcore10.Models.Dtos;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Efcore10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdudctsDtoController : ControllerBase
    {
        private readonly ProductDBContext _context;
        public ProdudctsDtoController(ProductDBContext productDBContext)
        {
            _context = productDBContext;
        }

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

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(
    ProductCreateDto request)
        {
            //sku duplicate check
            var skuExists = await _context.Products
                .AnyAsync(p => p.SKU == request.SKU);


            if (skuExists)
            {
                return Conflict("SKU already exists.");
            }


            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                SKU = request.SKU,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? search, decimal? minPrice,
            decimal? maxPrice, bool? inStock, bool? active, string? sortBy, string? sortDirection)
        {
            var query = _context.Products
    .AsNoTracking()
    .AsQueryable();



            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search));
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= minPrice.Value);
            }
            //max price
            if (maxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= maxPrice.Value);
            }


            if (inStock == true)
            {
                query = query.Where(p =>
                    p.StockQuantity > 0);
            }


            if (active.HasValue)
            {
                query = query.Where(p =>
                    p.IsActive == active.Value);
            }



            sortBy = sortBy?.ToLower();
            sortDirection = sortDirection?.ToLower();

            query = sortBy switch
            {
                "name" => sortDirection == "desc"
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),

                "price" => sortDirection == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),

                "stock" => sortDirection == "desc"
                    ? query.OrderByDescending(p => p.StockQuantity)
                    : query.OrderBy(p => p.StockQuantity),

                _ => query.OrderBy(p => p.Id)
            };



            //var products = await query.ToListAsync();

            //projection

            var products=await query.Select(p=>new ProductListDto { 
                Id=p.Id,
                Name=p.Name,
                Price=p.Price,
                SKU=p.SKU
            
            
            }).ToListAsync();




            return Ok(products);
        }

    }
}
