using AgendaPics.Application.Features.PraticasInstituicoes.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.PraticasInstituicoes.Queries;

public class ObterPraticasPorInstituicaoQueryHandlerTests : HandlerTestBase
{
    private readonly ObterPraticasInstituicaoQueryHandler _handler;

    public ObterPraticasPorInstituicaoQueryHandlerTests()
    {
        _handler = new ObterPraticasInstituicaoQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_VinculoNaoEncontrado_RetornaFalha()
    {
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync((Domain.Entities.PraticaInstituicao?)null);

        var result = await _handler.Handle(new ObterPraticaInstituicaoQuery(1, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task Handle_VinculoEncontrado_RetornaDtoCorreto()
    {
        var vinculo = EntityBuilders.CriarPraticaInstituicao(idPratica: 1, idInstituicao: 1, periodicidade: 3, qtdSessoes: 12, diaPermitido: 2);
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct)).ReturnsAsync(vinculo);

        var result = await _handler.Handle(new ObterPraticaInstituicaoQuery(1, 1), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.IdPratica.Should().Be(1);
        result.Value.IdInstituicao.Should().Be(1);
        result.Value.Periodicidade.Should().Be(3);
        result.Value.QtdSessoes.Should().Be(12);
        result.Value.DiaPermitidoParaAgendamento.Should().Be(2);
    }
}
