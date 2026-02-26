using AgendaPics.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class Atendimento : Entity<int>
{
    protected Atendimento() { }

    public Atendimento(
        int idAtendimento,
        int idAgendamento,
        DateTime dataAtendimento,
        byte status,
        string queixaPaciente,
        string? observacao)
    {
        IdAtendimento = idAtendimento;
        IdAgendamento = idAgendamento;
        DataAtendimento = dataAtendimento;
        Status = status;
        QueixaPaciente = queixaPaciente;
        Observacao = observacao;
    }

    [Key]
    public int IdAtendimento { get; private set; }
    [ForeignKey(nameof(Agendamento))]
    public int IdAgendamento { get; private set; }
    public DateTime DataAtendimento { get; private set; }
    public byte Status { get; private set; }
    public string QueixaPaciente { get; private set; } = null!;
    public string? Observacao { get; private set; }

    public virtual Agendamento? Agendamento { get; private set; }
    public virtual ICollection<Avaliacao> Avaliacoes { get; private set; } = new List<Avaliacao>();

    public void AlterarStatus(byte status)
    {
        Status = status;
    }

    public void AlterarStatus(StatusAtendimento status)
    {
        Status = (byte)status;
    }

    public void AdicionarObservacao(string? observacao)
    {
        Observacao = observacao;
    }

    public StatusAtendimento GetStatusEnum() => (StatusAtendimento)Status;

    public static Atendimento Criar(
        int idAgendamento,
        DateTime dataAtendimento,
        string queixaPaciente)
    {
        return new Atendimento
        {
            IdAgendamento = idAgendamento,
            DataAtendimento = dataAtendimento,
            Status = (byte)StatusAtendimento.Agendado,
            QueixaPaciente = queixaPaciente
        };
    }
}
