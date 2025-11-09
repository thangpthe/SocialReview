using SocialReview.Models;
using System.Threading.Tasks;

namespace SocialReview.Repositories.Interface
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdWithUserAsync(int id);
        Task<Comment?> GetByIdAsync(int id);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}
