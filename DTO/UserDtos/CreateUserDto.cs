using System.ComponentModel.DataAnnotations;

namespace Phase_07_Poc_01.DTO.UserDtos;

public class CreateUserDto
{
    [Required]
    [StringLength(50), MinLength(4)]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(8)]
    [MaxLength(16)]
    public string? Password { get; set; }
}