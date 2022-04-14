using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;

namespace ShoppingCart.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            var productDb = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (product != null)
            {
                productDb.Name = product.Name;
                productDb.Description = product.Description;
                productDb.Price = product.Price;
                if (product.ImageUrl != null)
                {
                    productDb.ImageUrl = product.ImageUrl;
                }
                
            }
        }
    }
}
