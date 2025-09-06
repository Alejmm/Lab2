namespace FileAPI.DTOs
{
    public class CreateItemDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CreatedBy { get; set; }
    }
}
