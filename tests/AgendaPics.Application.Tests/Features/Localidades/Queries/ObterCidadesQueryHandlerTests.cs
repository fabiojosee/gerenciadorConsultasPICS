using AgendaPics.Application.Features.Localidades.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Localidades.Queries;

public class ObterCidadesQueryHandlerTests : HandlerTestBase
{
    private readonly ObterCidadesQueryHandler _handler;

    public ObterCidadesQueryHandlerTests()
    {
        _handler = new ObterCidadesQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_SemCidades_RetornaListaVazia()
    {
        CidadesMock.Setup(r => r.ObterPorEstadoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Cidade>());

        var result = await _handler.Handle(new ObterCidadesQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComCidades_RetornaDtosOrdenadosPorNome()
    {
        var cidades = new[]
        {
            EntityBuilders.CriarCidade(idCidade: 1, nome: "São Paulo"),
            EntityBuilders.CriarCidade(idCidade: 2, nome: "Campinas"),
        };
        CidadesMock.Setup(r => r.ObterPorEstadoAsync(1, Ct)).ReturnsAsync(cidades);

        var result = await _handler.Handle(new ObterCidadesQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value.ToList();
        dtos[0].Nome.Should().Be("Campinas");
        dtos[1].Nome.Should().Be("São Paulo");
    }

    [Fact]
    public async Task Handle_PassaIdEstadoCorretoParaRepositorio()
    {
        CidadesMock.Setup(r => r.ObterPorEstadoAsync(42, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Cidade>());

        await _handler.Handle(new ObterCidadesQuery(42), Ct);

        CidadesMock.Verify(r => r.ObterPorEstadoAsync(42, Ct), Times.Once);
    }
}
