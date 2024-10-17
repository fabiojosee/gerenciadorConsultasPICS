namespace gerenciadorConsultasPICS.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string destinatario, string assunto, string mensagem);
        public string GerarTemplateEmail(string titulo, List<string> linhas);
    }
}
