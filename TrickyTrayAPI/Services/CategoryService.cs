using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;

namespace TrickyTrayAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                var categories = await _repository.GetAllAsync();
                _logger.LogInformation("Successfully retrieved all categories");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                throw;
            }
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category != null)
                {
                    _logger.LogInformation("Successfully retrieved category with id {CategoryId}", id);
                }
                else
                {
                    _logger.LogWarning("Category with id {CategoryId} not found", id);
                }
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting category with id {id}");
                throw;
            }
        }

        public async Task<Category?> AddAsync(String name)
        {
            try
            {
                var added = await _repository.AddAsync(name);
                _logger.LogInformation("Successfully added category with id {CategoryId}", added.Id);
                return added;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error adding category with name {CategoryName}", name);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, string name)
        {
            try
            {
                var result = await _repository.UpdateAsync(id, name);
                if (result)
                {
                    _logger.LogInformation("Successfully updated category with id {CategoryId}", id);
                }
                else
                {
                    _logger.LogWarning("Category with id {CategoryId} not found for update", id);
                }
                return result;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating category with id {CategoryId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category with id {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation("Successfully deleted category with id {CategoryId}", id);
                }
                else
                {
                    _logger.LogWarning("Category with id {CategoryId} not found for deletion", id);
                }
                return result;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting category with id {CategoryId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category with id {id}");
                throw;
            }
        }
    }
}