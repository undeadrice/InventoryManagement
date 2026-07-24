using InventoryManagement.Domain.Products.Entities;
using InventoryManagement.Domain.Products.Services;
using MediatR;

namespace InventoryManagement.Application.Products.Commands;

public class SeedProductsCommandHandler(IProductRepository productRepository)
    : IRequestHandler<SeedProductsCommand, int>
{
    private static readonly string[] Adjectives = ["Amazing", "Premium", "Durable", "Eco", "Smart", "Ultra", "Pro", "Light", "Compact", "Ergonomic"];
    private static readonly string[] Nouns = ["Widget", "Gadget", "Tool", "Device", "Component", "Accessory", "Module", "Kit", "Set", "Unit"];
    private static readonly string[] Descriptions =
    [
        "High quality product for everyday use",
        "Professional grade item built to last",
        "Versatile solution for any workspace",
        "Reliable performance at an affordable price",
        "Innovative design with modern features",
        "Essential item for any collection",
        "Durable construction with premium materials",
        "Compact and portable for on the go",
        "User friendly with intuitive controls",
        "Multi purpose design for various applications"
    ];
    private static readonly Random Random = new();

    public async Task<int> Handle(SeedProductsCommand request, CancellationToken cancellationToken)
    {
        var products = new List<Product>();

        for (var i = 0; i < request.Quantity; i++)
        {
            var adjective = Adjectives[Random.Next(Adjectives.Length)];
            var noun = Nouns[Random.Next(Nouns.Length)];
            var number = Random.Next(1, 9999);
            var name = $"{adjective} {noun} #{number}";

            var description = Descriptions[Random.Next(Descriptions.Length)];
            var price = Math.Round((decimal)(Random.NextDouble() * 100 + 1), 2);
            var stock = Random.Next(0, 1000);

            var product = Product.Create(name, description, price, stock);
            products.Add(product);
        }

        foreach (var product in products)
        {
            await productRepository.Add(product, cancellationToken);
        }

        return products.Count;
    }
}