namespace Cobra.Api.DTO.Customers
{
    public record CustomerSummaryDto
    {
        public Guid Id { get; init; }                 // int (matches domain)
        public string Name { get; init; } = string.Empty;
        public string? Email { get; init; }
    }
}
