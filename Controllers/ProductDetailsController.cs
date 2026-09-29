using Efcore10.Data;
using Efcore10.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Efcore10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly ProductDBContext _context;
        public ProductDetailsController(ProductDBContext productDBContext)
        {
            _context = productDBContext;
        }
        [HttpPost]
        public async Task<ActionResult<Models.ProductDetail>> CreateProductDetails(ProductDetailDto productDetails)
        {
            var productExists = await _context.Products.AnyAsync(p => 
            p.Id == productDetails.ProductId);

            if(!productExists)
            {
                return BadRequest($"Product with Id {productDetails.ProductId} does not exist.");
            }

            var productDetailsEntity = new Models.ProductDetail
            {
                ProductId = productDetails.ProductId,
                Brand = productDetails.Brand,
                ModelNumber = productDetails.ModelNumber,
                Warranty = productDetails.Warranty
            };

            await _context.ProductDetails.AddAsync(productDetailsEntity);

            await _context.SaveChangesAsync();

            return Ok("Product Details created Successfully");
        }

        // GET: api/ProductDetails/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Models.ProductDetail>> GetProductDetailsById(int id)
        {
            var productDetails = await _context.ProductDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(pd => pd.Id == id);
            if (productDetails is null)
            {
                return NotFound();
            }
            return Ok(productDetails);
        }
    }
}
