using AgendaPics.Application.Features.Autenticacao.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Autenticacao.Commands;

public class SolicitarAcessoCommandHandlerTests : HandlerTestBase
{
    private readonly SolicitarAcessoCommandHandler _handler;

    public SolicitarAcessoCommandHandlerTests()
    {
        _handler = new SolicitarAcessoCommandHandler(UnitOfWorkMock.Object, AccessTokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_CpfInvalido_RetornaFalha()
    {
        var command = new SolicitarAcessoCommand("cpfinvalido", EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmailInvalido_RetornaFalha()
    {
        var command = new SolicitarAcessoCommand(EntityBuilders.CpfValido, "emailinvalido");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SemAgendamentosParaCpf_RetornaFalha()
    {
        AgendamentosMock.Setup(r => r.ObterPorCpfAsync(EntityBuilders.CpfValido, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Agendamento>());

        var command = new SolicitarAcessoCommand(EntityBuilders.CpfValido, EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("foram encontrados");
    }

    [Fact]
    public async Task Handle_EmailNaoConfereComAgendamentos_RetornaFalha()
    {
        var agendamento = EntityBuilders.CriarAgendamento(
            cpfPaciente: EntityBuilders.CpfValido,
            emailPaciente: "outro@example.com");
        AgendamentosMock.Setup(r => r.ObterPorCpfAsync(EntityBuilders.CpfValido, Ct))
            .ReturnsAsync(new[] { agendamento });

        var command = new SolicitarAcessoCommand(EntityBuilders.CpfValido, EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("foram encontrados");
    }

    [Fact]
    public async Task Handle_DadosValidos_GeraTokenERetornaSucesso()
    {
        var agendamento = EntityBuilders.CriarAgendamento(
            cpfPaciente: EntityBuilders.CpfValido,
            emailPaciente: EntityBuilders.EmailValido);
        AgendamentosMock.Setup(r => r.ObterPorCpfAsync(EntityBuilders.CpfValido, Ct))
            .ReturnsAsync(new[] { agendamento });
        AccessTokenServiceMock.Setup(s => s.GerarTokenAsync(EntityBuilders.CpfValido, EntityBuilders.EmailValido))
            .ReturnsAsync("token_gerado");

        var command = new SolicitarAcessoCommand(EntityBuilders.CpfValido, EntityBuilders.EmailValido);

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        AccessTokenServiceMock.Verify(s => s.GerarTokenAsync(EntityBuilders.CpfValido, EntityBuilders.EmailValido), Times.Once);
    }
}
