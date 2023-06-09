namespace AspTalbrat.Models
{
    public class Journee
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Numero { get; set; }
        public string? Note { get; set; }
        public IEnumerable<JourneePaiement>? Paiements { get; set; }
    }
}
