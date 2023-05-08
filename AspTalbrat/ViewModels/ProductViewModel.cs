using AspTalbrat.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspTalbrat.ViewModels
{
    public class ProductViewModel
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [StringLength(255)]
        public string? Description { get; set; }
        [Required]
        public Decimal Price { get; set; } = 1;
        [Required,Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        public IEnumerable<Category>? Categories { get; set; }

    }
}
