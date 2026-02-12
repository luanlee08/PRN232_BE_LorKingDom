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
    public class ModerationLayer2Service : IModerationLayer2Service
    {
        private readonly HttpClient _http;
        private readonly ILogger<ModerationLayer2Service> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _model;

        public ModerationLayer2Service(
            HttpClient http,
            IConfiguration config,
            ILogger<ModerationLayer2Service> logger)
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

        public async Task<ModerationLayer2Result> AnalyzeAsync(ReviewModerationRequest request)
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

        private async Task<OpenAiModerationDto> CallApiAsync(string text, List<string>? images)
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

            return await response.Content.ReadFromJsonAsync<OpenAiModerationDto>(_jsonOptions)
                ?? throw new InvalidOperationException("Empty AI response");
        }

        private ModerationLayer2Result MapResult(OpenAiModerationDto api)
        {
            var result = api.Results.FirstOrDefault();
            if (result == null) return Fallback("No result");

            return new ModerationLayer2Result
            {
                IsFlagged = result.Flagged,
                Categories = result.Categories,
                CategoryScores = result.CategoryScores
            };
        }

        private static ModerationLayer2Result Fallback(string reason) => new()
        {
            IsFlagged = false,
            Categories = new OmniCategories(),
            CategoryScores = new OmniCategoryScores()
        };
    }
}
