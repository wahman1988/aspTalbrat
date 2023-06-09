using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspTalbrat.Models
{
    public class JourneePaiement
    {
        public int Id { get; set; }
        public string Libelle { get; set; }
        public int JourneeId { get; set; }
        public Journee? Journee { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a value")]
        [Column(TypeName = "decimal(8, 2)")]
        public Decimal Montant { get; set; }

    }
}
