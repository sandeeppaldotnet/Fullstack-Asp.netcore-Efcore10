using Efcore10.Data;
using Efcore10.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Efcore10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatgegoryController : ControllerBase
    {
        private readonly ProductDBContext _context;
        public CatgegoryController(ProductDBContext productDBContext )
        {
            _context = productDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<Models.Category>> CreateCategory(CategoryCreateDto request)
        {
            var category = new Models.Category
            {
                Name = request.Name,
                Description = request.Description
            };
            await _context.Categories.AddAsync(category);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Models.Category>> GetCategoryById(int id)
        {
            var category = await _context.Categories.AsNoTracking()
                .Include(p=>p.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        /// <summary>
        /// Get a category along with its products and their details by category ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}/product-details")]
        public async Task<IActionResult> GetCategoryProductDetails(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductDetail)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }
}
