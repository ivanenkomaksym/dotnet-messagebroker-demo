using Common.Models;

namespace WebUI.Data
{
    public record ApplicationUser
    {
        public Guid CustomerId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public UserRole UserRole { get; set; }
    }
}
