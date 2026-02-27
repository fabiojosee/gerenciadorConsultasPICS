using AgendaPics.Application.Features.Instituicoes.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Instituicoes.Commands;

public class ExcluirInstituicaoCommandHandlerTests : HandlerTestBase
{
    private readonly ExcluirInstituicaoCommandHandler _handler;

    public ExcluirInstituicaoCommandHandlerTests()
    {
        _handler = new ExcluirInstituicaoCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_InstituicaoNaoEncontrada_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);

        var result = await _handler.Handle(new ExcluirInstituicaoCommand(1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task Handle_ComAgendamentosAtivos_RetornaFalha()
    {
        var instituicao = EntityBuilders.CriarInstituicao();
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        AgendamentosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(new[] { EntityBuilders.CriarAgendamento() });

        var result = await _handler.Handle(new ExcluirInstituicaoCommand(1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("agendamentos vinculados");
    }

    [Fact]
    public async Task Handle_SemAgendamentosNemUsuarioNemPraticas_RemoveInstituicaoECommit()
    {
        var instituicao = EntityBuilders.CriarInstituicao();
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        AgendamentosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());
        UsuariosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Usuario?)null);
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.PraticaInstituicao>());

        var result = await _handler.Handle(new ExcluirInstituicaoCommand(1), Ct);

        result.IsSuccess.Should().BeTrue();
        InstituicoesMock.Verify(r => r.Remover(instituicao), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComUsuario_RemoveUsuarioTambem()
    {
        var instituicao = EntityBuilders.CriarInstituicao();
        var usuario = EntityBuilders.CriarUsuario(idInstituicao: 1);
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        AgendamentosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());
        UsuariosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync(usuario);
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.PraticaInstituicao>());

        var result = await _handler.Handle(new ExcluirInstituicaoCommand(1), Ct);

        result.IsSuccess.Should().BeTrue();
        UsuariosMock.Verify(r => r.Remover(usuario), Times.Once);
        InstituicoesMock.Verify(r => r.Remover(instituicao), Times.Once);
    }

    [Fact]
    public async Task Handle_ComPraticasVinculadas_RemoveCadaPraticaInstituicao()
    {
        var instituicao = EntityBuilders.CriarInstituicao();
        var pratica1 = EntityBuilders.CriarPraticaInstituicao(idPratica: 1, idInstituicao: 1);
        var pratica2 = EntityBuilders.CriarPraticaInstituicao(idPratica: 2, idInstituicao: 1);
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        AgendamentosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());
        UsuariosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Usuario?)null);
        PraticasInstituicoesMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(new[] { pratica1, pratica2 });

        var result = await _handler.Handle(new ExcluirInstituicaoCommand(1), Ct);

        result.IsSuccess.Should().BeTrue();
        PraticasInstituicoesMock.Verify(r => r.Remover(pratica1), Times.Once);
        PraticasInstituicoesMock.Verify(r => r.Remover(pratica2), Times.Once);
        InstituicoesMock.Verify(r => r.Remover(instituicao), Times.Once);
    }
}
