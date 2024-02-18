using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Name")]
        public string? Name { get; set; }
        [Range(1, 100, ErrorMessage = "The display order must be between 1 and 100")]
        [DisplayName("Display order")]
        public int DisplayOrder { get; set; }
    }
}
