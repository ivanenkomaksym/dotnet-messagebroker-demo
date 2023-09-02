using Common.Models;

namespace Common.Events
{
    public class SubUserCashback
    {
        public CustomerInfo CustomerInfo { get; init; }

        public double Cashback { get; init; }
    }
}
