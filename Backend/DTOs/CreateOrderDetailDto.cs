namespace FileAPI.DTOs
{
    public class CreateOrderDetailDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public int CreatedBy { get; set; }
    }
}
