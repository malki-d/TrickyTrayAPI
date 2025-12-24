using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Drawing;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using WebApi.Data;

namespace TrickyTrayAPI.Services
{
    public class GiftService : IGiftService

    {
        private readonly IGiftRepository _giftrepository;
        private readonly ILogger<GiftService> _logger;

        public GiftService(IGiftRepository giftrepository, ILogger<GiftService> logger)
        {
            _giftrepository = giftrepository;
            _logger = logger;
        }

        public async Task<IEnumerable<GetGiftDTO>> GetAllAsync()
        {
            try
            {
                var donors = await _giftrepository.GetAllAsync();
                _logger.LogInformation("get gifts");

                return donors.Select(x => new GetGiftDTO() { Name = x.Name, Description = x.Description, Category = x.Category.Name, DonorName = x.Donor.Name,ImgUrl=x.ImgUrl });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "cant get gifts");
                throw;
            }
        }

        public async Task<GetGiftDTO?> GetByIdAsync(int id)
        {
            try
            {
                var gift = await _giftrepository.GetByIdAsync(id);
                _logger.LogInformation("get gift by id " + id);

                return new GetGiftDTO { Name = gift.Name, Description = gift.Description, Category = gift.Category.Name, DonorName = gift.Donor.Name , ImgUrl = gift.ImgUrl };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant get gift by id " + id);
                throw;
            }

        }

        public async Task<GetGiftDTO> AddAsync(CreateGiftDTO gift)
        {
            try
            {
                var newGift = await _giftrepository.AddAsync(gift);
                _logger.LogInformation("create gift " + newGift.Id);
                return new GetGiftDTO { Name = newGift.Name, Description = newGift.Description, Category = newGift.Category.Name, DonorName = newGift.Donor.Name, ImgUrl = newGift.ImgUrl };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant create gift");
                throw;
            }

        }

        public async Task<GetGiftDTO> UpdateAsync(UpdateGiftDTO gift, int id)
        {
            try
            {

                var updateGift = await _giftrepository.UpdateAsync(gift, id);
                _logger.LogInformation("update gift " + id);
                return new GetGiftDTO { Name = updateGift.Name, Description = updateGift.Description, Category = updateGift.Category.Name, DonorName = updateGift.Donor.Name , ImgUrl = updateGift.ImgUrl };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant update gift " + id);
                throw;
            }

        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {

                var isSucceed = await _giftrepository.DeleteAsync(id);
                if (isSucceed)
                {
                    _logger.LogInformation("delete gift " + id);
                }
                return isSucceed;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant delete gift " + id);
                throw;
            }

        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var isSucceed = await _giftrepository.ExistsAsync(id);
                _logger.LogInformation("check Exists gift " + id);
                return isSucceed;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant check Exists gift " + id);
                throw;
            }
        }
        public async Task<IEnumerable<GetGiftDTO>> SearchAsync(string? giftName, string? donorName, int? purchaserCount)
        {
            var gifts = await _giftrepository.SearchAsync(giftName, donorName, purchaserCount);
            return gifts.Select(g => new GetGiftDTO
            {
                Name = g.Name,
                DonorName = g.Donor.Name,
                Description = g.Description,
                Category = g.Category.Name,
                ImgUrl= g.ImgUrl
            });
        }
    }
}
