using System.ComponentModel.DataAnnotations;

namespace Phase_07_Poc_01.Infrastructure.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public required string ProductCode { get; set; }
        
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        
        // Audit Logs
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public Category? Category { get; set; }

    }
}   