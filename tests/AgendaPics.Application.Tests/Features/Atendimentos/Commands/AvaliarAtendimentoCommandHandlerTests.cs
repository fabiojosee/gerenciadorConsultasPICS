using AgendaPics.Application.Features.Atendimentos.Commands;
using AgendaPics.Application.Tests.Common;
using AgendaPics.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Atendimentos.Commands;

public class AvaliarAtendimentoCommandHandlerTests : HandlerTestBase
{
    private readonly AvaliarAtendimentoCommandHandler _handler;

    public AvaliarAtendimentoCommandHandlerTests()
    {
        _handler = new AvaliarAtendimentoCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_AtendimentoNaoEncontrado_RetornaFalha()
    {
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Atendimento?)null);

        var result = await _handler.Handle(new AvaliarAtendimentoCommand(1, "http://link", "obs"), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_StatusNaoFinalizado_RetornaFalha()
    {
        var atendimento = EntityBuilders.CriarAtendimento(status: (byte)StatusAtendimento.Agendado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);

        var result = await _handler.Handle(new AvaliarAtendimentoCommand(1, "http://link", "obs"), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("finalizados");
    }

    [Fact]
    public async Task Handle_LinkVazio_RetornaFalha()
    {
        var atendimento = EntityBuilders.CriarAtendimento(status: (byte)StatusAtendimento.Finalizado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);

        var result = await _handler.Handle(new AvaliarAtendimentoCommand(1, "", "obs"), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Link");
    }

    [Fact]
    public async Task Handle_DadosValidos_AdicionaAvaliacaoECommit()
    {
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, status: (byte)StatusAtendimento.Finalizado);
        AtendimentosMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(atendimento);

        var result = await _handler.Handle(new AvaliarAtendimentoCommand(1, "http://forms.example.com", "Boa sessão"), Ct);

        result.IsSuccess.Should().BeTrue();
        AvaliacoesMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Avaliacao>(), Ct), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }
}
