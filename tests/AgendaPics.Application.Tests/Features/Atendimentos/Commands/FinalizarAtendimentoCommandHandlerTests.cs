using AgendaPics.Application.Features.Atendimentos.Commands;
using AgendaPics.Application.Tests.Common;
using AgendaPics.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Atendimentos.Commands;

public class FinalizarAtendimentoCommandHandlerTests : HandlerTestBase
{
    private readonly FinalizarAtendimentoCommandHandler _handler;

    public FinalizarAtendimentoCommandHandlerTests()
    {
        _handler = new FinalizarAtendimentoCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_AtendimentoNaoEncontrado_RetornaFalha()
    {
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Atendimento?)null);

        var result = await _handler.Handle(new FinalizarAtendimentoCommand(1, null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_JaFinalizado_RetornaFalha()
    {
        var atendimento = EntityBuilders.CriarAtendimento(status: (byte)StatusAtendimento.Finalizado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);

        var result = await _handler.Handle(new FinalizarAtendimentoCommand(1, null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("já está finalizado");
    }

    [Fact]
    public async Task Handle_Cancelado_RetornaFalha()
    {
        var atendimento = EntityBuilders.CriarAtendimento(status: (byte)StatusAtendimento.Cancelado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);

        var result = await _handler.Handle(new FinalizarAtendimentoCommand(1, null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cancelado");
    }

    [Fact]
    public async Task Handle_UltimoAtendimento_AgendamentoVaiParaConcluido()
    {
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 10, status: (byte)StatusAtendimento.Agendado);
        var agendamento = EntityBuilders.CriarAgendamento(idAgendamento: 10);

        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);
        // Após finalizar o atendimento atual, só ele mesmo resta (count == 1 antes de commit)
        AtendimentosMock.Setup(r => r.ObterNaoFinalizadosPorAgendamentoAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { atendimento });
        AgendamentosMock.Setup(r => r.ObterPorIdAsync(10, Ct)).ReturnsAsync(agendamento);

        var result = await _handler.Handle(new FinalizarAtendimentoCommand(1, null), Ct);

        result.IsSuccess.Should().BeTrue();
        agendamento.GetStatusEnum().Should().Be(StatusAgendamento.Concluido);
        AgendamentosMock.Verify(r => r.Atualizar(agendamento), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_NaoUltimoAtendimento_AgendamentoNaoAtualizado()
    {
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 10, status: (byte)StatusAtendimento.Agendado);
        var outroAtendimento = EntityBuilders.CriarAtendimento(idAtendimento: 2, idAgendamento: 10, status: (byte)StatusAtendimento.Agendado);

        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);
        AtendimentosMock.Setup(r => r.ObterNaoFinalizadosPorAgendamentoAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { atendimento, outroAtendimento });

        var result = await _handler.Handle(new FinalizarAtendimentoCommand(1, null), Ct);

        result.IsSuccess.Should().BeTrue();
        AgendamentosMock.Verify(r => r.Atualizar(It.IsAny<Domain.Entities.Agendamento>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ComObservacao_PropagaObservacao()
    {
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 10, status: (byte)StatusAtendimento.Agendado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);
        AtendimentosMock.Setup(r => r.ObterNaoFinalizadosPorAgendamentoAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { atendimento, EntityBuilders.CriarAtendimento(idAtendimento: 2) });

        var result = await _handler.Handle(new FinalizarAtendimentoCommand(1, "Sessão produtiva"), Ct);

        result.IsSuccess.Should().BeTrue();
        atendimento.Observacao.Should().Be("Sessão produtiva");
    }
}
