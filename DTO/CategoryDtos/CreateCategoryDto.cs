using System.ComponentModel.DataAnnotations;

namespace Phase_07_Poc_01.DTO.CategoryDtos
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
