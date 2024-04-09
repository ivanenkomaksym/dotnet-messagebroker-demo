using Common.Models;

namespace Common.Events
{
    public class SubUserCashback
    {
        public required CustomerInfo CustomerInfo { get; init; }

        public required string Cashback { get; init; }
    }
}
