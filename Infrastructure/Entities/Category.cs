namespace Phase_07_Poc_01.Infrastructure.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        // Audit Logs
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property for related products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
