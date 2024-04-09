using Common.Models;

namespace Common.Events
{
    public record AddUserCashback
    {
        public required CustomerInfo CustomerInfo { get; init; }

        public required string Cashback { get; init; }

        public DateTime ValidUntil { get; init; }
    }
}
