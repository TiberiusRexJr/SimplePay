using Cobra.Api.Domain;
using Cobra.Api.DTO.Customers;


namespace Cobra.Api.Mappings
{
    public static class CustomerMappings
    {
        public static CustomerDto ToDto(this Customer c) =>
             new CustomerDto
             {
                 Id = c.Id,          // int ↔ int
                 Name = c.Name,
                 Email = c.Email,
                 CreatedUtc = c.CreatedUtc
             };

        public static DTO.Customers.CustomerSummaryDto ToSummaryDto(this Customer c) =>
            new DTO.Customers.CustomerSummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            };
    }
}
