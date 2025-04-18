using gerenciadorConsultasPICS.Services.Interfaces;
using gerenciadorConsultasPICS.Utils;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace gerenciadorConsultasPICS.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string mensagem)
        {
            MailjetClient client = new(_smtpSettings.ApiKey, _smtpSettings.ApiSecret);

            MailjetRequest request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            }
            .Property(Send.Messages, new JArray {
                new JObject {
                    {"From", new JObject {
                        {"Email", _smtpSettings.SenderEmail},
                        {"Name", _smtpSettings.SenderName}
                    }},
                    {"To", new JArray {
                        new JObject {
                            {"Email", destinatario}
                        }
                    }},
                    {"Subject", assunto},
                    {"HTMLPart", mensagem}
                }
            });

            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
                Console.WriteLine("E-mail enviado com sucesso!");
            else
                Console.WriteLine($"Erro ao enviar e-mail: {response.StatusCode}\n{response.GetErrorMessage()}");
        }
    }
}
