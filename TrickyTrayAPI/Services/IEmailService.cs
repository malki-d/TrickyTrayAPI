namespace TrickyTrayAPI.Services
{
    public interface IEmailService
    {
        Task SendWinnerEmailAsync(string toEmail, string winnerName, string giftName);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
    }
}
