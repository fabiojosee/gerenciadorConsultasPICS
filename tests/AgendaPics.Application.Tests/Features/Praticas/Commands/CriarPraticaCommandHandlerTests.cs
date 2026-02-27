using AgendaPics.Application.Features.Praticas.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Praticas.Commands;

public class CriarPraticaCommandHandlerTests : HandlerTestBase
{
    private readonly CriarPraticaCommandHandler _handler;

    public CriarPraticaCommandHandlerTests()
    {
        _handler = new CriarPraticaCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_NomeVazio_RetornaFalha()
    {
        var result = await _handler.Handle(new CriarPraticaCommand("", null), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("nome é obrigatório");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task Handle_NomeNuloOuEspacos_RetornaFalha(string? nome)
    {
        var result = await _handler.Handle(new CriarPraticaCommand(nome!, null), Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DadosValidos_AdicionaPraticaECommit()
    {
        var result = await _handler.Handle(new CriarPraticaCommand("Yoga", "Descricao"), Ct);

        result.IsSuccess.Should().BeTrue();
        PraticasMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Pratica>(), Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }
}
