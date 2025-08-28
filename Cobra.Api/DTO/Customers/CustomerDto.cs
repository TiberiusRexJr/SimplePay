namespace Cobra.Api.DTO.Customers
{
    public record CustomerDto
    {
        public Guid Id { get; init; }                 // int (matches domain)
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public DateTime CreatedUtc { get; init; }    // expose
    }
}
