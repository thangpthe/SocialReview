using SocialReview.Models;
using SocialReview.ViewModels;

namespace SocialReview.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);


        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product> GetProductDetailById(int id);
        Task AddAsync(Product product);


        Task UpdateAsync(Product product);

        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> FilterAsync(string? productType, int? rating);
        Task<IEnumerable<Product>> Search(string query);
        Task<Product> GetProductDetailBySlugAsync(string slug);

    }
}
