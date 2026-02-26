using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Domain.ValueObjects;

namespace AgendaPics.Application.Features.Autenticacao.Commands;

public record EnviarCodigoRecuperacaoCommand(string Email) : IRequest<Result<EnviarCodigoRecuperacaoResult>>;

public record EnviarCodigoRecuperacaoResult(string Codigo, int IdInstituicao);

public class EnviarCodigoRecuperacaoCommandHandler : IRequestHandler<EnviarCodigoRecuperacaoCommand, Result<EnviarCodigoRecuperacaoResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ISecureRandomGenerator _randomGenerator;

    public EnviarCodigoRecuperacaoCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ISecureRandomGenerator randomGenerator)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _randomGenerator = randomGenerator;
    }

    public async Task<Result<EnviarCodigoRecuperacaoResult>> Handle(
        EnviarCodigoRecuperacaoCommand request,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result<EnviarCodigoRecuperacaoResult>.Failure(emailResult.Error);

        var instituicao = await _unitOfWork.Instituicoes.ObterPorEmailAsync(request.Email, cancellationToken);

        if (instituicao == null)
            return Result<EnviarCodigoRecuperacaoResult>.Failure("E-mail não encontrado");

        var codigo = _randomGenerator.GenerateCode(6);

        var mensagem = GerarTemplateEmail("Recuperação de Senha",
        [
            $"Código de recuperação: <strong>{codigo}</strong>",
            "Este código expira em 15 minutos.",
            "Se você não solicitou a recuperação de senha, ignore este e-mail."
        ]);

        var enviado = await _emailService.EnviarEmailAsync(
            request.Email,
            "Agenda PICS - Recuperação de Senha",
            mensagem);

        if (!enviado)
            return Result<EnviarCodigoRecuperacaoResult>.Failure("Falha ao enviar e-mail");

        return Result<EnviarCodigoRecuperacaoResult>.Success(
            new EnviarCodigoRecuperacaoResult(codigo, instituicao.IdInstituicao));
    }

    private static string GerarTemplateEmail(string titulo, List<string> linhas)
    {
        var sb = new System.Text.StringBuilder();
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
