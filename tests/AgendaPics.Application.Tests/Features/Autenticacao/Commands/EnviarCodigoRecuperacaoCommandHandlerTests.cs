using AgendaPics.Application.Features.Autenticacao.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Autenticacao.Commands;

public class EnviarCodigoRecuperacaoCommandHandlerTests : HandlerTestBase
{
    private readonly EnviarCodigoRecuperacaoCommandHandler _handler;

    public EnviarCodigoRecuperacaoCommandHandlerTests()
    {
        _handler = new EnviarCodigoRecuperacaoCommandHandler(
            UnitOfWorkMock.Object,
            EmailServiceMock.Object,
            RandomGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_EmailInvalido_RetornaFalha()
    {
        var command = new EnviarCodigoRecuperacaoCommand("nao-e-um-email");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InstituicaoNaoEncontrada_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);

        var command = new EnviarCodigoRecuperacaoCommand(EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_FalhaNoEnvioDeEmail_RetornaFalha()
    {
        var instituicao = EntityBuilders.CriarInstituicao(email: EntityBuilders.EmailValido);
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct)).ReturnsAsync(instituicao);
        RandomGeneratorMock.Setup(r => r.GenerateCode(6)).Returns("ABC123");
        EmailServiceMock.Setup(s => s.EnviarEmailAsync(EntityBuilders.EmailValido, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var command = new EnviarCodigoRecuperacaoCommand(EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Falha ao enviar");
    }

    [Fact]
    public async Task Handle_DadosValidos_EnviaEmailERetornaCodigo()
    {
        var instituicao = EntityBuilders.CriarInstituicao(idInstituicao: 5, email: EntityBuilders.EmailValido);
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct)).ReturnsAsync(instituicao);
        RandomGeneratorMock.Setup(r => r.GenerateCode(6)).Returns("XYZ789");
        EmailServiceMock.Setup(s => s.EnviarEmailAsync(EntityBuilders.EmailValido, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var command = new EnviarCodigoRecuperacaoCommand(EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Codigo.Should().Be("XYZ789");
        result.Value.IdInstituicao.Should().Be(5);
        RandomGeneratorMock.Verify(r => r.GenerateCode(6), Times.Once);
        EmailServiceMock.Verify(s => s.EnviarEmailAsync(EntityBuilders.EmailValido, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
