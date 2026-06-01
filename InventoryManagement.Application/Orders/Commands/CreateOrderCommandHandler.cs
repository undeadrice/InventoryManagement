using InventoryManagement.Domain.Customers.Services;
using InventoryManagement.Domain.Orders;
using InventoryManagement.Domain.Orders.Exceptions;
using InventoryManagement.Domain.Orders.Services;
using InventoryManagement.Domain.Products.Services;
using MediatR;

namespace InventoryManagement.Application.Orders.Commands;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ICustomerRepository customerRepository,
    IDiscountCalculator discountCalculator) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(request.CustomerId, cancellationToken);

        var orderItems = new List<OrderItem>();
        decimal baseTotal = 0;
        int totalQuantity = 0;

        foreach (var item in request.Items)
        {
            var product = await productRepository.GetById(item.ProductId, cancellationToken);

            if (product.Stock < item.Quantity)
            {
                throw new InsufficientStockException(product.Name, item.Quantity, product.Stock);
            }

            var orderItem = OrderItem.Create(product.Id, item.Quantity, product.Price);
            orderItems.Add(orderItem);

            baseTotal += product.Price * item.Quantity;
            totalQuantity += item.Quantity;

            product.DecreaseStock(item.Quantity);
            await productRepository.Update(product, cancellationToken);
        }

        var finalPrice = discountCalculator.CalculateDiscount(baseTotal, totalQuantity, customer.Location, DateTime.UtcNow);
        var order = Order.Create(customer.Id, orderItems, finalPrice);

        foreach (var item in order.OrderItems)
        {
            item.SetOrderId(order.Id);
        }

        await orderRepository.Add(order, cancellationToken);

        return order.Id;
    }
}
