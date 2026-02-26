using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class Instituicao : Entity<int>
{
    protected Instituicao() { }

    public Instituicao(
        int idInstituicao,
        string nome,
        string? descricao,
        int idCidade,
        string cnpj,
        string cep,
        string email,
        TimeSpan horarioInicioAtendimento,
        TimeSpan horarioFimAtendimento)
    {
        IdInstituicao = idInstituicao;
        Nome = nome;
        Descricao = descricao;
        IdCidade = idCidade;
        Cnpj = cnpj;
        Cep = cep;
        Email = email;
        HorarioInicioAtendimento = horarioInicioAtendimento;
        HorarioFimAtendimento = horarioFimAtendimento;
    }

    [Key]
    public int IdInstituicao { get; private set; }
    public string Nome { get; private set; } = null!;
    public string? Descricao { get; private set; }
    [ForeignKey(nameof(Cidade))]
    public int IdCidade { get; private set; }
    public string Cnpj { get; private set; } = null!;
    public string Cep { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public TimeSpan HorarioInicioAtendimento { get; private set; }
    public TimeSpan HorarioFimAtendimento { get; private set; }

    public virtual Cidade? Cidade { get; private set; }
    public virtual ICollection<PraticaInstituicao> PraticasInstituicoes { get; private set; } = new List<PraticaInstituicao>();
    public virtual ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();

    public void Atualizar(
        string nome,
        string? descricao,
        int idCidade,
        string cnpj,
        string cep,
        string email,
        TimeSpan horarioInicioAtendimento,
        TimeSpan horarioFimAtendimento)
    {
        Nome = nome;
        Descricao = descricao;
        IdCidade = idCidade;
        Cnpj = cnpj;
        Cep = cep;
        Email = email;
        HorarioInicioAtendimento = horarioInicioAtendimento;
        HorarioFimAtendimento = horarioFimAtendimento;
    }

    public static Instituicao Criar(
        string nome,
        string? descricao,
        int idCidade,
        string cnpj,
        string cep,
        string email,
        TimeSpan horarioInicioAtendimento,
        TimeSpan horarioFimAtendimento)
    {
        return new Instituicao
        {
            Nome = nome,
            Descricao = descricao,
            IdCidade = idCidade,
            Cnpj = cnpj,
            Cep = cep,
            Email = email,
            HorarioInicioAtendimento = horarioInicioAtendimento,
            HorarioFimAtendimento = horarioFimAtendimento
        };
    }
}
