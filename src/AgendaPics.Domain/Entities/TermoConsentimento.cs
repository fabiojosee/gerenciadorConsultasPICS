using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class TermoConsentimento
{
    protected TermoConsentimento() { }

    public TermoConsentimento(int idAgendamento, DateTime dataConsentimento)
    {
        IdAgendamento = idAgendamento;
        DataConsentimento = dataConsentimento;
    }

    [Key]
    [ForeignKey(nameof(Agendamento))]
    public int IdAgendamento { get; private set; }
    public DateTime DataConsentimento { get; private set; }

    public virtual Agendamento? Agendamento { get; private set; }

    public static TermoConsentimento Criar(int idAgendamento, DateTime dataConsentimento)
    {
        return new TermoConsentimento
        {
            IdAgendamento = idAgendamento,
            DataConsentimento = dataConsentimento
        };
    }
}
