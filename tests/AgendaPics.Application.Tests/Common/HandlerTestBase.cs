using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Domain.Interfaces;
using Moq;

namespace AgendaPics.Application.Tests.Common;

public abstract class HandlerTestBase
{
    protected readonly Mock<IUnitOfWork> UnitOfWorkMock = new();
    protected readonly Mock<IAgendamentoRepository> AgendamentosMock = new();
    protected readonly Mock<IAtendimentoRepository> AtendimentosMock = new();
    protected readonly Mock<IAvaliacaoRepository> AvaliacoesMock = new();
    protected readonly Mock<ICidadeRepository> CidadesMock = new();
    protected readonly Mock<IEstadoRepository> EstadosMock = new();
    protected readonly Mock<IInstituicaoRepository> InstituicoesMock = new();
    protected readonly Mock<IPraticaRepository> PraticasMock = new();
    protected readonly Mock<IPraticaInstituicaoRepository> PraticasInstituicoesMock = new();
    protected readonly Mock<ITermoConsentimentoRepository> TermosConsentimentoMock = new();
    protected readonly Mock<IUsuarioRepository> UsuariosMock = new();
    protected readonly Mock<ITokenAcessoRepository> TokensAcessoMock = new();
    protected readonly Mock<IPasswordHasher> PasswordHasherMock = new();
    protected readonly Mock<IEmailService> EmailServiceMock = new();
    protected readonly Mock<ISecureRandomGenerator> RandomGeneratorMock = new();
    protected readonly Mock<IAccessTokenService> AccessTokenServiceMock = new();

    protected HandlerTestBase()
    {
        UnitOfWorkMock.Setup(u => u.Agendamentos).Returns(AgendamentosMock.Object);
        UnitOfWorkMock.Setup(u => u.Atendimentos).Returns(AtendimentosMock.Object);
        UnitOfWorkMock.Setup(u => u.Avaliacoes).Returns(AvaliacoesMock.Object);
        UnitOfWorkMock.Setup(u => u.Cidades).Returns(CidadesMock.Object);
        UnitOfWorkMock.Setup(u => u.Estados).Returns(EstadosMock.Object);
        UnitOfWorkMock.Setup(u => u.Instituicoes).Returns(InstituicoesMock.Object);
        UnitOfWorkMock.Setup(u => u.Praticas).Returns(PraticasMock.Object);
        UnitOfWorkMock.Setup(u => u.PraticasInstituicoes).Returns(PraticasInstituicoesMock.Object);
        UnitOfWorkMock.Setup(u => u.TermosConsentimento).Returns(TermosConsentimentoMock.Object);
        UnitOfWorkMock.Setup(u => u.Usuarios).Returns(UsuariosMock.Object);
        UnitOfWorkMock.Setup(u => u.TokensAcesso).Returns(TokensAcessoMock.Object);
        UnitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    protected static CancellationToken Ct => CancellationToken.None;
}
