namespace Phase_07_Poc_01.Infrastructure.Entities;

    public class User
    {
      public int Id { get; set; }
      public required string Name { get; set; }
      public required string Email { get; set; }
      public required string Password { get; set; }
      public string Role { get; set; } = "user";
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
}