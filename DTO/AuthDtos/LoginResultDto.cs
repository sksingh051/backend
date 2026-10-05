namespace Phase_07_Poc_01.DTO.AuthDtos;

/// <summary>
/// DTO returned by the repository after successful login.
/// Contains only the fields needed for JWT generation — no password.
/// </summary>
public class LoginResultDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}
