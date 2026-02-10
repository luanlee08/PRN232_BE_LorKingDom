using BLL.DTOs.Moderation;
using BLL.Interfaces.Moderation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BLL.Services.Moderation
{
    public class AiOmniModerationService : IAiOmniModerationService
    {
        private readonly HttpClient _http;
        private readonly ILogger<AiOmniModerationService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _model;

        public AiOmniModerationService(
            HttpClient http,
            IConfiguration config,
            ILogger<AiOmniModerationService> logger)
        {
            _http = http;
            _logger = logger;

            _apiKey = config["AiModeration:ApiKey"] ?? "";
            _endpoint = config["AiModeration:Endpoint"] ?? "";
            _model = config["AiModeration:Model"] ?? "omni-moderation-latest";

            _jsonOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<AiModerationResponse> AnalyzeAsync(ModerationRequest request)
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_endpoint))
                return Fallback("Missing config");

            try
            {
                var apiResponse = await CallApiAsync(request.ReviewText, request.ImageUrls);
                return MapResult(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "AI Moderation failed | AccountId={AccountId}, ProductId={ProductId}",
                    request.AccountId, request.ProductId);

                return Fallback("Exception occurred");
            }
        }

        private async Task<OmniModerationApiResponse> CallApiAsync(string text, List<string>? images)
        {
            var input = new List<object>
            {
                new { type = "text", text }
            };

            if (images != null)
            {
                foreach (var url in images.Take(3))
                {
                    input.Add(new
                    {
                        type = "image_url",
                        image_url = new { url }
                    });
                }
            }

            var payload = new { model = _model, input };

            var response = await _http.PostAsJsonAsync(_endpoint, payload, _jsonOptions);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OmniModerationApiResponse>(_jsonOptions)
                ?? throw new InvalidOperationException("Empty AI response");
        }

        private AiModerationResponse MapResult(OmniModerationApiResponse api)
        {
            var result = api.Results.FirstOrDefault();
            if (result == null) return Fallback("No result");

            return new AiModerationResponse
            {
                IsFlagged = result.Flagged,
                Categories = result.Categories,
                CategoryScores = result.CategoryScores
            };
        }

        private static AiModerationResponse Fallback(string reason) => new()
        {
            IsFlagged = false,
            Categories = new OmniCategories(),
            CategoryScores = new OmniCategoryScores()
        };
    }
}
