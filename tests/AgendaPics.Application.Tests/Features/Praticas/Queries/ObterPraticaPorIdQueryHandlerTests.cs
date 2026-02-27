using AgendaPics.Application.Features.Praticas.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Praticas.Queries;

public class ObterPraticaPorIdQueryHandlerTests : HandlerTestBase
{
    private readonly ObterPraticaPorIdQueryHandler _handler;

    public ObterPraticaPorIdQueryHandlerTests()
    {
        _handler = new ObterPraticaPorIdQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_PraticaNaoEncontrada_RetornaFalha()
    {
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)1, Ct))
            .ReturnsAsync((Domain.Entities.Pratica?)null);

        var result = await _handler.Handle(new ObterPraticaPorIdQuery(1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task Handle_PraticaEncontrada_RetornaDtoComCamposCorretos()
    {
        var pratica = EntityBuilders.CriarPratica(idPratica: 3, nome: "Meditação", descricao: "Prática mindful");
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)3, Ct)).ReturnsAsync(pratica);

        var result = await _handler.Handle(new ObterPraticaPorIdQuery(3), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.IdPratica.Should().Be(3);
        result.Value.Nome.Should().Be("Meditação");
        result.Value.Descricao.Should().Be("Prática mindful");
    }
}
