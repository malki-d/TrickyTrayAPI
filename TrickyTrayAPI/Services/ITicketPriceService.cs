using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Services
{
    public interface ITicketPriceService
    {
        Task<GetTicketPriceDTO> GetAsync();
        Task<GetTicketPriceDTO> UpdateAsync(int price);
    }
}