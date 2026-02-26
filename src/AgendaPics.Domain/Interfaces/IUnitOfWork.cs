namespace AgendaPics.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAgendamentoRepository Agendamentos { get; }
    IAtendimentoRepository Atendimentos { get; }
    IAvaliacaoRepository Avaliacoes { get; }
    ICidadeRepository Cidades { get; }
    IEstadoRepository Estados { get; }
    IInstituicaoRepository Instituicoes { get; }
    IPraticaRepository Praticas { get; }
    IPraticaInstituicaoRepository PraticasInstituicoes { get; }
    ITermoConsentimentoRepository TermosConsentimento { get; }
    IUsuarioRepository Usuarios { get; }
    ITokenAcessoRepository TokensAcesso { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
