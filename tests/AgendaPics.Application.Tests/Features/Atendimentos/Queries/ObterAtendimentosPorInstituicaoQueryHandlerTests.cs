using AgendaPics.Application.Features.Atendimentos.Queries;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Atendimentos.Queries;

public class ObterAtendimentosPorInstituicaoQueryHandlerTests : HandlerTestBase
{
    private readonly ObterAtendimentosPorInstituicaoQueryHandler _handler;

    public ObterAtendimentosPorInstituicaoQueryHandlerTests()
    {
        _handler = new ObterAtendimentosPorInstituicaoQueryHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_InstituicaoNaoEncontrada_RetornaFalha()
    {
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct))
            .ReturnsAsync((Domain.Entities.Instituicao?)null);

        var result = await _handler.Handle(new ObterAtendimentosPorInstituicaoQuery(1), Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task Handle_SemAtendimentos_RetornaListaVazia()
    {
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(EntityBuilders.CriarInstituicao());
        AtendimentosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Atendimento>());

        var result = await _handler.Handle(new ObterAtendimentosPorInstituicaoQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComAtendimentos_CpfMascarado()
    {
        var instituicao = EntityBuilders.CriarInstituicao();
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 10);
        var agendamento = EntityBuilders.CriarAgendamento(idAgendamento: 10, cpfPaciente: EntityBuilders.CpfValido);
        var pratica = EntityBuilders.CriarPratica();
        var cidade = EntityBuilders.CriarCidade();
        var estado = EntityBuilders.CriarEstado();

        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        AtendimentosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync(new[] { atendimento });
        AgendamentosMock.Setup(r => r.ObterPorIdAsync(10, Ct)).ReturnsAsync(agendamento);
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)agendamento.IdPratica, Ct)).ReturnsAsync(pratica);
        CidadesMock.Setup(r => r.ObterPorIdAsync(agendamento.IdCidadePaciente, Ct)).ReturnsAsync(cidade);
        EstadosMock.Setup(r => r.ObterPorIdAsync((short)agendamento.IdEstadoPaciente, Ct)).ReturnsAsync(estado);

        var result = await _handler.Handle(new ObterAtendimentosPorInstituicaoQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value.Single();
        dto.CpfPaciente.Should().Be($"{EntityBuilders.CpfValido[..3]}.***.***.{EntityBuilders.CpfValido[9..]}");
    }

    [Fact]
    public async Task Handle_AtendimentoSemAgendamento_PulaEntrada()
    {
        var instituicao = EntityBuilders.CriarInstituicao();
        var atendimento = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 99);

        InstituicoesMock.Setup(r => r.ObterPorIdAsync(1, Ct)).ReturnsAsync(instituicao);
        AtendimentosMock.Setup(r => r.ObterPorInstituicaoAsync(1, Ct)).ReturnsAsync(new[] { atendimento });
        AgendamentosMock.Setup(r => r.ObterPorIdAsync(99, Ct))
            .ReturnsAsync((Domain.Entities.Agendamento?)null);

        var result = await _handler.Handle(new ObterAtendimentosPorInstituicaoQuery(1), Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
