using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileAPI.Data;
using FileAPI.Models;
using FileAPI.DTOs;

namespace FileAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var items = await _context.Items.ToListAsync();
            return items.Select(i => new ItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price
            }).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price
            };
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto dto)
        {
            var item = new Item
            {
                Name = dto.Name,
                Price = dto.Price,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
