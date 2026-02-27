using AgendaPics.Application.Features.PraticasInstituicoes.Commands;
using AgendaPics.Application.Tests.Common;
using AgendaPics.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.PraticasInstituicoes.Commands;

public class EditarVinculoPraticaCommandHandlerTests : HandlerTestBase
{
    private readonly EditarVinculoPraticaCommandHandler _handler;

    public EditarVinculoPraticaCommandHandlerTests()
    {
        _handler = new EditarVinculoPraticaCommandHandler(UnitOfWorkMock.Object);
    }

    private EditarVinculoPraticaCommand ComandoValido() => new(1, 1, 2, 8, 3);

    [Fact]
    public async Task Handle_IdInstituicaoZero_RetornaFalha()
    {
        var result = await _handler.Handle(new EditarVinculoPraticaCommand(1, 0, 1, 4, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("instituição");
    }

    [Fact]
    public async Task Handle_IdPraticaZero_RetornaFalha()
    {
        var result = await _handler.Handle(new EditarVinculoPraticaCommand(0, 1, 1, 4, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("prática");
    }

    [Fact]
    public async Task Handle_PeriodicidadeZero_RetornaFalha()
    {
        var result = await _handler.Handle(new EditarVinculoPraticaCommand(1, 1, 0, 4, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("periodicidade");
    }

    [Fact]
    public async Task Handle_QtdSessoesZero_RetornaFalha()
    {
        var result = await _handler.Handle(new EditarVinculoPraticaCommand(1, 1, 1, 0, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("sessões");
    }

    [Fact]
    public async Task Handle_VinculoNaoEncontrado_RetornaFalha()
    {
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync((Domain.Entities.PraticaInstituicao?)null);

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não está vinculada");
    }

    [Fact]
    public async Task Handle_DadosValidos_AtualizaVinculoECommit()
    {
        var vinculo = EntityBuilders.CriarPraticaInstituicao(
            periodicidade: (byte)Periodicidade.Semanal, qtdSessoes: 4);
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct)).ReturnsAsync(vinculo);

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsSuccess.Should().BeTrue();
        vinculo.Periodicidade.Should().Be(2); // Semanal
        vinculo.QtdSessoes.Should().Be(8);
        PraticasInstituicoesMock.Verify(r => r.AtualizarAsync(vinculo, Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }
}
