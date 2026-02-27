using AgendaPics.Application.Features.PraticasInstituicoes.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.PraticasInstituicoes.Queries;

public class ObterMinhasPraticasQueryHandlerTests : HandlerTestBase
{
    private readonly ObterMinhasPraticasQueryHandler _handler;

    public ObterMinhasPraticasQueryHandlerTests()
    {
        _handler = new ObterMinhasPraticasQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_SemVinculos_RetornaListaVazia()
    {
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.PraticaInstituicao>());

        var result = await _handler.Handle(new ObterMinhasPraticasQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComVinculos_MapeiaPeriodicidadeQtdSessoesEDia()
    {
        var vinculo = EntityBuilders.CriarPraticaInstituicao(idPratica: 1, idInstituicao: 1, periodicidade: 2, qtdSessoes: 6, diaPermitido: 5);
        var pratica = EntityBuilders.CriarPratica(idPratica: 1, nome: "Yoga");
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync(new[] { vinculo });
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)1, Ct)).ReturnsAsync(pratica);

        var result = await _handler.Handle(new ObterMinhasPraticasQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value.Single();
        dto.Nome.Should().Be("Yoga");
        dto.Periodicidade.Should().Be(2);
        dto.QtdSessoes.Should().Be(6);
        dto.DiaPermitidoParaAgendamento.Should().Be(5);
    }

    [Fact]
    public async Task Handle_PraticaNaoEncontradaParaVinculo_PulaEntrada()
    {
        var vinculo = EntityBuilders.CriarPraticaInstituicao(idPratica: 99, idInstituicao: 1);
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync(new[] { vinculo });
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)99, Ct))
            .ReturnsAsync((Domain.Entities.Pratica?)null);

        var result = await _handler.Handle(new ObterMinhasPraticasQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
