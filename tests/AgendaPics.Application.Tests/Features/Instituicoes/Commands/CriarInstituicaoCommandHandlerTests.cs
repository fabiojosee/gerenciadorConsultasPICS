using AgendaPics.Application.Features.Instituicoes.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Instituicoes.Commands;

public class CriarInstituicaoCommandHandlerTests : HandlerTestBase
{
    private readonly CriarInstituicaoCommandHandler _handler;

    public CriarInstituicaoCommandHandlerTests()
    {
        _handler = new CriarInstituicaoCommandHandler(
            UnitOfWorkMock.Object,
            PasswordHasherMock.Object,
            RandomGeneratorMock.Object,
            EmailServiceMock.Object);
    }

    private CriarInstituicaoCommand ComandoValido(string? email = null, string? cnpj = null) =>
        new("Instituição Teste", null, 1, 1,
            cnpj ?? EntityBuilders.CnpjValido, "01310100",
            email ?? EntityBuilders.EmailValido,
            TimeSpan.FromHours(8), TimeSpan.FromHours(18));

    [Fact]
    public async Task Handle_CnpjInvalido_RetornaFalha()
    {
        var command = ComandoValido(cnpj: "00000000000000");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmailInvalido_RetornaFalha()
    {
        var command = ComandoValido(email: "emailinvalido");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmailJaCadastrado_RetornaFalha()
    {
        var instituicaoExistente = EntityBuilders.CriarInstituicao();
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct)).ReturnsAsync(instituicaoExistente);

        var command = ComandoValido();

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("E-mail já cadastrado");
    }

    [Fact]
    public async Task Handle_CnpjJaCadastrado_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);
        InstituicoesMock.Setup(r => r.ObterPorCnpjAsync(It.IsAny<string>(), Ct))
            .ReturnsAsync(EntityBuilders.CriarInstituicao());

        var command = ComandoValido();

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("CNPJ já cadastrado");
    }

    [Fact]
    public async Task Handle_DadosValidos_CriaInstituicaoUsuarioEEnviaEmail()
    {
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);
        InstituicoesMock.Setup(r => r.ObterPorCnpjAsync(It.IsAny<string>(), Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);
        RandomGeneratorMock.Setup(r => r.GenerateCode(8)).Returns("SENHA123");
        PasswordHasherMock.Setup(p => p.Hash("SENHA123")).Returns("$2a$hash");
        EmailServiceMock.Setup(s => s.EnviarEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var command = ComandoValido();

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.SenhaGerada.Should().Be("SENHA123");
        InstituicoesMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Instituicao>(), Ct), Times.Once);
        UsuariosMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Usuario>(), Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Exactly(2));
        EmailServiceMock.Verify(s => s.EnviarEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
