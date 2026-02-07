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
            var isexist = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == cartitem.UserId && c.GiftId == cartitem.GiftId);
            if (isexist != null)
            {
                // You may want to handle the case where the item already exists.
                // For now, just return the existing item.
                isexist.Quantity += cartitem.Quantity;
                await _context.SaveChangesAsync();

                return await _context.CartItems
                    .Include(c => c.User)
                    .Include(c => c.Gift)
                    .FirstOrDefaultAsync(c => c.Id == isexist.Id);
            }

            var newcartitem = new CartItem { UserId = cartitem.UserId, GiftId = cartitem.GiftId, Quantity = cartitem.Quantity };
            _context.CartItems.Add(newcartitem);


            await _context.SaveChangesAsync();
            return await _context.CartItems
                    .Include(c => c.User)
                    .Include(c => c.Gift)
                    .FirstOrDefaultAsync(c => c.Id == newcartitem.Id);
        }
        public async Task<IEnumerable<CartItem>> GetAllUserCartAsync(int id)
        {
            return await _context.CartItems.Include(c => c.User).Include(c => c.Gift).ThenInclude(c=>c.Category)
                .Where(ci => ci.UserId == id)
                .ToListAsync();
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