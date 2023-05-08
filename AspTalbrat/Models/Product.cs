using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AspTalbrat.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a value")]
        [Column(TypeName = "decimal(8, 2)")]
        public Decimal Price { get; set; }
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }=string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
