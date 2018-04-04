using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.ProductCatalog.Model;

namespace ECommerce.ProductCatalog
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();

        Task AddProduct(Product product);

        Task<bool> RemoveProduct(Guid productId);

        Task<Product> GetProduct(Guid productId);
    }
}
