using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace DAL.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string userName, string otpCode)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("LorKingdom", _configuration["Email:Username"]));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "Xác Thực Tài Khoản LorKingdom - Mã OTP";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GetOtpEmailTemplate(userName, otpCode)
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_configuration["Email:Host"],
                    int.Parse(_configuration["Email:Port"]!),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("LorKingdom", _configuration["Email:Username"]));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "Chào Mừng Đến Với LorKingdom!";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GetWelcomeEmailTemplate(userName)
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_configuration["Email:Host"],
                    int.Parse(_configuration["Email:Port"]!),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending welcome email: {ex.Message}");
                return false;
            }
        }

        private string GetOtpEmailTemplate(string userName, string otpCode)
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body { font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0; }
        .container { max-width: 600px; margin: 40px auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 16px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #FF6B35 0%, #E55A24 100%); padding: 40px 20px; text-align: center; }
        .header h1 { color: #ffffff; margin: 0; font-size: 32px; font-weight: 700; }
        .crown { font-size: 48px; margin-bottom: 10px; }
        .content { padding: 40px 30px; }
        .greeting { font-size: 18px; color: #222222; margin-bottom: 20px; }
        .otp-box { background: #FFF5F0; border: 2px dashed #FF6B35; border-radius: 8px; padding: 30px; text-align: center; margin: 30px 0; }
        .otp-label { font-size: 14px; color: #666666; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 1px; }
        .otp-code { font-size: 36px; font-weight: 700; color: #FF6B35; letter-spacing: 8px; font-family: 'Courier New', monospace; }
        .info-box { background: #F5F5F5; border-left: 4px solid #FF6B35; padding: 15px 20px; margin: 20px 0; border-radius: 4px; }
        .info-box p { margin: 5px 0; color: #666666; font-size: 14px; }
        .warning { color: #E55A24; font-weight: 600; }
        .footer { background: #F5F5F5; padding: 30px; text-align: center; border-top: 1px solid #e8e8e8; }
        .footer p { color: #666666; font-size: 13px; margin: 5px 0; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""crown"">&#128081;</div>
            <h1>LorKingdom</h1>
        </div>
        
        <div class=""content"">
            <p class=""greeting"">Xin chào <strong>" + userName + @"</strong>,</p>
            
            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>LorKingdom</strong> - Vương quốc đồ chơi thần kỳ!</p>
            
            <div class=""otp-box"">
                <div class=""otp-label"">Mã Xác Thực OTP</div>
                <div class=""otp-code"">" + otpCode + @"</div>
            </div>
            
            <div class=""info-box"">
                <p><strong>&#9200; Lưu ý quan trọng:</strong></p>
                <p>&#8226; Mã OTP có hiệu lực trong <span class=""warning"">5 phút</span></p>
                <p>&#8226; Không chia sẻ mã này với bất kỳ ai</p>
                <p>&#8226; Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email</p>
            </div>
            
            <p style=""color: #666666; font-size: 14px; margin-top: 30px;"">
                Sau khi xác thực thành công, bạn sẽ có thể khám phá hàng ngàn sản phẩm đồ chơi chất lượng cao với giá ưu đãi!
            </p>
        </div>
        
        <div class=""footer"">
            <p><strong>LorKingdom - Vương Quốc Đồ Chơi</strong></p>
            <p>Email: lorkingdom.service@gmail.com</p>
            <p style=""margin-top: 20px; color: #999999; font-size: 12px;"">
                &copy; 2026 LorKingdom. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetWelcomeEmailTemplate(string userName)
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body { font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0; }
        .container { max-width: 600px; margin: 40px auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 16px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #FF6B35 0%, #E55A24 100%); padding: 40px 20px; text-align: center; position: relative; }
        .crown { font-size: 64px; margin-bottom: 15px; position: relative; z-index: 1; }
        .header h1 { color: #ffffff; margin: 0; font-size: 36px; font-weight: 700; position: relative; z-index: 1; }
        .content { padding: 40px 30px; }
        .welcome-message { font-size: 24px; color: #FF6B35; font-weight: 700; text-align: center; margin-bottom: 20px; }
        .greeting { font-size: 16px; color: #222222; line-height: 1.6; }
        .benefits { background: #FFF5F0; border-radius: 8px; padding: 25px; margin: 25px 0; }
        .benefits h3 { color: #FF6B35; margin-top: 0; font-size: 18px; }
        .benefit-item { display: flex; align-items: start; margin: 15px 0; }
        .benefit-icon { font-size: 24px; margin-right: 12px; }
        .benefit-text { flex: 1; color: #666666; }
        .cta-button { display: block; background: #FF6B35; color: #ffffff; text-align: center; padding: 15px 30px; border-radius: 8px; text-decoration: none; font-weight: 600; margin: 30px 0; }
        .footer { background: #F5F5F5; padding: 30px; text-align: center; border-top: 1px solid #e8e8e8; }
        .footer p { color: #666666; font-size: 13px; margin: 5px 0; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""crown"">&#128081;</div>
            <h1>LorKingdom</h1>
        </div>
        
        <div class=""content"">
            <div class=""welcome-message"">&#127881; Chào Mừng Đến Với LorKingdom!</div>
            
            <p class=""greeting"">Xin chào <strong>" + userName + @"</strong>,</p>
            
            <p class=""greeting"">
                Chúc mừng bạn đã trở thành thành viên của <strong>LorKingdom</strong> - vương quốc đồ chơi thần kỳ nơi niềm vui không có giới hạn!
            </p>
            
            <div class=""benefits"">
                <h3>&#127873; Đặc Quyền Thành Viên</h3>
                
                <div class=""benefit-item"">
                    <div class=""benefit-icon"">&#128142;</div>
                    <div class=""benefit-text"">
                        <strong>Giảm giá exclusive</strong><br>
                        Ưu đãi đặc biệt dành riêng cho thành viên
                    </div>
                </div>
                
                <div class=""benefit-item"">
                    <div class=""benefit-icon"">&#128666;</div>
                    <div class=""benefit-text"">
                        <strong>Giao hàng nhanh miễn phí</strong><br>
                        Nhận hàng nhanh chóng ngay tại nhà
                    </div>
                </div>
                
                <div class=""benefit-item"">
                    <div class=""benefit-icon"">&#11088;</div>
                    <div class=""benefit-text"">
                        <strong>Tích điểm thưởng</strong><br>
                        Mỗi đơn hàng đều được tích điểm để đổi quà
                    </div>
                </div>
                
                <div class=""benefit-item"">
                    <div class=""benefit-icon"">&#128276;</div>
                    <div class=""benefit-text"">
                        <strong>Thông báo ưu đãi</strong><br>
                        Cập nhật deals hot và flash sale mới nhất
                    </div>
                </div>
            </div>
            
            <a href=""#"" class=""cta-button"">&#128717; Khám Phá Ngay</a>
            
            <p style=""color: #666666; font-size: 14px; text-align: center; margin-top: 30px;"">
                Hãy bắt đầu hành trình mua sắm tuyệt vời cùng chúng tôi!
            </p>
        </div>
        
        <div class=""footer"">
            <p><strong>LorKingdom - Vương Quốc Đồ Chơi</strong></p>
            <p>Email: lorkingdom.service@gmail.com</p>
            <p style=""margin-top: 20px; color: #999999; font-size: 12px;"">
                &copy; 2026 LorKingdom. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}