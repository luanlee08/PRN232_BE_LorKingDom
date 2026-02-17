using BLL.DTOs.Orders;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    public class AOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

    }
}