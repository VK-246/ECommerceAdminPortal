using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Seed;

public static class DataSeeder
{
    private static readonly Guid AdminUserId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    private static readonly Guid EditorUserId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
    private const string AdminPasswordHash = "$2a$11$WQZF3vQxlPHKjUCeBzq9iu.rNEkBaTMfGHHSipbRoGMiHfApMyPCa";
    private const string EditorPasswordHash = "$2a$11$WQZF3vQxlPHKjUCeBzq9iuJlCNq5HaEsCNXbPz7L9ppbXNqBcWnHK";

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedUsers(modelBuilder);
        SeedCategories(modelBuilder);
        SeedAttributes(modelBuilder);
        
        // --- NEW FOR PHASE 2 ---
        SeedProductsAndVariants(modelBuilder);
        SeedOrdersAndItems(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = AdminUserId, Email = "admin@ecommerce.com", PasswordHash = AdminPasswordHash, Role = AppConstants.RoleAdmin, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = EditorUserId, Email = "editor@ecommerce.com", PasswordHash = EditorPasswordHash, Role = AppConstants.RoleEditor, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices, gadgets, and accessories" },
            new Category { Id = 2, Name = "Apparel", Description = "Clothing and wearables" },
            new Category { Id = 3, Name = "Home & Garden", Description = "Household items and decor" },
            new Category { Id = 4, Name = "Sports", Description = "Sports equipment and fitness gear" },
            new Category { Id = 5, Name = "Beauty", Description = "Cosmetics and personal care" }
        );
    }

    private static void SeedAttributes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Attribute>().HasData(
            new Domain.Entities.Attribute { Id = 1, Name = "Color", Description = "Visual color" },
            new Domain.Entities.Attribute { Id = 2, Name = "Size", Description = "Physical size" },
            new Domain.Entities.Attribute { Id = 3, Name = "Storage", Description = "Storage capacity" },
            new Domain.Entities.Attribute { Id = 4, Name = "Material", Description = "Build material" },
            new Domain.Entities.Attribute { Id = 5, Name = "Weight", Description = "Physical weight" }
        );
    }

    private static void SeedProductsAndVariants(ModelBuilder modelBuilder)
    {
        // Must be deterministic for EF Core HasData to avoid recreating rows every migration
        var random = new Random(12345); 
        var products = new List<Product>();
        var variants = new List<ProductVariant>();

        int productId = 1;
        int variantId = 1;

        string[] adjectives = { "Premium", "Classic", "Pro", "Ultra", "Essential" };
        string[] electronics = { "Headphones", "Smart TV", "Laptop", "Smartphone", "Tablet" };
        string[] apparel = { "T-Shirt", "Jeans", "Jacket", "Sneakers", "Hat" };
        string[] home = { "Coffee Mug", "Desk Lamp", "Office Chair", "Rug", "Vase" };
        string[] sports = { "Yoga Mat", "Dumbbells", "Tennis Racket", "Water Bottle", "Backpack" };
        string[] beauty = { "Face Cream", "Lipstick", "Perfume", "Serum", "Shampoo" };

        string[][] categoryNouns = { electronics, apparel, home, sports, beauty };

        // 5 Categories * 3 Products each = 15 Products
        for (int catId = 1; catId <= 5; catId++)
        {
            var nouns = categoryNouns[catId - 1];
            for (int i = 0; i < 3; i++)
            {
                var productName = $"{adjectives[random.Next(adjectives.Length)]} {nouns[i]}";
                products.Add(new Product
                {
                    Id = productId,
                    Name = productName,
                    Description = $"This is a high quality {productName}.",
                    CategoryId = catId,
                    CreatedById = AdminUserId,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });

                // 1 to 5 variants per product
                int numVariants = random.Next(1, 6);
                for (int v = 0; v < numVariants; v++)
                {
                    // Random price between 10 and 1000
                    decimal price = Math.Round((decimal)(random.NextDouble() * 990 + 10), 2);
                    
                    // Force some edge cases: 10% chance of out of stock, 20% chance of low stock
                    int stockChance = random.Next(100);
                    int stock = stockChance < 10 ? 0 : (stockChance < 30 ? random.Next(1, 10) : random.Next(10, 500));

                    variants.Add(new ProductVariant
                    {
                        Id = variantId,
                        ProductId = productId,
                        SKU = $"CAT{catId}-PROD{productId}-V{variantId}",
                        Price = price,
                        StockQuantity = stock,
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    });
                    variantId++;
                }
                productId++;
            }
        }

        modelBuilder.Entity<Product>().HasData(products);
        modelBuilder.Entity<ProductVariant>().HasData(variants);
    }

    private static void SeedOrdersAndItems(ModelBuilder modelBuilder)
    {
        var random = new Random(54321); // Deterministic seed
        var orders = new List<Order>();
        var orderItems = new List<OrderItem>();
        
        // Let's generate 500 orders
        for (int i = 1; i <= 500; i++)
        {
            // Deterministic Guid generation based on index
            var orderId = new Guid(i.ToString().PadLeft(32, '0'));
            
            // Random date in the last 12 months (before Aug 31, 2026)
            var daysAgo = random.Next(0, 365);
            var orderDate = new DateTime(2026, 8, 31, 0, 0, 0, DateTimeKind.Utc).AddDays(-daysAgo);

            // Random status
            var statusRoll = random.Next(100);
            OrderStatus status = OrderStatus.Delivered;
            if (statusRoll < 5) status = OrderStatus.Cancelled;
            else if (statusRoll < 15) status = OrderStatus.Pending;
            else if (statusRoll < 35) status = OrderStatus.Shipped;

            int numItems = random.Next(1, 4);
            decimal orderTotal = 0;

            for (int j = 0; j < numItems; j++)
            {
                // We know from SeedProductsAndVariants that variant IDs go from 1 to at least 40.
                int variantId = random.Next(1, 41);
                int quantity = random.Next(1, 4);
                decimal unitPrice = Math.Round((decimal)(random.NextDouble() * 100 + 10), 2); // random mock price
                
                orderTotal += quantity * unitPrice;

                // Deterministic Guid for order item
                var orderItemId = new Guid((i * 10 + j).ToString().PadLeft(32, '0'));

                orderItems.Add(new OrderItem
                {
                    Id = orderItemId,
                    OrderId = orderId,
                    ProductVariantId = variantId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            orders.Add(new Order
            {
                Id = orderId,
                OrderDate = orderDate,
                TotalAmount = orderTotal,
                Status = status
            });
        }

        modelBuilder.Entity<Order>().HasData(orders);
        modelBuilder.Entity<OrderItem>().HasData(orderItems);
    }
}
