using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialReview.Repositories.Class
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            // Dùng Include để lấy thông tin User (chủ sở hữu)
            return await _context.Companies
                //.Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CompanyID == id);
        }

        public async Task<Company?> GetCompanyByUserIdAsync(int userId)
        {
            return await _context.Companies.Include(c => c.Products).FirstOrDefaultAsync(c => c.UserID == userId);
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        
    }
}
