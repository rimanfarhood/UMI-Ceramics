using Microsoft.AspNetCore.Identity.UI.Services;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Task = System.Threading.Tasks.Task;

namespace UmiCeramics.Services;

public class BrevoEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public BrevoEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string email,
        string subject,
        string htmlMessage)
    {
        var apiKey = _configuration["Brevo:ApiKey"];

        Configuration.Default.ApiKey["api-key"] = apiKey;

        var apiInstance = new TransactionalEmailsApi();

        var sender = new SendSmtpEmailSender(
            "Umi Ceramics",
            "noreply@umiceramics.se"
        );

        var receivers = new List<SendSmtpEmailTo>
        {
            new SendSmtpEmailTo(email)
        };

        var emailMessage = new SendSmtpEmail(
            sender,
            receivers,
            null,
            null,
            htmlMessage,
            null,
            subject
        );

        await apiInstance.SendTransacEmailAsync(emailMessage);
    }
}