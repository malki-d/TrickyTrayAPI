using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _repository;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(IPurchaseRepository repository, ILogger<PurchaseService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            try
            {
                var purchases = await _repository.GetAllAsync();
                var result = purchases.Select(p => new UserResponseDTO
                {
                    FirstName=p.User.FirstName,
                    LastName=p.User.LastName,
                    Email =p.User.Email,
                    Phone=p.User.PhoneNumber,                    
                });
                _logger.LogInformation("Successfully retrieved all purchases");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all purchases");
                throw;
            }
        }

        public async Task<GetPurchaseDTO?> GetByIdAsync(int id)
        {
            try
            {
                var purchase = await _repository.GetByIdAsync(id);
                if (purchase == null)
                {
                    _logger.LogWarning("Purchase with id {PurchaseId} not found", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved purchase with id {PurchaseId}", id);
                return new GetPurchaseDTO
                {
                    Date = purchase.Date,
                    UserId = purchase.UserId,
                    Price = purchase.Price
                    // Map other properties as needed
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting purchase with id {id}");
                throw;
            }
        }

        public async Task<GetPurchaseDTO> AddAsync(CreatePurchaseDTO dto)
        {
            try
            {
                var purchase = new Purchase
                {
                    Date = dto.Date,
                    UserId = dto.UserId,
                    Price = dto.Price
                    // Map other properties as needed
                };
                var added = await _repository.AddAsync(purchase);
                _logger.LogInformation("Successfully added purchase with id {PurchaseId}", added.Id);
                return new GetPurchaseDTO
                {
                    Date = added.Date,
                    UserId = added.UserId,
                    Price = added.Price
                    // Map other properties as needed
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding purchase");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, CreatePurchaseDTO dto)
        {
            try
            {
                var purchase = new Purchase
                {
                    Id = id,
                    Date = dto.Date,
                    UserId = dto.UserId,
                    Price = dto.Price
                    // Map other properties as needed
                };
                var updated = await _repository.UpdateAsync(purchase);
                if (updated)
                {
                    _logger.LogInformation("Successfully updated purchase with id {PurchaseId}", id);
                }
                else
                {
                    _logger.LogWarning("Purchase with id {PurchaseId} not found for update", id);
                }
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating purchase with id {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Successfully deleted purchase with id {PurchaseId}", id);
                }
                else
                {
                    _logger.LogWarning("Purchase with id {PurchaseId} not found for deletion", id);
                }
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting purchase with id {id}");
                throw;
            }
        }

        public async Task<PurchaseRevenueDTO> GetTotalRevenueAsync()
        {
            try
            {
                var revenue = await _repository.GetTotalRevenueAsync();
                _logger.LogInformation("Retrieved total revenue");
                return revenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total revenue");
                throw;
            }
        }
    }
}