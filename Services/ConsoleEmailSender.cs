using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinTrack.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogWarning("==== EMAIL SIMULADO (DEV) ====");
            _logger.LogWarning($"Para: {email}");
            _logger.LogWarning($"Assunto: {subject}");
            _logger.LogWarning("Conteúdo:");
            _logger.LogWarning(htmlMessage);
            _logger.LogWarning("==============================");

            return Task.CompletedTask;
        }
    }
}
