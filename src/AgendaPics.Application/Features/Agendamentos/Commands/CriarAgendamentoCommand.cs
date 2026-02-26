using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Enums;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Domain.ValueObjects;

namespace AgendaPics.Application.Features.Agendamentos.Commands;

public record CriarAgendamentoCommand(
    int IdInstituicao,
    short IdPratica,
    string NomePaciente,
    string CpfPaciente,
    string TelefonePaciente,
    DateTime DataNascimentoPaciente,
    byte GeneroPaciente,
    string EmailPaciente,
    short IdEstadoPaciente,
    int IdCidadePaciente,
    byte GrauAnsiedadePaciente,
    DateTime DataPrimeiroAtendimento,
    string QueixaPaciente) : IRequest<Result<int>>;

public class CriarAgendamentoCommandHandler : IRequestHandler<CriarAgendamentoCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public CriarAgendamentoCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result<int>> Handle(CriarAgendamentoCommand request, CancellationToken cancellationToken)
    {
        var cpfResult = CPF.Create(request.CpfPaciente);
        if (cpfResult.IsFailure)
            return Result<int>.Failure(cpfResult.Error);

        var emailResult = Email.Create(request.EmailPaciente);
        if (emailResult.IsFailure)
            return Result<int>.Failure(emailResult.Error);

        var telefoneResult = Telefone.Create(request.TelefonePaciente);
        if (telefoneResult.IsFailure)
            return Result<int>.Failure(telefoneResult.Error);

        var instituicao = await _unitOfWork.Instituicoes.ObterPorIdAsync(request.IdInstituicao, cancellationToken);
        if (instituicao == null)
            return Result<int>.Failure("Instituição não encontrada");

        var praticaInstituicao = await _unitOfWork.PraticasInstituicoes
            .ObterPorPraticaInstituicaoAsync(request.IdInstituicao, request.IdPratica, cancellationToken);
        if (praticaInstituicao == null)
            return Result<int>.Failure("Prática não disponível nesta instituição");

        var agendamentosExistentes = await _unitOfWork.Agendamentos
            .ObterPorPacienteAsync(request.IdPratica, cpfResult.Value.Value, (byte)StatusAgendamento.EmAndamento, cancellationToken);

        if (agendamentosExistentes.Any())
            return Result<int>.Failure("Paciente já possui um agendamento em andamento para esta prática");

        var agendamento = Agendamento.Criar(
            request.IdInstituicao,
            request.IdPratica,
            request.NomePaciente,
            cpfResult.Value.Value,
            telefoneResult.Value.Value,
            request.DataNascimentoPaciente,
            request.GeneroPaciente,
            emailResult.Value.Value,
            request.IdEstadoPaciente,
            request.IdCidadePaciente,
            request.GrauAnsiedadePaciente
        );

        await _unitOfWork.Agendamentos.AdicionarAsync(agendamento, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var termoConsentimento = TermoConsentimento.Criar(agendamento.IdAgendamento, DateTime.Now);
        await _unitOfWork.TermosConsentimento.AdicionarAsync(termoConsentimento, cancellationToken);

        var datasAtendimento = CalcularDatasAtendimento(
            request.DataPrimeiroAtendimento,
            praticaInstituicao.QtdSessoes,
            (Periodicidade)praticaInstituicao.Periodicidade);

        foreach (var dataAtendimento in datasAtendimento)
        {
            var atendimento = Atendimento.Criar(
                agendamento.IdAgendamento,
                dataAtendimento,
                $"Grau de ansiedade: {request.GrauAnsiedadePaciente}");

            await _unitOfWork.Atendimentos.AdicionarAsync(atendimento, cancellationToken);
        }
        await _unitOfWork.CommitAsync(cancellationToken);

        await EnviarEmailConfirmacaoAsync(
            emailResult.Value.Value,
            request.NomePaciente,
            instituicao.Nome,
            datasAtendimento);

        return Result<int>.Success(agendamento.IdAgendamento);
    }

    private static List<DateTime> CalcularDatasAtendimento(DateTime dataInicio, short qtdSessoes, Periodicidade periodicidade)
    {
        var datas = new List<DateTime> { dataInicio };

        for (int i = 1; i < qtdSessoes; i++)
        {
            var proximaData = periodicidade switch
            {
                Periodicidade.Diaria => datas[^1].AddDays(1),
                Periodicidade.Semanal => datas[^1].AddDays(7),
                Periodicidade.Mensal => datas[^1].AddMonths(1),
                _ => datas[^1].AddDays(7)
            };
            datas.Add(proximaData);
        }

        return datas;
    }

    private async Task EnviarEmailConfirmacaoAsync(
        string email,
        string nomePaciente,
        string nomeInstituicao,
        List<DateTime> datasAtendimento)
    {
        var linhas = new List<string>
        {
            $"Olá, {nomePaciente}!",
            "",
            $"Seu agendamento na instituição <strong>{nomeInstituicao}</strong> foi confirmado.",
            "",
            "Datas dos atendimentos:"
        };

        foreach (var data in datasAtendimento)
        {
            linhas.Add($"- {data:dd/MM/yyyy}");
        }

        linhas.Add("");
        linhas.Add("Em caso de dúvidas, entre em contato com a instituição.");

        var mensagem = GerarTemplateEmail("Confirmação de Agendamento", linhas);

        await _emailService.EnviarEmailAsync(email, "Agenda PICS - Confirmação de Agendamento", mensagem);
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
