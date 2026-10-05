namespace Phase_07_Poc_01.DTO.AdminDtos
{
    public class UserQueryParametersDto
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SortBy { get; set; } = "id";
        public string? Order { get; set; } = "asc";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
