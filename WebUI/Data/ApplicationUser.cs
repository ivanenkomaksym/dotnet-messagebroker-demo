using Common.Models;

namespace WebUI.Data
{
    public record ApplicationUser
    {
        public Guid CustomerId { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public UserRole UserRole { get; set; }
    }
}
