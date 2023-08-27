namespace WebUI.Models.Discounts
{
    public record UserPromo
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string CustomerEmail { get; set; }

        public double Promo { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}
