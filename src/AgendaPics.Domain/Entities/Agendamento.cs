using AgendaPics.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class Agendamento : Entity<int>
{
    protected Agendamento() { }

    public Agendamento(
        int idAgendamento,
        int idInstituicao,
        short idPratica,
        DateTime dataCriacao,
        byte status,
        string? observacao,
        string nomePaciente,
        string cpfPaciente,
        string telefonePaciente,
        DateTime dataNascimentoPaciente,
        byte generoPaciente,
        string emailPaciente,
        short idEstadoPaciente,
        int idCidadePaciente,
        byte grauAnsiedadePaciente)
    {
        IdAgendamento = idAgendamento;
        IdInstituicao = idInstituicao;
        IdPratica = idPratica;
        DataCriacao = dataCriacao;
        Status = status;
        Observacao = observacao;
        NomePaciente = nomePaciente;
        CpfPaciente = cpfPaciente;
        TelefonePaciente = telefonePaciente;
        DataNascimentoPaciente = dataNascimentoPaciente;
        GeneroPaciente = generoPaciente;
        EmailPaciente = emailPaciente;
        IdEstadoPaciente = idEstadoPaciente;
        IdCidadePaciente = idCidadePaciente;
        GrauAnsiedadePaciente = grauAnsiedadePaciente;
    }

    [Key]
    public int IdAgendamento { get; private set; }
    [ForeignKey(nameof(Instituicao))]
    public int IdInstituicao { get; private set; }
    [ForeignKey(nameof(Pratica))]
    public short IdPratica { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public byte Status { get; private set; }
    public string? Observacao { get; private set; }
    public string NomePaciente { get; private set; } = null!;
    public string CpfPaciente { get; private set; } = null!;
    public string TelefonePaciente { get; private set; } = null!;
    public DateTime DataNascimentoPaciente { get; private set; }
    public byte GeneroPaciente { get; private set; }
    public string EmailPaciente { get; private set; } = null!;
    [ForeignKey(nameof(Estado))]
    public short IdEstadoPaciente { get; private set; }
    [ForeignKey(nameof(Cidade))]
    public int IdCidadePaciente { get; private set; }
    public byte GrauAnsiedadePaciente { get; private set; }

    public virtual Instituicao? Instituicao { get; private set; }
    public virtual Pratica? Pratica { get; private set; }
    public virtual TermoConsentimento? TermoConsentimento { get; private set; }
    public virtual ICollection<Atendimento> Atendimentos { get; private set; } = new List<Atendimento>();

    public void AlterarStatus(byte status)
    {
        Status = status;
    }

    public void AlterarStatus(StatusAgendamento status)
    {
        Status = (byte)status;
    }

    public StatusAgendamento GetStatusEnum() => (StatusAgendamento)Status;
    public Genero GetGeneroEnum() => (Genero)GeneroPaciente;

    public static Agendamento Criar(
        int idInstituicao,
        short idPratica,
        string nomePaciente,
        string cpfPaciente,
        string telefonePaciente,
        DateTime dataNascimentoPaciente,
        byte generoPaciente,
        string emailPaciente,
        short idEstadoPaciente,
        int idCidadePaciente,
        byte grauAnsiedadePaciente,
        string? observacao = null)
    {
        return new Agendamento
        {
            IdInstituicao = idInstituicao,
            IdPratica = idPratica,
            DataCriacao = DateTime.Now,
            Status = (byte)StatusAgendamento.EmAndamento,
            NomePaciente = nomePaciente,
            CpfPaciente = cpfPaciente,
            TelefonePaciente = telefonePaciente,
            DataNascimentoPaciente = dataNascimentoPaciente,
            GeneroPaciente = generoPaciente,
            EmailPaciente = emailPaciente,
            IdEstadoPaciente = idEstadoPaciente,
            IdCidadePaciente = idCidadePaciente,
            GrauAnsiedadePaciente = grauAnsiedadePaciente,
            Observacao = observacao
        };
    }
}
