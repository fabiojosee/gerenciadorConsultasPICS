using AgendaPics.Application.Features.Autenticacao.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Autenticacao.Commands;

public class AlterarSenhaCommandHandlerTests : HandlerTestBase
{
    private readonly AlterarSenhaCommandHandler _handler;

    public AlterarSenhaCommandHandlerTests()
    {
        _handler = new AlterarSenhaCommandHandler(UnitOfWorkMock.Object, PasswordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_SenhaVazia_RetornaFalha()
    {
        var command = new AlterarSenhaCommand(1, null, "", "", false);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("obrigatória");
    }

    [Fact]
    public async Task Handle_SenhaMenosDe6Chars_RetornaFalha()
    {
        var command = new AlterarSenhaCommand(1, null, "12345", "12345", false);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("mínimo 6");
    }

    [Fact]
    public async Task Handle_SenhasDivergem_RetornaFalha()
    {
        var command = new AlterarSenhaCommand(1, null, "senha123", "diferente", false);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não conferem");
    }

    [Fact]
    public async Task Handle_UsuarioNaoEncontradoPorInstituicao_RetornaFalha()
    {
        UsuariosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync((Domain.Entities.Usuario?)null);

        var command = new AlterarSenhaCommand(1, null, "senha123", "senha123", false);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_UsuarioNaoEncontradoPorLogin_RetornaFalha()
    {
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync((Domain.Entities.Usuario?)null);

        var command = new AlterarSenhaCommand(null, "admin", "senha123", "senha123", false);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_PorIdInstituicao_AlteraSenhaECommit()
    {
        var usuario = EntityBuilders.CriarUsuario(idInstituicao: 1);
        UsuariosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.Hash("senha123")).Returns("$2a$novohash");

        var command = new AlterarSenhaCommand(1, null, "senha123", "senha123", false);

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        PasswordHasherMock.Verify(p => p.Hash("senha123"), Times.Once);
        UsuariosMock.Verify(r => r.Atualizar(usuario), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_PorLogin_AlteraSenhaECommit()
    {
        var usuario = EntityBuilders.CriarUsuario(login: "admin");
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.Hash("senha123")).Returns("$2a$novohash");

        var command = new AlterarSenhaCommand(null, "admin", "senha123", "senha123", false);

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        UsuariosMock.Verify(r => r.Atualizar(usuario), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_IsPrimeiroAcesso_AlteraFlagParaFalse()
    {
        var usuario = EntityBuilders.CriarUsuario(idInstituicao: 1, flPrimeiroAcesso: true);
        UsuariosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.Hash("senha123")).Returns("$2a$novohash");

        var command = new AlterarSenhaCommand(1, null, "senha123", "senha123", IsPrimeiroAcesso: true);

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        usuario.FlPrimeiroAcesso.Should().BeFalse();
    }
}
