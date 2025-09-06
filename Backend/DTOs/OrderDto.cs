namespace FileAPI.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public int Number { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
