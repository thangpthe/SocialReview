using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);

       
        Task<IEnumerable<Company>> GetAllAsync();

        
        Task AddAsync(Company company);

       
        Task UpdateAsync(Company company);

        Task DeleteAsync(int id);
        Task<Company?> GetCompanyByUserIdAsync(int userId);
    }
}
