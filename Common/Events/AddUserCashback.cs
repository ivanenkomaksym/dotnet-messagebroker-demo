using Common.Models;

namespace Common.Events
{
    public record AddUserCashback
    {
        public CustomerInfo CustomerInfo { get; init; }

        public double Cashback { get; init; }

        public DateTime ValidUntil { get; init; }
    }
}
