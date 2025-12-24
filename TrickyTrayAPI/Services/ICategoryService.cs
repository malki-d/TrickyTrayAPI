using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Services
{
    public interface ICategoryService
    {
        Task<Category?> AddAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Category category);
    }
}