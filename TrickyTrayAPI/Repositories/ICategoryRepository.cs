using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> AddAsync(string name);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Category category);
    }
}