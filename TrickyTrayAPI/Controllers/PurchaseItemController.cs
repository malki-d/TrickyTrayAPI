using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;



namespace TrickyTrayAPI.Controllers
{
    public class PurchaseItemController: ControllerBase
    {
        private readonly IPurchaseItemService _service;

        public PurchaseItemController(IPurchaseItemService service)
        {
            _service = service;
        }

        [HttpGet("{giftId}/purchase-items")]
        public async Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetPurchaseItemsForGift(int giftId)
        {
            var items = await _service.GetPurchaseItemsForGiftAsync(giftId);
            return Ok(items);
        }
    }
}
