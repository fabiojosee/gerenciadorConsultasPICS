﻿using gerenciadorConsultasPICS.Services.Interfaces;
using gerenciadorConsultasPICS.Utils;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

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
            var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = assunto,
                Body = mensagem,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            mailMessage.To.Add(destinatario);

            await client.SendMailAsync(mailMessage);
        }

        public string GerarTemplateEmail(string titulo, List<string> linhas)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<h1>{titulo}</h1>");
            sb.AppendLine("<p>");

            foreach (var linha in linhas)
            {
                sb.AppendLine($"{linha}<br>");
            }

            sb.AppendLine("</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }
}