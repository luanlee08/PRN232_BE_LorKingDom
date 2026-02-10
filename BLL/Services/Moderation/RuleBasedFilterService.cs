using BLL.DTOs.Moderation;
using BLL.Interfaces.Moderation;
using System.Text.RegularExpressions;

namespace BLL.Services.Moderation
{
    public class RuleBasedFilterService : IRuleBasedFilterService
    {
        // === TỪ CẤM TIẾNG VIỆT & TIẾNG ANH ===
        private readonly HashSet<string> _bannedWords = new(StringComparer.OrdinalIgnoreCase)
        {
            // Từ tục tĩu tiếng Việt
            "đồ chó", "đồ khốn", "đồ ngu", "đồ điên", "đồ khùng",
            "lồn", "cặc", "buồi", "đĩ", "đéo", "vãi", "đụ", "địt",
            "clgt", "vcl", "vkl", "dmm", "dcm", "cc", "dm", "đm",
            
            // Từ tục tĩu tiếng Anh
            "fuck", "fucking", "fucked", "fucker", "motherfucker",
            "shit", "bitch", "ass", "asshole", "bastard", "damn",
            "cunt", "dick", "cock", "pussy", "whore", "slut",
            "nigger", "nigga", "faggot", "retard",
            
            // Từ xúc phạm tiếng Việt
            "ngu ngốc", "đần độn", "tởm lợm", "rác rưởi", "phế phẩm",
            "loser", "thất bại", "kém cỏi", "vô dụng",
            
            // Từ spam thương mại
            "mua ngay", "giảm giá", "khuyến mãi", "liên hệ",
            "inbox", "zalo", "facebook", "click here", "buy now"
        };

        // === PATTERN PHÁT HIỆN ===
        private readonly List<Regex> _suspiciousPatterns = new()
        {
            // Email
            new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled),
            
            // Số điện thoại Việt Nam
            new Regex(@"\b(0|\+84)[0-9]{9,10}\b", RegexOptions.Compiled),
            
            // URL
            new Regex(@"(https?://|www\.)[^\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            
            // Spam ký tự lặp (aaaaa, !!!!!!)
            new Regex(@"(.)\1{4,}", RegexOptions.Compiled),
            
            // Quá nhiều chữ in hoa liên tiếp
            new Regex(@"\b[A-Z]{10,}\b", RegexOptions.Compiled)
        };

        public Task<RuleBasedResponse> CheckAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult(new RuleBasedResponse
                {
                    IsViolated = true,
                    ViolationReasons = new List<string> { "Nội dung review không được để trống" }
                });
            }

            var result = new RuleBasedResponse();
            var violations = new List<string>();

            // === 1. CHECK TỪ CẤM ===
            foreach (var word in _bannedWords)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    violations.Add($"Phát hiện từ ngữ không phù hợp: '{word}'");
                    result.IsViolated = true;
                }
            }

            // === 2. CHECK PATTERN ===
            foreach (var pattern in _suspiciousPatterns)
            {
                if (pattern.IsMatch(text))
                {
                    violations.Add($"Phát hiện nội dung spam/link không cho phép");
                    result.IsViolated = true;
                    break; // Chỉ cần 1 violation là đủ
                }
            }

            // === 3. CHECK ĐỘ DÀI ===
            if (text.Length < 10)
            {
                violations.Add("Review quá ngắn (tối thiểu 10 ký tự)");
                result.IsViolated = true;
            }
            else if (text.Length > 500)
            {
                violations.Add("Review quá dài (tối đa 500 ký tự)");
                result.IsViolated = true;
            }

            result.ViolationReasons = violations;
            return Task.FromResult(result);
        }
    }
}
