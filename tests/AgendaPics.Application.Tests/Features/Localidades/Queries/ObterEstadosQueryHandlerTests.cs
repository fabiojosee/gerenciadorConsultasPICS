using AgendaPics.Application.Features.Localidades.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Localidades.Queries;

public class ObterEstadosQueryHandlerTests : HandlerTestBase
{
    private readonly ObterEstadosQueryHandler _handler;

    public ObterEstadosQueryHandlerTests()
    {
        _handler = new ObterEstadosQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_SemEstados_RetornaListaVazia()
    {
        EstadosMock.Setup(r => r.ObterTodosAsync(Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Estado>());

        var result = await _handler.Handle(new ObterEstadosQuery(), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComEstados_RetornaDtosOrdenadosPorNome()
    {
        var estados = new[]
        {
            EntityBuilders.CriarEstado(idEstado: 1, nome: "São Paulo", sigla: "SP"),
            EntityBuilders.CriarEstado(idEstado: 2, nome: "Acre", sigla: "AC"),
        };
        EstadosMock.Setup(r => r.ObterTodosAsync(Ct)).ReturnsAsync(estados);

        var result = await _handler.Handle(new ObterEstadosQuery(), Ct);

        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value.ToList();
        dtos.Should().HaveCount(2);
        dtos[0].Nome.Should().Be("Acre");
        dtos[1].Nome.Should().Be("São Paulo");
    }
}
