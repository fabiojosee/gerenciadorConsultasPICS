using AgendaPics.Application.Features.Atendimentos.Commands;
using AgendaPics.Application.Tests.Common;
using AgendaPics.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Atendimentos.Commands;

public class CancelarAtendimentoCommandHandlerTests : HandlerTestBase
{
    private readonly CancelarAtendimentoCommandHandler _handler;

    public CancelarAtendimentoCommandHandlerTests()
    {
        _handler = new CancelarAtendimentoCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_AtendimentoNaoEncontrado_RetornaFalha()
    {
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Atendimento?)null);

        var result = await _handler.Handle(new CancelarAtendimentoCommand(1, null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_JaCancelado_RetornaFalha()
    {
        var atendimento = EntityBuilders.CriarAtendimento(status: (byte)StatusAtendimento.Cancelado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);

        var result = await _handler.Handle(new CancelarAtendimentoCommand(1, null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("já está cancelado");
    }

    [Fact]
    public async Task Handle_JaFinalizado_RetornaFalha()
    {
        var atendimento = EntityBuilders.CriarAtendimento(status: (byte)StatusAtendimento.Finalizado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);

        var result = await _handler.Handle(new CancelarAtendimentoCommand(1, null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("finalizado");
    }

    [Fact]
    public async Task Handle_DadosValidos_CancelaAtendimentosEAgendamento()
    {
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 10, status: (byte)StatusAtendimento.Agendado);
        var agendamento = EntityBuilders.CriarAgendamento(idAgendamento: 10);
        var outroAtendimento = EntityBuilders.CriarAtendimento(idAtendimento: 2, idAgendamento: 10, status: (byte)StatusAtendimento.Agendado);

        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);
        AtendimentosMock.Setup(r => r.ObterNaoFinalizadosPorAgendamentoAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { atendimento, outroAtendimento });
        AgendamentosMock.Setup(r => r.ObterPorIdAsync(10, Ct)).ReturnsAsync(agendamento);

        var result = await _handler.Handle(new CancelarAtendimentoCommand(1, null), Ct);

        result.IsSuccess.Should().BeTrue();
        atendimento.GetStatusEnum().Should().Be(StatusAtendimento.Cancelado);
        agendamento.GetStatusEnum().Should().Be(StatusAgendamento.Cancelado);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComObservacao_PropagaObservacao()
    {
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 10, status: (byte)StatusAtendimento.Agendado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);
        AtendimentosMock.Setup(r => r.ObterNaoFinalizadosPorAgendamentoAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { atendimento });
        AgendamentosMock.Setup(r => r.ObterPorIdAsync(10, Ct))
            .ReturnsAsync((Domain.Entities.Agendamento?)null);

        var result = await _handler.Handle(new CancelarAtendimentoCommand(1, "Paciente desistiu"), Ct);

        result.IsSuccess.Should().BeTrue();
        atendimento.Observacao.Should().Be("Paciente desistiu");
    }
}
