using AgendaPics.Application.Features.Localidades.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Localidades.Queries;

public class ObterCidadeQueryHandlerTests : HandlerTestBase
{
    private readonly ObterCidadeQueryHandler _handler;

    public ObterCidadeQueryHandlerTests()
    {
        _handler = new ObterCidadeQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_CidadeEncontrada_RetornaDtoCorreto()
    {
        var cidade = EntityBuilders.CriarCidade(idCidade: 5, nome: "Campinas", idEstado: 35);
        CidadesMock.Setup(r => r.ObterPorIdAsync(5, Ct)).ReturnsAsync(cidade);

        var result = await _handler.Handle(new ObterCidadeQuery(5), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.IdCidade.Should().Be(5);
        result.Value.Nome.Should().Be("Campinas");
        result.Value.IdEstado.Should().Be(35);
    }

    [Fact]
    public async Task Handle_CidadeNaoEncontrada_LancaNullReferenceException()
    {
        // Bug conhecido: o handler não verifica null antes de acessar cidade.IdCidade
        CidadesMock.Setup(r => r.ObterPorIdAsync(99, Ct))
            .ReturnsAsync((Domain.Entities.Cidade?)null);

        var act = async () => await _handler.Handle(new ObterCidadeQuery(99), Ct);

        await act.Should().ThrowAsync<NullReferenceException>();
    }
}
