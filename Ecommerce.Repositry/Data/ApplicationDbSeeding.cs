using Ecommerce.API.Data;
using Ecommerce.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecommerce.Repositry.Data
{
    public static class ApplicationDbSeeding
    {
        public static async Task SeedAsync(ApplicationDbContext dbContext)
        {

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

            if (dbContext.Products.Count() == 0)
            {
                var products = File.ReadAllText("F:\\Ecommerce\\Ecommerce.Repositry\\DataSeed\\products.json");
                var productList = JsonSerializer.Deserialize<List<Product>>(products);
                if (productList?.Count > 0)
                {
                    foreach (var product in productList)
                    {
                        dbContext.Products.Add(product);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }


        }
    }
}
