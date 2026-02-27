using AgendaPics.Application.Features.PraticasInstituicoes.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.PraticasInstituicoes.Commands;

public class DesvincularPraticaCommandHandlerTests : HandlerTestBase
{
    private readonly DesvincularPraticaCommandHandler _handler;

    public DesvincularPraticaCommandHandlerTests()
    {
        _handler = new DesvincularPraticaCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComAgendamentos_RetornaFalha()
    {
        AgendamentosMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync(new[] { EntityBuilders.CriarAgendamento() });

        var result = await _handler.Handle(new DesvincularPraticaCommand(1, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("agendamentos");
    }

    [Fact]
    public async Task Handle_SemAgendamentos_RemoveVinculoECommit()
    {
        AgendamentosMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());

        var result = await _handler.Handle(new DesvincularPraticaCommand(1, 1), Ct);

        result.IsSuccess.Should().BeTrue();
        PraticasInstituicoesMock.Verify(r => r.RemoverAsync(1, 1, Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }
}
