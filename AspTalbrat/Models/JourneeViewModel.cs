using System.ComponentModel.DataAnnotations;

namespace AspTalbrat.Models
{
    public class JourneeViewModel
    {
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        [Range(10, int.MaxValue, ErrorMessage = "Please enter a value")]
        public int Numero { get; set; }
        public string? Note { get; set; }
        public IEnumerable<JourneePaiement> Paiements { get; set; }
    }
}
