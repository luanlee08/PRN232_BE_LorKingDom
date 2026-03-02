using BLL.DTOs.Statistics;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/statistics")]
    public class AStatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ILogger<AStatisticsController> _logger;

        public AStatisticsController(IStatisticsService statisticsService, ILogger<AStatisticsController> logger)
        {
            _statisticsService = statisticsService;
            _logger = logger;
        }

        /// <summary>
        /// Revenue statistics — Admin only.
        /// </summary>
        [HttpGet("revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueStatistics(
            [FromQuery] string? period = "month",
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var query = new RevenueStatisticsQuery
            {
                Period = period ?? "month",
                From = from,
                To = to
            };

            var result = await _statisticsService.GetRevenueStatisticsAsync(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Product statistics — Admin and Warehouse staff.
        /// </summary>
        [HttpGet("products")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> GetProductStatistics(
            [FromQuery] int topN = 10,
            [FromQuery] int lowStockThreshold = 10)
        {
            var query = new ProductStatisticsQuery
            {
                TopN = topN,
                LowStockThreshold = lowStockThreshold
            };

            var result = await _statisticsService.GetProductStatisticsAsync(query);
            return StatusCode(result.Status, result);
        }
    }
}
