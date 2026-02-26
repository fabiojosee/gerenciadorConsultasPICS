using AgendaPics.Application.Common.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace AgendaPics.Infrastructure.Services;

public class SmtpSettings
{
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem)
    {
        try
        {
            var client = new MailjetClient(_smtpSettings.ApiKey, _smtpSettings.ApiSecret);

            var request = new MailjetRequest
            {
                Resource = Send.Resource
            }
            .Property(Send.FromEmail, _smtpSettings.SenderEmail)
            .Property(Send.FromName, _smtpSettings.SenderName)
            .Property(Send.Subject, assunto)
            .Property(Send.HtmlPart, mensagem)
            .Property(Send.Recipients, new JArray { new JObject { { "Email", destinatario } } });

            var response = await client.PostAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
