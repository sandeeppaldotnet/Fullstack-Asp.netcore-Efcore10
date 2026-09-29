using Efcore10.Data;
using Efcore10.Models;
using Efcore10.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Efcore10.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ProductDBContext _context;

        public TagsController(ProductDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateTagRequest request)
        {
            var exists = await _context.Tags
                .AnyAsync(t => t.Name == request.Name);

            if (exists)
            {
                return Conflict("Tag already exists.");
            }

            var tag = new Tag
            {
                Name = request.Name
            };

            _context.Tags.Add(tag);

            await _context.SaveChangesAsync();

            return Ok(tag);
        }
    }

}
