using AgendaPics.Application.Features.PraticasInstituicoes.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.PraticasInstituicoes.Commands;

public class VincularPraticaCommandHandlerTests : HandlerTestBase
{
    private readonly VincularPraticaCommandHandler _handler;

    public VincularPraticaCommandHandlerTests()
    {
        _handler = new VincularPraticaCommandHandler(UnitOfWorkMock.Object);
    }

    private VincularPraticaCommand ComandoValido() => new(1, 1, 1, 4, 1);

    [Fact]
    public async Task Handle_IdInstituicaoZero_RetornaFalha()
    {
        var result = await _handler.Handle(new VincularPraticaCommand(1, 0, 1, 4, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("instituição");
    }

    [Fact]
    public async Task Handle_IdPraticaZero_RetornaFalha()
    {
        var result = await _handler.Handle(new VincularPraticaCommand(0, 1, 1, 4, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("prática");
    }

    [Fact]
    public async Task Handle_PeriodicidadeZero_RetornaFalha()
    {
        var result = await _handler.Handle(new VincularPraticaCommand(1, 1, 0, 4, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("periodicidade");
    }

    [Fact]
    public async Task Handle_QtdSessoesZero_RetornaFalha()
    {
        var result = await _handler.Handle(new VincularPraticaCommand(1, 1, 1, 0, 1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("sessões");
    }

    [Fact]
    public async Task Handle_VinculoJaExiste_RetornaFalha()
    {
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync(EntityBuilders.CriarPraticaInstituicao());

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("já está vinculada");
    }

    [Fact]
    public async Task Handle_DadosValidos_AdicionaVinculoECommit()
    {
        PraticasInstituicoesMock.Setup(r => r.ObterPorPraticaInstituicaoAsync(1, 1, Ct))
            .ReturnsAsync((Domain.Entities.PraticaInstituicao?)null);

        var result = await _handler.Handle(ComandoValido(), Ct);

        result.IsSuccess.Should().BeTrue();
        PraticasInstituicoesMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.PraticaInstituicao>(), Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }
}
