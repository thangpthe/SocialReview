using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using SocialReview.ViewModels;

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

        public async Task<Product> GetProductDetailById(int id)
        {
            //return await _context.Products.Include(p =>p.Reviews).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.ProductID == id);
            return await _context.Products
                .Include(p => p.Company)

               
                .Include(p => p.Reviews) 
                    .ThenInclude(r => r.User)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.Comments) 
                        .ThenInclude(c => c.User) 
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.Reactions) 

                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        //public async Task<IEnumerable<Product>> FilterAsync(string? productType, int? rating)
        //{
        //    var query = _context.Products
        //                        .Include(p => p.Company)
        //                        .Include(p => p.Reviews) // Thêm này nếu lọc theo rating
        //                        .AsQueryable();

        //    if (!string.IsNullOrEmpty(productType))
        //    {
        //        query = query.Where(p => p.ProductType == productType);
        //    }

        //    if (rating.HasValue)
        //    {
        //        query = query.Where(p => p.Reviews.Any() &&
        //                                 p.Reviews.Average(r => r.Rating) >= rating.Value);
        //    }

        //    return await query.ToListAsync();
        //}

        public async Task<IEnumerable<Product>> FilterAsync(string? productType, int? rating)
        {
            // Bắt đầu với query cơ bản
            var query = _context.Products
                .Include(p => p.Company)
                .AsQueryable();

            // Lọc theo loại sản phẩm
            if (!string.IsNullOrEmpty(productType))
            {
                query = query.Where(p => p.ProductType == productType);
            }

            // Lọc theo rating - CHÚ Ý: Phải Include Reviews trước
            if (rating.HasValue)
            {
                query = query
                    .Include(p => p.Reviews) // Include Reviews để tính average
                    .Where(p => p.Reviews.Any() &&
                                p.Reviews.Average(r => r.Rating) >= rating.Value);
            }
            else
            {
                // Nếu không lọc theo rating, vẫn include Reviews cho hiển thị
                query = query.Include(p => p.Reviews);
            }

            var results = await query.ToListAsync();

            // DEBUG: In ra console để kiểm tra
            Console.WriteLine($"Filter params - Type: {productType}, Rating: {rating}");
            Console.WriteLine($"Results count: {results.Count()}");

            return results;
        }

        // --- THỰC HIỆN PHƯƠNG THỨC NÀY ---
        public async Task<IEnumerable<Product>> Search(string query)
        {
            // Chuyển sang chữ thường để tìm kiếm không phân biệt hoa/thường
            var lowerQuery = query.ToLower();

            var results = await _context.Products
                                    .Include(p => p.Company) // Include Company để hiển thị card
                                    .Where(p =>
                                        // Tìm theo tên sản phẩm
                                        p.ProductName.ToLower().Contains(lowerQuery) ||
                                        // Hoặc tìm theo tên công ty
                                        (p.Company != null && p.Company.CompanyName.ToLower().Contains(lowerQuery))
                                    )
                                    .ToListAsync();

            return results;
        }
    }
    }
