using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Enums;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Domain.ValueObjects;

namespace AgendaPics.Application.Features.Instituicoes.Commands;

public record CriarInstituicaoCommand(
    string Nome,
    string? Descricao,
    short IdEstado,
    int IdCidade,
    string Cnpj,
    string Cep,
    string Email,
    TimeSpan HorarioInicioAtendimento,
    TimeSpan HorarioFimAtendimento) : IRequest<Result<CriarInstituicaoResult>>;

public record CriarInstituicaoResult(int IdInstituicao, string SenhaGerada);

public class CriarInstituicaoCommandHandler : IRequestHandler<CriarInstituicaoCommand, Result<CriarInstituicaoResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISecureRandomGenerator _randomGenerator;
    private readonly IEmailService _emailService;

    public CriarInstituicaoCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ISecureRandomGenerator randomGenerator,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _randomGenerator = randomGenerator;
        _emailService = emailService;
    }

    public async Task<Result<CriarInstituicaoResult>> Handle(CriarInstituicaoCommand request, CancellationToken cancellationToken)
    {
        var cnpjResult = CNPJ.Create(request.Cnpj);
        if (cnpjResult.IsFailure)
            return Result<CriarInstituicaoResult>.Failure(cnpjResult.Error);

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result<CriarInstituicaoResult>.Failure(emailResult.Error);

        var instituicaoExistente = await _unitOfWork.Instituicoes.ObterPorEmailAsync(request.Email, cancellationToken);
        if (instituicaoExistente != null)
            return Result<CriarInstituicaoResult>.Failure("E-mail já cadastrado");

        var instituicaoCnpjExistente = await _unitOfWork.Instituicoes.ObterPorCnpjAsync(cnpjResult.Value.Value, cancellationToken);
        if (instituicaoCnpjExistente != null)
            return Result<CriarInstituicaoResult>.Failure("CNPJ já cadastrado");

        var instituicao = Instituicao.Criar(
            request.Nome,
            request.Descricao,
            request.IdCidade,
            cnpjResult.Value.Value,
            request.Cep,
            emailResult.Value.Value,
            request.HorarioInicioAtendimento,
            request.HorarioFimAtendimento);

        await _unitOfWork.Instituicoes.AdicionarAsync(instituicao, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var senhaGerada = _randomGenerator.GenerateCode(8);
        var senhaHash = _passwordHasher.Hash(senhaGerada);

        var usuario = Usuario.Criar(
            (byte)Perfil.Instituicao,
            instituicao.IdInstituicao,
            emailResult.Value.Value,
            senhaHash);

        await _unitOfWork.Usuarios.AdicionarAsync(usuario, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await EnviarEmailBoasVindasAsync(emailResult.Value.Value, request.Nome, senhaGerada);

        return Result<CriarInstituicaoResult>.Success(new CriarInstituicaoResult(instituicao.IdInstituicao, senhaGerada));
    }

    private async Task EnviarEmailBoasVindasAsync(string email, string nomeInstituicao, string senha)
    {
        var linhas = new List<string>
        {
            $"Olá, {nomeInstituicao}!",
            "",
            "Sua instituição foi cadastrada com sucesso no sistema Agenda PICS.",
            "",
            "Credenciais de acesso:",
            $"- Login: <strong>{email}</strong>",
            $"- Senha: <strong>{senha}</strong>",
            "",
            "Por segurança, é necessário que você altere sua senha no primeiro acesso.",
            "",
            "Acesse o sistema em: [URL do sistema]"
        };

        var mensagem = GerarTemplateEmail("Bem-vindo ao Agenda PICS", linhas);
        await _emailService.EnviarEmailAsync(email, "Agenda PICS - Cadastro de Instituição", mensagem);
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
