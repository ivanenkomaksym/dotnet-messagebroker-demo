using Common.Models;

namespace Common.Events
{
    public record AddUserCashback
    {
        public CustomerInfo CustomerInfo { get; init; }

        public string Cashback { get; init; }

        public DateTime ValidUntil { get; init; }
    }
}
