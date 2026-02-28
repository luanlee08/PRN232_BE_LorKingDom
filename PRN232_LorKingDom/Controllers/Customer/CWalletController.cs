using BLL.DTOs.Wallet;
using BLL.Interfaces.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer;

[ApiController]
[Route("api/[controller]")]
public class CWalletController : ControllerBase
{
    private readonly IWalletQueryService _walletQueryService;
    private readonly IWalletCommandService _walletCommandService;

    public CWalletController(
        IWalletQueryService walletQueryService,
        IWalletCommandService walletCommandService)
    {
        _walletQueryService = walletQueryService;
        _walletCommandService = walletCommandService;
    }

    private int GetAccountId()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(accountIdClaim, out var accountId) ? accountId : 0;
    }

    /// <summary>
    /// Get wallet balance and info
    /// </summary>
    [Authorize]
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var accountId = GetAccountId();
        if (accountId == 0)
            return Unauthorized(new { message = "Unauthorized" });

        var result = await _walletQueryService.GetWalletAsync(accountId);
        return StatusCode(result.Status, result);
    }

    /// <summary>
    /// Get wallet transaction history with pagination and filters
    /// </summary>
    [Authorize]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? type = null,
        [FromQuery] string? direction = null)
    {
        var accountId = GetAccountId();
        if (accountId == 0)
            return Unauthorized(new { message = "Unauthorized" });

        var result = await _walletQueryService.GetTransactionHistoryAsync(
            accountId, page, pageSize, type, direction);
        return StatusCode(result.Status, result);
    }

    /// <summary>
    /// Initiate wallet top-up via payment gateway
    /// </summary>
    [Authorize]
    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequestDTO request)
    {
        var accountId = GetAccountId();
        if (accountId == 0)
            return Unauthorized(new { message = "Unauthorized" });

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var result = await _walletCommandService.InitiateTopUpAsync(accountId, request, ipAddress);
        return StatusCode(result.Status, result);
    }

    /// <summary>
    /// VNPay callback after top-up payment (redirect from VNPay)
    /// </summary>
    [HttpGet("topup/callback/vnpay")]
    public async Task<IActionResult> VNPayCallback()
    {
        var queryParams = Request.Query.ToDictionary(
            x => x.Key,
            x => x.Value.ToString());

        var result = await _walletCommandService.HandleVNPayCallbackAsync(queryParams);

        // Redirect to frontend wallet page with result
        var frontendUrl = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["AppSettings:FrontendUrl"] ?? "http://localhost:3000";

        var status = result.Data?.Status == "Completed" ? "success" : "failed";
        var amount = result.Data?.Amount.ToString("F0") ?? "0";
        return Redirect($"{frontendUrl}/profile/wallet?topup={status}&amount={amount}");
    }

    /// <summary>
    /// MoMo IPN callback after top-up payment
    /// </summary>
    [HttpPost("topup/callback/momo")]
    public async Task<IActionResult> MoMoCallback()
    {
        var queryParams = Request.Query.ToDictionary(
            x => x.Key,
            x => x.Value.ToString());

        // MoMo also sends params in body for IPN
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            foreach (var item in form)
            {
                queryParams[item.Key] = item.Value.ToString();
            }
        }

        var result = await _walletCommandService.HandleMoMoCallbackAsync(queryParams);
        return StatusCode(result.Status, result);
    }

    /// <summary>
    /// Sepay webhook after top-up payment
    /// </summary>
    [HttpPost("topup/webhook/sepay")]
    public async Task<IActionResult> SepayWebhook()
    {
        var body = await new StreamReader(Request.Body).ReadToEndAsync();

        // Parse Sepay webhook body to dictionary
        var queryParams = new Dictionary<string, string>();
        try
        {
            var json = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(body);
            if (json != null)
            {
                foreach (var kvp in json)
                {
                    queryParams[kvp.Key] = kvp.Value.ToString();
                }
            }
        }
        catch
        {
            return BadRequest(new { message = "Invalid request body" });
        }

        var result = await _walletCommandService.HandleSepayCallbackAsync(queryParams);
        return StatusCode(result.Status, result);
    }
}
