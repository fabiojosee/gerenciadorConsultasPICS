using AgendaPics.Application.Features.Agendamentos.Commands;
using AgendaPics.Application.Tests.Common;
using AgendaPics.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Agendamentos.Commands;

public class CriarAgendamentoCommandHandlerTests : HandlerTestBase
{
    private readonly CriarAgendamentoCommandHandler _handler;

    public CriarAgendamentoCommandHandlerTests()
    {
        _handler = new CriarAgendamentoCommandHandler(UnitOfWorkMock.Object, EmailServiceMock.Object);
        EmailServiceMock.Setup(s => s.EnviarEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    private CriarAgendamentoCommand ComandoValido(
        string? cpf = null,
        string? email = null,
        string? telefone = null) =>
        new(1, 1, "Paciente Teste",
            cpf ?? EntityBuilders.CpfValido,
            telefone ?? EntityBuilders.TelefoneValido,
            new DateTime(1990, 1, 1),
            (byte)Genero.Masculino,
            email ?? EntityBuilders.EmailValido,
            1, 1, 1,
            DateTime.Today.AddDays(7),
            "Ansiedade leve");

    [Fact]
    public async Task Handle_CpfInvalido_RetornaFalha()
    {
        var command = ComandoValido(cpf: "00000000000");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmailInvalido_RetornaFalha()
    {
        var command = ComandoValido(email: "invalido");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_TelefoneInvalido_RetornaFalha()
    {
        var command = ComandoValido(telefone: "123");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InstituicaoNaoEncontrada_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Instituição não encontrada");
    }

    [Fact]
    public async Task Handle_PraticaNaoDisponivel_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync(EntityBuilders.CriarInstituicao());
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync((Domain.Entities.PraticaInstituicao?)null);

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Prática não disponível");
    }

    [Fact]
    public async Task Handle_PacienteJaTemAgendamentoEmAndamento_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync(EntityBuilders.CriarInstituicao());
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync(EntityBuilders.CriarPraticaInstituicao());
        AgendamentosMock.Setup(r => r.ObterPorPacienteAsync(1, EntityBuilders.CpfValido, (byte)StatusAgendamento.EmAndamento, Ct))
            .ReturnsAsync(new[] { EntityBuilders.CriarAgendamento() });

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("agendamento em andamento");
    }

    [Fact]
    public async Task Handle_DadosValidos_CriaAgendamentoTermoAtendimentosEEnviaEmail()
    {
        var praticaInst = EntityBuilders.CriarPraticaInstituicao(
            periodicidade: (byte)Periodicidade.Semanal, qtdSessoes: 4);
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync(EntityBuilders.CriarInstituicao());
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync(praticaInst);
        AgendamentosMock.Setup(r => r.ObterPorPacienteAsync(1, EntityBuilders.CpfValido, (byte)StatusAgendamento.EmAndamento, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsSuccess.Should().BeTrue();
        AgendamentosMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Agendamento>(), Ct), Times.Once);
        TermosConsentimentoMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.TermoConsentimento>(), Ct), Times.Once);
        AtendimentosMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Atendimento>(), Ct), Times.Exactly(4));
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Exactly(2));
        EmailServiceMock.Verify(s => s.EnviarEmailAsync(EntityBuilders.EmailValido, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PeriodicidadeDiaria_GeraSlotsDiarios()
    {
        var praticaInst = EntityBuilders.CriarPraticaInstituicao(
            periodicidade: (byte)Periodicidade.Diaria, qtdSessoes: 3);
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync(EntityBuilders.CriarInstituicao());
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync(praticaInst);
        AgendamentosMock.Setup(r => r.ObterPorPacienteAsync(1, EntityBuilders.CpfValido, (byte)StatusAgendamento.EmAndamento, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsSuccess.Should().BeTrue();
        AtendimentosMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Atendimento>(), Ct), Times.Exactly(3));
    }

    [Fact]
    public async Task Handle_PeriodicidadeMensal_GeraSlotsMensais()
    {
        var praticaInst = EntityBuilders.CriarPraticaInstituicao(
            periodicidade: (byte)Periodicidade.Mensal, qtdSessoes: 2);
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync(EntityBuilders.CriarInstituicao());
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync(praticaInst);
        AgendamentosMock.Setup(r => r.ObterPorPacienteAsync(1, EntityBuilders.CpfValido, (byte)StatusAgendamento.EmAndamento, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsSuccess.Should().BeTrue();
        AtendimentosMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Atendimento>(), Ct), Times.Exactly(2));
    }
}
