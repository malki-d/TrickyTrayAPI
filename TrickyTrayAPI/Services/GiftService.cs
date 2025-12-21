using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using WebApi.Data;

namespace TrickyTrayAPI.Services
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftrepository;
        public GiftService(IGiftRepository giftRepository)
        {
            _giftrepository = giftRepository;
        }

        public async Task<IEnumerable<Gift>> GetAllAsync()
        {
            return await _giftrepository.GetAllAsync();
        }

        public async Task<Gift?> GetByIdAsync(int id)
        {
            return await _giftrepository.GetByIdAsync(id);

        }

        public async Task<Gift> AddAsync(Gift gift)
        {

            return await _giftrepository.AddAsync(gift);
        }

        public async Task<Gift> UpdateAsync(Gift gift)
        {
            return await _giftrepository.UpdateAsync(gift);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _giftrepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _giftrepository.ExistsAsync(id);
        }

    }
}
