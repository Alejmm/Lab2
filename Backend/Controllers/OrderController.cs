using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileAPI.Data;
using FileAPI.Models;
using FileAPI.DTOs;

namespace FileAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Person)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Item)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                PersonId = order.PersonId,
                PersonName = order.Person?.FirstName + " " + order.Person?.LastName,
                Number = order.Number,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ItemId = od.ItemId,
                    ItemName = od.Item?.Name ?? "",
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Total = od.Total
                }).ToList()
            }).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Person)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return new OrderDto
            {
                Id = order.Id,
                PersonId = order.PersonId,
                PersonName = order.Person?.FirstName + " " + order.Person?.LastName,
                Number = order.Number,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ItemId = od.ItemId,
                    ItemName = od.Item?.Name ?? "",
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Total = od.Total
                }).ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
        {
            var order = new Order
            {
                PersonId = dto.PersonId,
                Number = dto.Number,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                OrderDetails = dto.OrderDetails.Select(d => new OrderDetail
                {
                    ItemId = d.ItemId,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    Total = d.Total,
                    CreatedBy = d.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new OrderDto
            {
                Id = order.Id,
                PersonId = order.PersonId,
                PersonName = "",
                Number = order.Number,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ItemId = od.ItemId,
                    ItemName = "",
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Total = od.Total
                }).ToList()
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
