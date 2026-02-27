using AgendaPics.Application.Features.Praticas.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Praticas.Commands;

public class ExcluirPraticaCommandHandlerTests : HandlerTestBase
{
    private readonly ExcluirPraticaCommandHandler _handler;

    public ExcluirPraticaCommandHandlerTests()
    {
        _handler = new ExcluirPraticaCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComAgendamentosVinculados_RetornaFalha()
    {
        AgendamentosMock.Setup(r => r.ObterPorPraticaAsync(1, Ct))
            .ReturnsAsync(new[] { EntityBuilders.CriarAgendamento() });

        var result = await _handler.Handle(new ExcluirPraticaCommand(1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("agendamentos");
    }

    [Fact]
    public async Task Handle_SemAgendamentosNemVinculos_RemovePraticaECommit()
    {
        AgendamentosMock.Setup(r => r.ObterPorPraticaAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());
        PraticasInstituicoesMock.Setup(r => r.ObterInstituicoesVinculadasAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.PraticaInstituicao>());

        var result = await _handler.Handle(new ExcluirPraticaCommand(1), Ct);

        result.IsSuccess.Should().BeTrue();
        PraticasMock.Verify(r => r.RemoverAsync(1, Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComVinculos_RemoveCadaVinculoEPratica()
    {
        var vinculo1 = EntityBuilders.CriarPraticaInstituicao(idPratica: 1, idInstituicao: 10);
        var vinculo2 = EntityBuilders.CriarPraticaInstituicao(idPratica: 1, idInstituicao: 20);
        AgendamentosMock.Setup(r => r.ObterPorPraticaAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());
        PraticasInstituicoesMock.Setup(r => r.ObterInstituicoesVinculadasAsync(1, Ct))
            .ReturnsAsync(new[] { vinculo1, vinculo2 });

        var result = await _handler.Handle(new ExcluirPraticaCommand(1), Ct);

        result.IsSuccess.Should().BeTrue();
        PraticasInstituicoesMock.Verify(r => r.RemoverAsync(1, 10, Ct), Times.Once);
        PraticasInstituicoesMock.Verify(r => r.RemoverAsync(1, 20, Ct), Times.Once);
        PraticasMock.Verify(r => r.RemoverAsync(1, Ct), Times.Once);
    }
}
