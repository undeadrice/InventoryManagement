using InventoryManagement.Domain.Customers;

namespace InventoryManagement.Application.Customers.TransferObjects;

public record CustomerDto(Guid Id, CustomerLocation Location);
