using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);


        Task<IEnumerable<Product>> GetAllAsync();


        Task AddAsync(Product product);


        Task UpdateAsync(Product product);

        Task DeleteAsync(int id);
    }
}
