using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models; // Adjust namespace as needed
using WebApi.Data;

namespace TrickyTrayAPI.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        


        // Get all categories
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Get category by id
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // Add new category
        public async Task<Category> AddAsync(string name)
        {
            var category = new Category { Name = name };
             _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        // Update category
        public async Task<bool> UpdateAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.Id);
            if (existing == null)
                return false;

            // Update properties
            existing.Name = category.Name;
            // Add other properties as needed

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete category
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
        // סינון לפי שם (חיפוש חלקי)
        public IEnumerable<Donor> GetDonorsByName(string name)
        {
            return _context.Set<Donor>()
                .Where(d => d.Name != null && d.Name.Contains(name))
                .ToList();
        }

        // סינון לפי מייל
        public IEnumerable<Donor> GetDonorsByEmail(string email)
        {
            return _context.Set<Donor>()
                .Where(d => d.Email != null && d.Email.Equals(email))
                .ToList();
        }

        // סינון לפי שם מתנה
        public IEnumerable<Donor> GetDonorsByGift(string giftName)
        {
            return _context.Set<Donor>()
                .Where(d => d.Gifts.Any(g => g.Name != null && g.Name.Contains(giftName)))
                .ToList();
        }
    }
}