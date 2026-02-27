using AgendaPics.Application.Features.Praticas.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Praticas.Commands;

public class EditarPraticaCommandHandlerTests : HandlerTestBase
{
    private readonly EditarPraticaCommandHandler _handler;

    public EditarPraticaCommandHandlerTests()
    {
        _handler = new EditarPraticaCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_NomeVazio_RetornaFalha()
    {
        var result = await _handler.Handle(new EditarPraticaCommand(1, "", null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("nome é obrigatório");
    }

    [Fact]
    public async Task Handle_PraticaNaoEncontrada_RetornaFalha()
    {
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)1, Ct))
            .ReturnsAsync((Domain.Entities.Pratica?)null);

        var result = await _handler.Handle(new EditarPraticaCommand(1, "Yoga", null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task Handle_DadosValidos_AtualizaECommit()
    {
        var pratica = EntityBuilders.CriarPratica(idPratica: 1, nome: "Antigo");
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)1, Ct)).ReturnsAsync(pratica);

        var result = await _handler.Handle(new EditarPraticaCommand(1, "Yoga", "Descricao nova"), Ct);

        result.IsSuccess.Should().BeTrue();
        pratica.Nome.Should().Be("Yoga");
        PraticasMock.Verify(r => r.AtualizarAsync(pratica, Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }
}
