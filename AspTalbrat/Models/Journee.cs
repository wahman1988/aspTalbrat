namespace AspTalbrat.Models
{
    public class Journee
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string? Note { get; set; }
        public IEnumerable<JourneePaiement>? Paiements { get; set; }
    }
}
