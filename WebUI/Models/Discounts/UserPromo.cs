namespace WebUI.Models.Discounts
{
    public record UserPromo
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public required string CustomerEmail { get; set; }

        public decimal Cashback { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}
