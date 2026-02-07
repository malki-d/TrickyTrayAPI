using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

public interface IPurchaseService
{
    Task<IEnumerable<UserResponseDTO>> GetAllAsync();
    Task<PurchaseRevenueDTO> GetTotalRevenueAsync();
    Task<Purchase> ProcessPurchaseAsync(int userId);
    Task<IEnumerable<UserPurchaseDto>> GetUserPurchasesLogic(int userId);

}