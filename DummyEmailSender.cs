using EFGetStarted.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace EFGetStarted
{


    public class DummyEmailSender : IEmailSender<User>
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // This is a dummy implementation that does nothing
            return Task.CompletedTask;
        }

        Task IEmailSender<User>.SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            // This is a dummy implementation that does nothing
            return Task.CompletedTask;
        }

        Task IEmailSender<User>.SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            // This is a dummy implementation that does nothing
            return Task.CompletedTask;
        }

        Task IEmailSender<User>.SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            // This is a dummy implementation that does nothing
            return Task.CompletedTask;
        }
    }
}
