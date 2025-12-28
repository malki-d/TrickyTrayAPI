using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.DTOs;
using WebApi.Data;
using System.Drawing;

namespace TrickyTrayAPI.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            return await _context.CartItems.Include(c => c.User).Include(c => c.Gift).ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(int id)
        {
            return await _context.CartItems.Include(c => c.User).Include(c => c.Gift)
                .Where(ci => ci.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<CartItem> AddAsync(CreateCartItemDTO cartitem)
        {
            var newcartitem = new CartItem { UserId = cartitem.UserId, GiftId = cartitem.GiftId, Quantity = cartitem.Quantity };
            _context.CartItems.Add(newcartitem);
            await _context.SaveChangesAsync();
            return newcartitem;
        }
        public async Task<bool> UpdateAsync(int id, UpdateCartItemDTO cartitem)
        {
            try
            {
                var existingCartItem = await _context.CartItems.FindAsync(id);
                _context.CartItems.Update(existingCartItem);
                await _context.SaveChangesAsync();
                return true; ;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cartitem = await _context.CartItems.FindAsync(id);
            if (cartitem == null)
                return false;

            _context.CartItems.Remove(cartitem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}