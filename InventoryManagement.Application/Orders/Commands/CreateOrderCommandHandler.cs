using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Domain.Customers.Services;
using InventoryManagement.Domain.Orders.Entities;
using InventoryManagement.Domain.Orders.Exceptions;
using InventoryManagement.Domain.Orders.Services;
using InventoryManagement.Domain.Products.Services;
using MediatR;

namespace InventoryManagement.Application.Orders.Commands;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ICustomerRepository customerRepository,
    IDiscountCalculator discountCalculator,
    ICurrentUserService currentUserService) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(request.CustomerId, cancellationToken);

        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var product = await productRepository.GetById(item.ProductId, cancellationToken);

            if (product.Stock < item.Quantity)
            {
                throw new InsufficientStockException(product.Name, item.Quantity, product.Stock);
            }

            var orderItem = OrderItem.Create(product.Id, item.Quantity, product.Price);
            orderItems.Add(orderItem);

            product.DecreaseStock(item.Quantity);
            await productRepository.Update(product, cancellationToken);
        }

        var userGuid = currentUserService.CurrentUserId ?? throw new UnauthorizedAccessException();

        var lineItems = orderItems.Select(i => new OrderLineItem(i.UnitPrice, i.Quantity));
        var finalPrice = discountCalculator.CalculateDiscount(lineItems, customer.Location, DateTime.UtcNow);
        var order = Order.Create(userGuid, customer.Id, orderItems, finalPrice);

        foreach (var item in order.OrderItems)
        {
            item.SetOrderId(order.Id);
        }

        await orderRepository.Add(order, cancellationToken);

        return order.Id;
    }
}
