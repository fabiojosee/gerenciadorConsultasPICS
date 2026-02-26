namespace AgendaPics.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem);
}
