namespace Cobra.Api.Domain
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    }
}
