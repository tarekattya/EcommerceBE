using System.Reflection;

namespace Ecommerce.Infrastructure;

public static class ApplicationDbSeeding
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        // Seed ProductBrands
        if(dbContext.ProductBrands.Count() == 0)
        {
            var brandsProduct = File.ReadAllText("..\\Ecommerce.Repositry\\DataSeed\\brands.json");
            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsProduct);

            if (brands?.Count > 0)
            {
                foreach (var brand in brands)
                {
                    dbContext.ProductBrands.Add(brand);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        // Seed ProductCategories
        if (dbContext.ProductCategories.Count() == 0)
        {
            var categoriesProduct = File.ReadAllText("F:\\Ecommerce\\Ecommerce.Repositry\\DataSeed\\types.json");
            var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesProduct);
            if (categories?.Count > 0)
            {
                foreach (var category in categories)
                {
                    dbContext.ProductCategories.Add(category);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        // Seed Products - Generate 1000 products
        var productCount = dbContext.Products.Count();
        if (productCount < 1000)
        {
            var brands = dbContext.ProductBrands.ToList();
            var categories = dbContext.ProductCategories.ToList();
            
            if (brands.Any() && categories.Any())
            {
                var random = new Random();
                var productNames = new[]
                {
                    "Premium", "Deluxe", "Classic", "Signature", "Artisan", "Gourmet", "Special", "Ultimate",
                    "Supreme", "Elite", "Exclusive", "Limited", "Vintage", "Modern", "Traditional", "Innovative"
                };
                
                var productTypes = new[]
                {
                    "Frappuccino", "Latte", "Mocha", "Macchiato", "Espresso", "Cappuccino", "Americano",
                    "Cake", "Donut", "Cookie", "Muffin", "Sandwich", "Salad", "Wrap", "Bagel", "Pastry"
                };
                
                var descriptions = new[]
                {
                    "A delicious blend of premium ingredients.",
                    "Handcrafted with care for the perfect taste.",
                    "Experience the rich flavors in every sip.",
                    "Made with the finest quality ingredients.",
                    "A delightful treat for your taste buds.",
                    "Premium quality guaranteed.",
                    "Fresh and flavorful every time.",
                    "A perfect combination of taste and quality."
                };
                
                var pictureUrls = new[]
                {
                    "images/products/sb-ang1.png", "images/products/sb-ang2.png", "images/products/sb-core1.png",
                    "images/products/sb-core2.png", "images/products/sb-react1.png", "images/products/boot-ang1.png",
                    "images/products/boot-ang2.png", "images/products/boot-core1.png", "images/products/boot-core2.png",
                    "images/products/glove-code1.png", "images/products/glove-code2.png", "images/products/glove-react1.png",
                    "images/products/hat-core1.png", "images/products/hat-react1.png", "images/products/hat-react2.png"
                };
                
                var productsToAdd = new List<Product>();
                var needed = 1000 - productCount;
                
                for (int i = 0; i < needed; i++)
                {
                    var brand = brands[random.Next(brands.Count)];
                    var category = categories[random.Next(categories.Count)];
                    var namePrefix = productNames[random.Next(productNames.Length)];
                    var productType = productTypes[random.Next(productTypes.Length)];
                    var productName = $"{namePrefix} {productType}";
                    
                    var product = new Product(
                        productName,
                        descriptions[random.Next(descriptions.Length)],
                        pictureUrls[random.Next(pictureUrls.Length)],
                        random.Next(50, 500), // Price between $50 and $500
                        brand.Id,
                        category.Id,
                        random.Next(10, 200) // Stock between 10 and 200
                    );
                    
                    productsToAdd.Add(product);
                    
                    // Batch save every 100 products for better performance
                    if (productsToAdd.Count >= 100)
                    {
                        dbContext.Products.AddRange(productsToAdd);
                        await dbContext.SaveChangesAsync();
                        productsToAdd.Clear();
                    }
                }
                
                // Add remaining products
                if (productsToAdd.Any())
                {
                    dbContext.Products.AddRange(productsToAdd);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        // Seed DeliveryMethods
        if (dbContext.DeliveryMethods.Count() == 0)
        {
            var delivermethods = File.ReadAllText("F:\\Ecommerce\\Ecommerce.Repositry\\DataSeed\\delivery.json");
            var delivermethodsList = JsonSerializer.Deserialize<List<DeliveryMethod>>(delivermethods);
            if (delivermethodsList?.Count > 0)
            {
                foreach (var delivermethod in delivermethodsList)
                {
                    dbContext.DeliveryMethods.Add(delivermethod);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        // Seed Coupons
        if (dbContext.Coupons.Count() == 0)
        {
            // Use raw SQL to insert coupons to bypass RowVersion generation issue in SQLite
            var rowVersion = new byte[8];
            var createdAt = DateTime.UtcNow;
            
            // WELCOME10
            await dbContext.Database.ExecuteSqlRawAsync(
                @"INSERT INTO Coupons (Code, DiscountValue, DiscountType, ExpiryDate, MinimumAmount, UsageLimit, UsageCount, IsActive, CreatedAt, IsDeleted, RowVersion)
                  VALUES ('WELCOME10', 10, 0, @expiry1, 50, 100, 0, 1, @created, 0, @rowVersion)",
                new Microsoft.Data.Sqlite.SqliteParameter("@expiry1", DateTime.UtcNow.AddMonths(3)),
                new Microsoft.Data.Sqlite.SqliteParameter("@created", createdAt),
                new Microsoft.Data.Sqlite.SqliteParameter("@rowVersion", rowVersion));
            
            // SAVE20
            await dbContext.Database.ExecuteSqlRawAsync(
                @"INSERT INTO Coupons (Code, DiscountValue, DiscountType, ExpiryDate, MinimumAmount, UsageLimit, UsageCount, IsActive, CreatedAt, IsDeleted, RowVersion)
                  VALUES ('SAVE20', 20, 0, @expiry2, 100, 50, 0, 1, @created, 0, @rowVersion)",
                new Microsoft.Data.Sqlite.SqliteParameter("@expiry2", DateTime.UtcNow.AddMonths(6)),
                new Microsoft.Data.Sqlite.SqliteParameter("@created", createdAt),
                new Microsoft.Data.Sqlite.SqliteParameter("@rowVersion", rowVersion));
            
            // FIXED50
            await dbContext.Database.ExecuteSqlRawAsync(
                @"INSERT INTO Coupons (Code, DiscountValue, DiscountType, ExpiryDate, MinimumAmount, UsageLimit, UsageCount, IsActive, CreatedAt, IsDeleted, RowVersion)
                  VALUES ('FIXED50', 50, 1, @expiry3, 200, NULL, 0, 1, @created, 0, @rowVersion)",
                new Microsoft.Data.Sqlite.SqliteParameter("@expiry3", DateTime.UtcNow.AddMonths(2)),
                new Microsoft.Data.Sqlite.SqliteParameter("@created", createdAt),
                new Microsoft.Data.Sqlite.SqliteParameter("@rowVersion", rowVersion));
            
            // FREESHIP
            await dbContext.Database.ExecuteSqlRawAsync(
                @"INSERT INTO Coupons (Code, DiscountValue, DiscountType, ExpiryDate, MinimumAmount, UsageLimit, UsageCount, IsActive, CreatedAt, IsDeleted, RowVersion)
                  VALUES ('FREESHIP', 0, 2, @expiry4, 50, 200, 0, 1, @created, 0, @rowVersion)",
                new Microsoft.Data.Sqlite.SqliteParameter("@expiry4", DateTime.UtcNow.AddMonths(1)),
                new Microsoft.Data.Sqlite.SqliteParameter("@created", createdAt),
                new Microsoft.Data.Sqlite.SqliteParameter("@rowVersion", rowVersion));
            
            // BOGO50
            await dbContext.Database.ExecuteSqlRawAsync(
                @"INSERT INTO Coupons (Code, DiscountValue, DiscountType, ExpiryDate, MinimumAmount, UsageLimit, UsageCount, IsActive, CreatedAt, IsDeleted, RowVersion)
                  VALUES ('BOGO50', 50, 3, @expiry5, NULL, 30, 0, 1, @created, 0, @rowVersion)",
                new Microsoft.Data.Sqlite.SqliteParameter("@expiry5", DateTime.UtcNow.AddMonths(4)),
                new Microsoft.Data.Sqlite.SqliteParameter("@created", createdAt),
                new Microsoft.Data.Sqlite.SqliteParameter("@rowVersion", rowVersion));
        }

        // Seed Orders - Generate 1000 orders with various statuses
        var orderCount = dbContext.Orders.Count();
        if (orderCount < 1000)
        {
            var products = dbContext.Products.ToList();
            var deliveryMethods = dbContext.DeliveryMethods.ToList();
            var users = dbContext.Users.ToList();

            if (products.Any() && deliveryMethods.Any())
            {
                var random = new Random();
                var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
                var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emily", "James", "Jessica", "Robert", "Ashley", "William", "Amanda", "Richard", "Melissa", "Joseph", "Michelle" };
                var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Wilson", "Anderson", "Thomas", "Taylor" };
                
                var statusDistribution = new Dictionary<OrderStatus, int>
                {
                    { OrderStatus.Pending, 150 },
                    { OrderStatus.Processing, 200 },
                    { OrderStatus.Delivered, 300 },
                    { OrderStatus.PaymentFailed, 100 },
                    { OrderStatus.Cancelled, 100 },
                    { OrderStatus.PaymentSucceeded, 100 },
                    { OrderStatus.Shipped, 50 }
                };
                
                var ordersToAdd = new List<Core.Order>();
                var needed = 1000 - orderCount;
                var statusIndex = 0;
                var statusList = statusDistribution.SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value)).ToList();
                
                // Shuffle status list for random distribution
                for (int i = statusList.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (statusList[i], statusList[j]) = (statusList[j], statusList[i]);
                }
                
                for (int i = 0; i < needed; i++)
                {
                    var product = products[random.Next(products.Count)];
                    var deliveryMethod = deliveryMethods[random.Next(deliveryMethods.Count)];
                    var buyerEmail = users.Any() && random.Next(10) < 7 
                        ? users[random.Next(users.Count)].Email! 
                        : $"customer{random.Next(10000, 99999)}@example.com";
                    
                    // Create 1-4 order items per order
                    var itemCount = random.Next(1, 5);
                    var orderItems = new List<OrderItem>();
                    
                    for (int j = 0; j < itemCount; j++)
                    {
                        var selectedProduct = products[random.Next(products.Count)];
                        orderItems.Add(new OrderItem(
                            new ProductItemOrderd
                            {
                                ProductId = selectedProduct.Id,
                                Name = selectedProduct.Name,
                                Description = selectedProduct.Description,
                                PictureUrl = selectedProduct.PictureUrl
                            },
                            selectedProduct.Price,
                            random.Next(1, 6) // Quantity between 1 and 5
                        ));
                    }

                    var subTotal = orderItems.Sum(oi => oi.Price * oi.Quantity);
                    var useCoupon = random.Next(10) < 3; // 30% chance of using coupon
                    var couponCode = useCoupon ? "WELCOME10" : null;
                    var discount = useCoupon ? subTotal * 0.1m : 0m;
                    
                    var order = new Core.Order(
                        buyerEmail,
                        new OrderAddress(
                            cities[random.Next(cities.Length)],
                            $"{random.Next(100, 9999)} {new[] { "Main", "Oak", "Park", "First", "Second", "Third", "Maple", "Cedar" }[random.Next(8)]} St",
                            "USA",
                            firstNames[random.Next(firstNames.Length)],
                            lastNames[random.Next(lastNames.Length)]
                        ),
                        deliveryMethod,
                        orderItems,
                        subTotal,
                        isCOD: random.Next(2) == 0,
                        couponCode: couponCode,
                        discount: discount
                    );

                    // Set order date to various times in the past (up to 90 days)
                    var orderDateProperty = typeof(Core.Order).GetProperty("OrderDate", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (orderDateProperty != null)
                    {
                        orderDateProperty.SetValue(order, DateTimeOffset.UtcNow.AddDays(-random.Next(0, 90)));
                    }

                    // Set order status using reflection
                    var statusProperty = typeof(Core.Order).GetProperty("Status", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (statusProperty != null)
                    {
                        var status = statusIndex < statusList.Count ? statusList[statusIndex] : statusList[random.Next(statusList.Count)];
                        statusProperty.SetValue(order, status);
                        statusIndex++;
                    }

                    ordersToAdd.Add(order);
                    
                    // Batch save every 100 orders for better performance
                    if (ordersToAdd.Count >= 100)
                    {
                        dbContext.Orders.AddRange(ordersToAdd);
                        await dbContext.SaveChangesAsync();
                        ordersToAdd.Clear();
                    }
                }
                
                // Add remaining orders
                if (ordersToAdd.Any())
                {
                    dbContext.Orders.AddRange(ordersToAdd);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }

    private static OrderStatus GetOrderStatus(Core.Order order)
    {
        var statusProperty = typeof(Core.Order).GetProperty("Status", BindingFlags.NonPublic | BindingFlags.Instance);
        return statusProperty != null ? (OrderStatus)statusProperty.GetValue(order)! : OrderStatus.Pending;
    }
}
