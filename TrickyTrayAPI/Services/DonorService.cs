using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using WebApi.Data;

namespace TrickyTrayAPI.Services
{
    public class DonorService : IDonorService

    {
        private readonly IDonorRepository _donorrepository;
        private readonly ILogger<DonorService> _logger;

        public DonorService(IDonorRepository donorrepository, ILogger<DonorService> logger)
        {
            _donorrepository = donorrepository;
            _logger = logger;
        }
        private string shirshur(IEnumerable<Gift> gifts)
        {
            if (gifts == null || !gifts.Any())
                return string.Empty;

            return string.Join(", ", gifts.Select(g => g.Name));
        }
        public async Task<IEnumerable<GetDonorWithGiftsDTO>> GetAllDonors()
        {
            try
            {
                var donors = await _donorrepository.GetAllDonors();
                _logger.LogInformation("get donors");

                return donors.Select(donor =>
                 new GetDonorWithGiftsDTO
                 {
                     Id = donor.Id,
                     PhoneNumber = donor.PhoneNumber,
                     Email = donor.Email,
                     Name = donor.Name ?? string.Empty,
                     Gifts = (ICollection<GetGiftDTO>)donor.Gifts.Select(g => new GetGiftDTO() { Description = g.Description, Category = g.Category.Name, Name = g.Name }).ToList(),
                     GiftsString = shirshur(donor.Gifts)


                 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "cant get donors");
                throw;
            }
        }

        public async Task<GetDonorDTO?> GetDonorById(int id)
        {
            try
            {
                var donor = await _donorrepository.GetDonorById(id);
                _logger.LogInformation("get donor by id " + id);

                return new GetDonorDTO { Name = donor.Name, Email = donor.Email };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant get donor by id " + id);
                throw;
            }

        }

        public async Task<Donor> AddDonor(CreateDonorDTO donor)
        {
            try
            {
                var newDonor = await _donorrepository.AddDonor(donor);
                _logger.LogInformation("create donor " + newDonor.Id);
                return newDonor;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant create donor");
                throw;
            }

        }

        public async Task<GetDonorDTO> UpdateAsync(int id, CreateDonorDTO donor)
        {
            try
            {

                var updateDonor = await _donorrepository.UpdateAsync(id,donor);
                _logger.LogInformation("update donor " + id);
                return new GetDonorDTO() { Email= updateDonor.Email, Name= updateDonor.Name };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant update donor " + id);
                throw;
            }

        }

        public async Task<bool> DeleteDonor(int id)
        {
            try
            {

                var isSucceed = await _donorrepository.DeleteDonor(id);
                _logger.LogInformation("delete donor " + id);
                return isSucceed;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant delete donor " + id);
                throw;
            }

        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {

                var isSucceed = await _donorrepository.ExistsAsync(id);
                _logger.LogInformation("check Exists donor " + id);
                return isSucceed;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant check Exists donor " + id);
                throw;
            }
        }
        public async Task<GetDonorWithGiftsDTO?> GetDonorWithGiftsAsync(int donorId)
        {
            try
            {
                _logger.LogInformation("Getting donor with gifts for donorId {DonorId}", donorId);
                var donor = await _donorrepository.GetDonorWithGiftsByIdAsync(donorId);
                if (donor == null)
                {
                    _logger.LogWarning("Donor with id {DonorId} not found", donorId);
                    return null;
                }
             
                return new GetDonorWithGiftsDTO
                {
                    Id = donor.Id,
                    PhoneNumber = donor.PhoneNumber,
                    Email = donor.Email,
                    Name = donor.Name ?? string.Empty,
                    Gifts = (ICollection<GetGiftDTO>)donor.Gifts.Select(g => new GetGiftDTO() { Description = g.Description, Category = g.Category.Name, Name = g.Name }).ToList(),
                    GiftsString = shirshur(donor.Gifts)

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donor with gifts for donorId {DonorId}", donorId);
                throw;
            }
        }
        public async Task<IEnumerable<GetDonorWithGiftsDTO>> FilterDonorsAsync(string? name, string? email, string? giftName)
        {
            var donors = await _donorrepository.FilterDonorsAsync(name, email, giftName);

            return donors.Select(d => new GetDonorWithGiftsDTO
            {
                Name = d.Name,
                Gifts = (ICollection<GetGiftDTO>)d.Gifts.Select(g => new GetGiftDTO() { Description = g.Description,Category=g.Category.Name,Name=g.Name }).ToList()
            });
        }

    }
}
