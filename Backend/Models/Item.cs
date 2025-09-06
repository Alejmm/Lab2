using System.ComponentModel.DataAnnotations.Schema;

namespace FileAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relación con OrderDetails
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
