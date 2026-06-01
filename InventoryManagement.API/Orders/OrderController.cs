using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Orders
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateOrder()
        {
            return Created();
        }
    }
}
