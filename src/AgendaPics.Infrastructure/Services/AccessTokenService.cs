using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using Azure.Core;
using System.Text;

namespace AgendaPics.Infrastructure.Services;

public class AccessTokenService : IAccessTokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecureRandomGenerator _randomGenerator;
    private readonly IEmailService _emailService;

    private static readonly TimeSpan TokenValidity = TimeSpan.FromMinutes(30);

    public AccessTokenService(
        IUnitOfWork unitOfWork,
        ISecureRandomGenerator randomGenerator,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _randomGenerator = randomGenerator;
        _emailService = emailService;
    }

    public async Task<string> GerarTokenAsync(string cpf, string email)
    {
        var token = _randomGenerator.GenerateToken(4);

        var tokenAcesso = TokenAcesso.Criar(cpf, token, TokenValidity);

        await _unitOfWork.TokensAcesso.AdicionarAsync(tokenAcesso);
        await _unitOfWork.CommitAsync();

        await EnviarTokenPorEmailAsync(email, token);

        return token;
    }

    public async Task<bool> ValidarTokenAsync(string cpf, string token)
    {
        var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());
        var tokenAcesso = await _unitOfWork.TokensAcesso.ObterPorCpfETokenAsync(cpfLimpo, token);
        if (tokenAcesso == null || !tokenAcesso.IsValido())
            return false;

        tokenAcesso.MarcarComoUtilizado();
        _unitOfWork.TokensAcesso.Atualizar(tokenAcesso);
        await _unitOfWork.CommitAsync();

        return true;
    }

    private async Task EnviarTokenPorEmailAsync(string email, string token)
    {
        var linhas = new List<string>
        {
            "Você solicitou acesso aos seus atendimentos.",
            "",
            $"Seu código de acesso é: <strong>{token}</strong>",
            "",
            "Este código é válido por 30 minutos.",
            "",
            "Se você não solicitou este acesso, ignore este e-mail."
        };

        var mensagem = GerarTemplateEmail("Código de Acesso - Agenda PICS", linhas);

        await _emailService.EnviarEmailAsync(email, "Agenda PICS - Código de Acesso", mensagem);
    }

    private static string GerarTemplateEmail(string titulo, List<string> linhas)
    {
        var sb = new StringBuilder();
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
