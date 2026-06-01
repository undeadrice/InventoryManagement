using InventoryManagement.API.Customers.Responses;
using InventoryManagement.Application.Customers.TransferObjects;

namespace InventoryManagement.API.Customers.Mappings;

public static class CustomerMappingExtensions
{
    public static CustomerResponse MapToCustomerResponse(this CustomerDto dto) =>
        new CustomerResponse(dto.Id, dto.Location.ToString());
}
