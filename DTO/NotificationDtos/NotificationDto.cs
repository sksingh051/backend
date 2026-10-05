namespace Phase_07_Poc_01.DTO.NotificationDtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public string Type { get; set; } = "Info";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
