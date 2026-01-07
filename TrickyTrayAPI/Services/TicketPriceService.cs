using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using WebApi.Data;

namespace TrickyTrayAPI.Services
{
    public class TicketPriceService :ITicketPriceService
    {
        private readonly AppDbContext _context;

        public TicketPriceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetTicketPriceDTO> GetAsync()
        {
            var price = await _context.TicketPrices.FindAsync(1);
            return new GetTicketPriceDTO { Price = price.Price };
        }
        public async Task<GetTicketPriceDTO> UpdateAsync(int price)
        {
            if (_context.Purchases.Count() == 0)
            {
                var entity = await _context.TicketPrices.FirstAsync();
                entity.Price = price;
                await _context.SaveChangesAsync();
                return new GetTicketPriceDTO { Price = entity.Price };
            }
            else
            {
                throw new InvalidOperationException("Cannot update ticket price after purchases have been made.");
            }
        }
    }
}
