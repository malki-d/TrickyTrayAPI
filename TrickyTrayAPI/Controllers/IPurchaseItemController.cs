using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Controllers
{
    public interface IPurchaseItemController
    {
        Task<ActionResult<GetPurchaseItemDTO>> Create([FromBody] CreatePurchaseItemDTO purchaseItem);
        Task<ActionResult> Delete(int id);
        Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetAll();
        Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetByGift(int giftId);
        Task<ActionResult<GetPurchaseItemDTO>> GetById(int id);
        Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetByUser(int userId);
        Task<ActionResult<GetPurchaseItemDTO>> Update(int id, [FromBody] UpdatePurchaseItemDTO purchaseItem);
    }
}