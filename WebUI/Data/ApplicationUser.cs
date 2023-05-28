namespace WebUI.Data
{
    public record ApplicationUser
    {
        public Guid CustomerId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
