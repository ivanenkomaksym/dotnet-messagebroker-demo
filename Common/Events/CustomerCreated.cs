using Common.Models;

namespace Common.Events
{
    public record CustomerCreated
    {
        public required CustomerInfo CustomerInfo { get; init; }

        public required DateTime CreationDateTime { get; init; }
    }
}