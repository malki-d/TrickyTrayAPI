using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace TrickyTrayAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }        public async Task SendWinnerEmailAsync(string toEmail, string winnerName, string giftName)
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var senderEmail = _configuration["Email:SenderEmail"];
                var senderPassword = _configuration["Email:SenderPassword"];
                var senderName = _configuration["Email:SenderName"] ?? "Tricky Tray System";

                _logger.LogInformation($"Attempting to send email to {toEmail}");
                _logger.LogInformation($"SMTP Server: {smtpServer}, Port: {smtpPort}, Sender: {senderEmail}");

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    _logger.LogError("Email configuration is missing. Please configure Email settings in appsettings.json");
                    return;
                }

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, senderPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "🎉 מזל טוב! זכית במתנה!",
                    Body = GetEmailBody(winnerName, giftName),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"✅ Winner email successfully sent to {toEmail} for gift: {giftName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send email to {toEmail}. Error: {ex.Message}");
                // לא זורקים חריגה כדי שלא לעצור את תהליך ההגרלה
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var senderEmail = _configuration["Email:SenderEmail"];
                var senderPassword = _configuration["Email:SenderPassword"];
                var senderName = _configuration["Email:SenderName"] ?? "Tricky Tray System";

                _logger.LogInformation($"Attempting to send welcome email to {toEmail}");

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    _logger.LogError("Email configuration is missing. Please configure Email settings in appsettings.json");
                    return;
                }

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, senderPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "🎊 ברוכים הבאים למערכת Tricky Tray!",
                    Body = GetWelcomeEmailBody(userName),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"✅ Welcome email successfully sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send welcome email to {toEmail}. Error: {ex.Message}");
                // לא זורקים חריגה כדי שלא לעצור את תהליך ההרשמה
            }
        }

        private string GetEmailBody(string winnerName, string giftName)
        {
            return $@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }}
        .container {{ background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        h1 {{ color: #4CAF50; text-align: center; }}
        .gift-name {{ color: #FF5722; font-size: 24px; font-weight: bold; text-align: center; margin: 20px 0; }}
        p {{ font-size: 16px; line-height: 1.6; }}
        .footer {{ margin-top: 30px; text-align: center; color: #777; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>🎉 מזל טוב {winnerName}! 🎉</h1>
        <p>אנחנו שמחים להודיע לך שזכית בהגרלה!</p>
        <div class='gift-name'>המתנה שלך: {giftName}</div>
        <p>נא ליצור קשר עמנו כדי לאסוף את המתנה שלך.</p>
        <p>תודה על ההשתתxxx במכירת Tricky Tray שלנו!</p>
        <div class='footer'>
            <p>בברכה,<br/>צוות Tricky Tray</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetWelcomeEmailBody(string userName)
        {
            return $@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }}
        .container {{ background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        h1 {{ color: #2196F3; text-align: center; }}
        .welcome-box {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0; }}
        p {{ font-size: 16px; line-height: 1.6; color: #333; }}
        .features {{ background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .features li {{ margin: 10px 0; }}
        .footer {{ margin-top: 30px; text-align: center; color: #777; font-size: 14px; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='welcome-box'>
            <h1>🎊 ברוכים הבאים!</h1>
            <h2 style='margin: 0;'>{userName}</h2>
        </div>
        
        <p>שלום {userName},</p>
        <p>אנחנו שמחים לקבל אותך למערכת Tricky Tray שלנו!</p>
        
        <div class='features'>
            <h3>עכשיו תוכל:</h3>
            <ul>
                <li>🎁 לצxxx בכל המתנות המוצעות</li>
                <li>🎫 לרכוש כרטיסים למתנות שאתה מעוניין בהן</li>
                <li>🏆 להשתתף בהגרלות ולזכות בפרסים מדהימים</li>
                <li>📊 לעקוב אחר הרכישות שלך</li>
            </ul>
        </div>
        
        <p style='text-align: center;'>
            <a href='http://localhost:4200' class='button'>התחל לגלוש במערכת</a>
        </p>
        
        <p>בהצלחה והמון מזל בהגרלות!</p>
        
        <div class='footer'>
            <p>שאלות? אנחנו כאן בשבילך!<br/>
            בברכה,<br/>צוות Tricky Tray</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
