using Common.Models;

namespace Common.Events
{
    public record CustomerCreated
    {
        public CustomerInfo CustomerInfo { get; init; }

        public DateTime CreationDateTime { get; init; }
    }
}
