using AgendaPics.Application.Features.Atendimentos.Queries;
using AgendaPics.Application.Tests.Common;
using AgendaPics.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Atendimentos.Queries;

public class ObterAtendimentosPorCpfQueryHandlerTests : HandlerTestBase
{
    private readonly ObterAtendimentosPorCpfQueryHandler _handler;

    public ObterAtendimentosPorCpfQueryHandlerTests()
    {
        _handler = new ObterAtendimentosPorCpfQueryHandler(UnitOfWorkMock.Object, AccessTokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_CpfInvalido_RetornaFalha()
    {
        var query = new ObterAtendimentosPorCpfQuery("cpfinvalido", "token123");

        var result = await _handler.Handle(query, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("CPF inválido");
    }

    [Fact]
    public async Task Handle_TokenInvalido_RetornaFalha()
    {
        AccessTokenServiceMock.Setup(s => s.ValidarTokenAsync(EntityBuilders.CpfValido, "tokeninvalido"))
            .ReturnsAsync(false);

        var query = new ObterAtendimentosPorCpfQuery(EntityBuilders.CpfValido, "tokeninvalido", ReutilizarToken: false);

        var result = await _handler.Handle(query, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Token inválido");
    }

    [Fact]
    public async Task Handle_ReutilizarTokenTrue_NaoValidaToken()
    {
        AtendimentosMock.Setup(r => r.ObterPorCpfPacienteAsync(EntityBuilders.CpfValido, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Atendimento>());

        var query = new ObterAtendimentosPorCpfQuery(EntityBuilders.CpfValido, "qualquertoken", ReutilizarToken: true);

        var result = await _handler.Handle(query, Ct);

        result.IsSuccess.Should().BeTrue();
        AccessTokenServiceMock.Verify(s => s.ValidarTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SemAtendimentos_RetornaListaVazia()
    {
        AccessTokenServiceMock.Setup(s => s.ValidarTokenAsync(EntityBuilders.CpfValido, "token123")).ReturnsAsync(true);
        AtendimentosMock.Setup(r => r.ObterPorCpfPacienteAsync(EntityBuilders.CpfValido, Ct))
            .ReturnsAsync(Enumerable.Empty<Domain.Entities.Atendimento>());

        var query = new ObterAtendimentosPorCpfQuery(EntityBuilders.CpfValido, "token123");

        var result = await _handler.Handle(query, Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComAtendimentos_MapeiaParaDtoOrdenadoPorDataDesc()
    {
        AccessTokenServiceMock.Setup(s => s.ValidarTokenAsync(EntityBuilders.CpfValido, "token123")).ReturnsAsync(true);

        var atendimento1 = EntityBuilders.CriarAtendimento(idAtendimento: 1, idAgendamento: 10, dataAtendimento: DateTime.Today.AddDays(-7));
        var atendimento2 = EntityBuilders.CriarAtendimento(idAtendimento: 2, idAgendamento: 10, dataAtendimento: DateTime.Today);
        var agendamento = EntityBuilders.CriarAgendamento(idAgendamento: 10, cpfPaciente: EntityBuilders.CpfValido);
        var instituicao = EntityBuilders.CriarInstituicao(nome: "Clínica A");
        var pratica = EntityBuilders.CriarPratica(nome: "Yoga");
        var cidade = EntityBuilders.CriarCidade(nome: "SP");
        var estado = EntityBuilders.CriarEstado(nome: "São Paulo");

        AtendimentosMock.Setup(r => r.ObterPorCpfPacienteAsync(EntityBuilders.CpfValido, Ct))
            .ReturnsAsync(new[] { atendimento1, atendimento2 });
        AgendamentosMock.Setup(r => r.ObterPorIdAsync(10, Ct)).ReturnsAsync(agendamento);
        InstituicoesMock.Setup(r => r.ObterPorIdAsync(agendamento.IdInstituicao, Ct)).ReturnsAsync(instituicao);
        PraticasMock.Setup(r => r.ObterPorIdAsync((short)agendamento.IdPratica, Ct)).ReturnsAsync(pratica);
        CidadesMock.Setup(r => r.ObterPorIdAsync(agendamento.IdCidadePaciente, Ct)).ReturnsAsync(cidade);
        EstadosMock.Setup(r => r.ObterPorIdAsync((short)agendamento.IdEstadoPaciente, Ct)).ReturnsAsync(estado);

        var query = new ObterAtendimentosPorCpfQuery(EntityBuilders.CpfValido, "token123");

        var result = await _handler.Handle(query, Ct);

        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value.ToList();
        dtos.Should().HaveCount(2);
        dtos[0].DataAtendimento.Should().BeAfter(dtos[1].DataAtendimento);
        dtos[0].NomeInstituicao.Should().Be("Clínica A");
        dtos[0].NomePratica.Should().Be("Yoga");
    }
}
