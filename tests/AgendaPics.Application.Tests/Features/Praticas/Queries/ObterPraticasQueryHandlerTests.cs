using AgendaPics.Application.Features.Praticas.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Praticas.Queries;

public class ObterPraticasQueryHandlerTests : HandlerTestBase
{
    private readonly ObterPraticasQueryHandler _handler;

    public ObterPraticasQueryHandlerTests()
    {
        _handler = new ObterPraticasQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_SemFiltroInstituicao_UsaObterTodosAsync()
    {
        PraticasMock.Setup(r => r.ObterTodosAsync(Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Pratica>());

        var result = await _handler.Handle(new ObterPraticasQuery(), Ct);

        result.IsSuccess.Should().BeTrue();
        PraticasMock.Verify(r => r.ObterTodosAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComFiltroInstituicao_BuscaVinculosDaInstituicao()
    {
        var vinculo = EntityBuilders.CriarPraticaInstituicao(idPratica: 1, idInstituicao: 5);
        var pratica = EntityBuilders.CriarPratica(idPratica: 1, nome: "Yoga");
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(5, Ct)).ReturnsAsync(new[] { vinculo });
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)1, Ct)).ReturnsAsync(pratica);

        var result = await _handler.Handle(new ObterPraticasQuery(IdInstituicao: 5), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Nome.Should().Be("Yoga");
    }

    [Fact]
    public async Task Handle_PraticaNaoEncontradaParaVinculo_PulaEntrada()
    {
        var vinculo = EntityBuilders.CriarPraticaInstituicao(idPratica: 99, idInstituicao: 5);
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(5, Ct)).ReturnsAsync(new[] { vinculo });
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)99, Ct))
            .ReturnsAsync((Domain.Entities.Pratica?)null);

        var result = await _handler.Handle(new ObterPraticasQuery(IdInstituicao: 5), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
