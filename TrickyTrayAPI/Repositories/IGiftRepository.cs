using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface IGiftRepository
    {
        Task<IEnumerable<Gift>> GetAllAsync();
        Task<Gift?> GetByIdAsync(int id);
        Task<Gift> AddAsync(Gift gift);
        Task<Gift> UpdateAsync(Gift gift);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        //Task<IEnumerable<ProductWithCategoryDto>> GetAllWithCategoryHierarchyAsync();
        //Task<ProductWithCategoryDto?> GetByIdWithCategoryHierarchyAsync(int id);
        //Task<IEnumerable<ProductWithCategoryDto>> GetAllWithCategoryNamesAsync();
    }
}
