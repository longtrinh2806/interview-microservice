using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderServices.Dtos;
using OrderServices.Services;

namespace OrderServices.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult CreateOrder(List<OrderDetailDto> request)
        {
            var result = _orderService.Create(request);

            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

    }
}
