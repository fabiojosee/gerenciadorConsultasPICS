using AgendaPics.Application.Features.Instituicoes.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Instituicoes.Commands;

public class EditarInstituicaoCommandHandlerTests : HandlerTestBase
{
    private readonly EditarInstituicaoCommandHandler _handler;

    public EditarInstituicaoCommandHandlerTests()
    {
        _handler = new EditarInstituicaoCommandHandler(UnitOfWorkMock.Object);
    }

    private EditarInstituicaoCommand ComandoValido(int id = 1, string? email = null, string? cnpj = null) =>
        new(id, "Novo Nome", null, 1, 1,
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
        var command = ComandoValido(email: "invalido");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InstituicaoNaoEncontrada_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);

        var command = ComandoValido();

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task Handle_NovoEmailJaExisteEmOutraInstituicao_RetornaFalha()
    {
        var instituicao = EntityBuilders.CriarInstituicao(email: "atual@example.com");
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct))
            .ReturnsAsync(EntityBuilders.CriarInstituicao(idInstituicao: 99, email: EntityBuilders.EmailValido));

        var command = ComandoValido(email: EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("já cadastrado");
    }

    [Fact]
    public async Task Handle_MesmoEmail_NaoConsultaObterPorEmail()
    {
        var instituicao = EntityBuilders.CriarInstituicao(email: EntityBuilders.EmailValido);
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);

        var command = ComandoValido(email: EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        InstituicoesMock.Verify(r => r.ObterPorEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DadosValidos_AtualizaECommit()
    {
        var instituicao = EntityBuilders.CriarInstituicao(email: "antigo@example.com");
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        InstituicoesMock.Setup(r => r.ObterPorEmailAsync(EntityBuilders.EmailValido, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);

        var command = ComandoValido(email: EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        InstituicoesMock.Verify(r => r.Atualizar(instituicao), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_DadosValidos_AtualizaPropriedadesInstituicao()
    {
        var instituicao = EntityBuilders.CriarInstituicao(email: EntityBuilders.EmailValido, nome: "Antigo");
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);

        var command = ComandoValido(email: EntityBuilders.EmailValido);

        await _handler.Handle(command, Ct);

        instituicao.Nome.Should().Be("Novo Nome");
    }
}
