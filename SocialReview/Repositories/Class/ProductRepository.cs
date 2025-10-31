using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;

namespace SocialReview.Repositories.Class
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // Dùng Include để lấy thông tin User (chủ sở hữu)
            return await _context.Products.ToListAsync();
            //.Include(c => c.User).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(c => c.ProductID)
                .FirstOrDefaultAsync(c => c.CompanyID == id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
    }
