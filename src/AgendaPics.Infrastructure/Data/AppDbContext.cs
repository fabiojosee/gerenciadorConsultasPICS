using AgendaPics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Instituicao> Instituicoes { get; set; }
    public DbSet<Pratica> Praticas { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }
    public DbSet<Atendimento> Atendimentos { get; set; }
    public DbSet<Cidade> Cidades { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<PraticaInstituicao> PraticasInstituicoes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Avaliacao> Avaliacoes { get; set; }
    public DbSet<TermoConsentimento> TermosConsentimento { get; set; }
    public DbSet<TokenAcesso> TokensAcesso { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PraticaInstituicao>()
            .HasKey(e => new { e.IdPratica, e.IdInstituicao });

        modelBuilder.Entity<Instituicao>().ToTable("Instituicao");
        modelBuilder.Entity<Pratica>().ToTable("Pratica");
        modelBuilder.Entity<Agendamento>().ToTable("Agendamento");
        modelBuilder.Entity<Atendimento>().ToTable("Atendimento");
        modelBuilder.Entity<Cidade>().ToTable("Cidade");
        modelBuilder.Entity<Estado>().ToTable("Estado");
        modelBuilder.Entity<PraticaInstituicao>().ToTable("PraticaInstituicao");
        modelBuilder.Entity<Usuario>().ToTable("Usuario");
        modelBuilder.Entity<Avaliacao>().ToTable("Avaliacao");
        modelBuilder.Entity<TermoConsentimento>().ToTable("TermoConsentimento");
        modelBuilder.Entity<TokenAcesso>().ToTable("TokenAcesso");

        ConfigureInstituicao(modelBuilder);
        ConfigurePratica(modelBuilder);
        ConfigureAgendamento(modelBuilder);
        ConfigureAtendimento(modelBuilder);
        ConfigureCidade(modelBuilder);
        ConfigureEstado(modelBuilder);
        ConfigurePraticaInstituicao(modelBuilder);
        ConfigureUsuario(modelBuilder);
        ConfigureAvaliacao(modelBuilder);
        ConfigureTermoConsentimento(modelBuilder);
        ConfigureTokenAcesso(modelBuilder);
    }

    private static void ConfigureInstituicao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instituicao>(entity =>
        {
            entity.HasKey(e => e.IdInstituicao);
            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
            entity.Property(e => e.IdCidade).HasColumnName("idCidade");
            entity.Property(e => e.Cnpj).HasColumnName("cnpj");
            entity.Property(e => e.Cep).HasColumnName("cep");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.HorarioInicioAtendimento).HasColumnName("horarioInicioAtendimento");
            entity.Property(e => e.HorarioFimAtendimento).HasColumnName("horarioFimAtendimento");
        });
    }

    private static void ConfigurePratica(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pratica>(entity =>
        {
            entity.HasKey(e => e.IdPratica);
            entity.Property(e => e.IdPratica).HasColumnName("idPratica");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
        });
    }

    private static void ConfigureAgendamento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.HasKey(e => e.IdAgendamento);
            entity.Property(e => e.IdAgendamento).HasColumnName("idAgendamento");
            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.IdPratica).HasColumnName("idPratica");
            entity.Property(e => e.DataCriacao).HasColumnName("dataCriacao");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Observacao).HasColumnName("observacao");
            entity.Property(e => e.NomePaciente).HasColumnName("nomePaciente");
            entity.Property(e => e.CpfPaciente).HasColumnName("cpfPaciente");
            entity.Property(e => e.TelefonePaciente).HasColumnName("telefonePaciente");
            entity.Property(e => e.DataNascimentoPaciente).HasColumnName("dataNascimentoPaciente");
            entity.Property(e => e.GeneroPaciente).HasColumnName("generoPaciente");
            entity.Property(e => e.EmailPaciente).HasColumnName("emailPaciente");
            entity.Property(e => e.IdEstadoPaciente).HasColumnName("idEstadoPaciente");
            entity.Property(e => e.IdCidadePaciente).HasColumnName("idCidadePaciente");
            entity.Property(e => e.GrauAnsiedadePaciente).HasColumnName("grauAnsiedadePaciente");
        });
    }

    private static void ConfigureAtendimento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atendimento>(entity =>
        {
            entity.HasKey(e => e.IdAtendimento);
            entity.Property(e => e.IdAtendimento).HasColumnName("idAtendimento");
            entity.Property(e => e.IdAgendamento).HasColumnName("idAgendamento");
            entity.Property(e => e.DataAtendimento).HasColumnName("dataAtendimento");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.QueixaPaciente).HasColumnName("queixaPaciente");
            entity.Property(e => e.Observacao).HasColumnName("observacao");
        });
    }

    private static void ConfigureCidade(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cidade>(entity =>
        {
            entity.HasKey(e => e.IdCidade);
            entity.Property(e => e.IdCidade).HasColumnName("idCidade");
            entity.Property(e => e.IdEstado).HasColumnName("idEstado");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.HasData(BrasilSeedData.ObterCidades());
        });
    }

    private static void ConfigureEstado(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IdEstado);
            entity.Property(e => e.IdEstado).HasColumnName("idEstado");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.Sigla).HasColumnName("sigla");
            entity.HasData(BrasilSeedData.ObterEstados());
        });
    }

    private static void ConfigurePraticaInstituicao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PraticaInstituicao>(entity =>
        {
            entity.Property(e => e.IdPratica).HasColumnName("idPratica");
            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.Periodicidade).HasColumnName("periodicidade");
            entity.Property(e => e.QtdSessoes).HasColumnName("qtdSessoes");
            entity.Property(e => e.DiaPermitidoParaAgendamento).HasColumnName("diaPermitidoParaAgendamento");
        });
    }

    private static void ConfigureUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");
            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Senha).HasColumnName("senha");
            entity.Property(e => e.FlPrimeiroAcesso).HasColumnName("flPrimeiroAcesso");
            entity.HasData(
                new Usuario(1, 1, null, "admin", "$2a$12$a0oXFr4B3AbjZk3sTh84Puq70PCBj9cf.iEIEm91aWvvnuLXh2Rfu", true)
            );
        });
    }

    private static void ConfigureAvaliacao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.HasKey(e => e.IdAvaliacao);
            entity.Property(e => e.IdAvaliacao).HasColumnName("idAvaliacao");
            entity.Property(e => e.IdAtendimento).HasColumnName("idAtendimento");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Link).HasColumnName("link");
            entity.Property(e => e.Observacao).HasColumnName("observacao");
        });
    }

    private static void ConfigureTermoConsentimento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TermoConsentimento>(entity =>
        {
            entity.HasKey(e => e.IdAgendamento);
            entity.Property(e => e.IdAgendamento).HasColumnName("idAgendamento");
            entity.Property(e => e.DataConsentimento).HasColumnName("dataConsentimento");
        });
    }

    private static void ConfigureTokenAcesso(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TokenAcesso>(entity =>
        {
            entity.HasKey(e => e.IdTokenAcesso);
            entity.Property(e => e.IdTokenAcesso).HasColumnName("idTokenAcesso");
            entity.Property(e => e.Cpf).HasColumnName("cpf");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.DataExpiracao).HasColumnName("dataExpiracao");
            entity.Property(e => e.Utilizado).HasColumnName("utilizado");
        });
    }
}
