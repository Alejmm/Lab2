namespace FileAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Relación con Person
        public required int PersonId { get; set; }
        public Person? Person { get; set; }

        public int Number { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relación con OrderDetails
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
