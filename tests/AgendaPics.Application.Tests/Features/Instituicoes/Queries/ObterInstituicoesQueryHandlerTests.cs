using AgendaPics.Application.Features.Instituicoes.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Instituicoes.Queries;

public class ObterInstituicoesQueryHandlerTests : HandlerTestBase
{
    private readonly ObterInstituicoesQueryHandler _handler;

    public ObterInstituicoesQueryHandlerTests()
    {
        _handler = new ObterInstituicoesQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_SemFiltro_UsaObterTodosAsync()
    {
        InstituicoesMock.Setup(r => r.ObterTodosAsync(Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Instituicao>());

        var result = await _handler.Handle(new ObterInstituicoesQuery(), Ct);

        result.IsSuccess.Should().BeTrue();
        InstituicoesMock.Verify(r => r.ObterTodosAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_FiltroIdCidade_UsaObterPorCidadeAsync()
    {
        InstituicoesMock.Setup(r => r.ObterPorCidadeAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Instituicao>());

        var result = await _handler.Handle(new ObterInstituicoesQuery(IdCidade: 1), Ct);

        result.IsSuccess.Should().BeTrue();
        InstituicoesMock.Verify(r => r.ObterPorCidadeAsync(1, Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_FiltroIdEstado_UsaObterPorEstadoAsync()
    {
        InstituicoesMock.Setup(r => r.ObterPorEstadoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Instituicao>());

        var result = await _handler.Handle(new ObterInstituicoesQuery(IdEstado: 1), Ct);

        result.IsSuccess.Should().BeTrue();
        InstituicoesMock.Verify(r => r.ObterPorEstadoAsync(1, Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComInstituicoes_MapeiaParaDto()
    {
        var instituicao = EntityBuilders.CriarInstituicao(idInstituicao: 7, nome: "Clínica A");
        InstituicoesMock.Setup(r => r.ObterTodosAsync(Ct))
            .ReturnsAsync(new[] { instituicao });

        var result = await _handler.Handle(new ObterInstituicoesQuery(), Ct);

        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value.ToList();
        dtos.Should().HaveCount(1);
        dtos[0].IdInstituicao.Should().Be(7);
        dtos[0].Nome.Should().Be("Clínica A");
    }
}
