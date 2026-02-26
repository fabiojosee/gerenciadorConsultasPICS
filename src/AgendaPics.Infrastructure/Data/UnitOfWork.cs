using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Repositories;

namespace AgendaPics.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IAgendamentoRepository? _agendamentos;
    private IAtendimentoRepository? _atendimentos;
    private IAvaliacaoRepository? _avaliacoes;
    private ICidadeRepository? _cidades;
    private IEstadoRepository? _estados;
    private IInstituicaoRepository? _instituicoes;
    private IPraticaRepository? _praticas;
    private IPraticaInstituicaoRepository? _praticasInstituicoes;
    private ITermoConsentimentoRepository? _termosConsentimento;
    private IUsuarioRepository? _usuarios;
    private ITokenAcessoRepository? _tokensAcesso;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IAgendamentoRepository Agendamentos =>
        _agendamentos ??= new AgendamentoRepository(_context);

    public IAtendimentoRepository Atendimentos =>
        _atendimentos ??= new AtendimentoRepository(_context);

    public IAvaliacaoRepository Avaliacoes =>
        _avaliacoes ??= new AvaliacaoRepository(_context);

    public ICidadeRepository Cidades =>
        _cidades ??= new CidadeRepository(_context);

    public IEstadoRepository Estados =>
        _estados ??= new EstadoRepository(_context);

    public IInstituicaoRepository Instituicoes =>
        _instituicoes ??= new InstituicaoRepository(_context);

    public IPraticaRepository Praticas =>
        _praticas ??= new PraticaRepository(_context);

    public IPraticaInstituicaoRepository PraticasInstituicoes =>
        _praticasInstituicoes ??= new PraticaInstituicaoRepository(_context);

    public ITermoConsentimentoRepository TermosConsentimento =>
        _termosConsentimento ??= new TermoConsentimentoRepository(_context);

    public IUsuarioRepository Usuarios =>
        _usuarios ??= new UsuarioRepository(_context);

    public ITokenAcessoRepository TokensAcesso =>
        _tokensAcesso ??= new TokenAcessoRepository(_context);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
