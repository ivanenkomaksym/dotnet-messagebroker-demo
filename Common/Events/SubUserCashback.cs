using Common.Models;

namespace Common.Events
{
    public class SubUserCashback
    {
        public CustomerInfo CustomerInfo { get; init; }

        public string Cashback { get; init; }
    }
}
