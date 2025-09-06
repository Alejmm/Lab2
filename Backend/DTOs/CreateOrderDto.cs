namespace FileAPI.DTOs
{
    public class CreateOrderDto
    {
        public int PersonId { get; set; }
        public int Number { get; set; }
        public int CreatedBy { get; set; }
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
    }
}
