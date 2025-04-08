using gerenciadorConsultasPICS.Services.Interfaces;
using gerenciadorConsultasPICS.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

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
            try
            {
                var token = await ObterTokenAcesso();

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", destinatario));
                message.Subject = assunto;
                message.Body = new TextPart("html") { Text = mensagem };

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(new SaslMechanismOAuth2(_smtpSettings.SenderEmail, token));
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        /// <summary>
        /// É necessário adicionar o arquivo 'credenciais.json', obtido no Google Cloud, à raiz do projeto.
        /// Observação: Caso o aplicativo não esteja publicado em produção no Google, será solicitado o login de um usuário de teste via navegador.
        /// </summary>
        /// <returns></returns>
        private async Task<string> ObterTokenAcesso()
        {
            using var stream = new FileStream("credenciais.json", FileMode.Open, FileAccess.Read);
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { "https://mail.google.com/" },
                _smtpSettings.SenderEmail,
                CancellationToken.None,
                new FileDataStore("token.json", true)
            );

            return credential.Token.AccessToken;
        }
    }
}
