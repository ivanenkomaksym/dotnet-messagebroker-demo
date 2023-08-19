using Common.Models;

namespace Common.Events
{
    public record AddUserPromo
    {
        public CustomerInfo CustomerInfo { get; init; }

        public double Promo { get; init; }

        public DateTime ValidUntil { get; init; }
    }
}
