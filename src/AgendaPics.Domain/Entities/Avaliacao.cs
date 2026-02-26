using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class Avaliacao : Entity<int>
{
    protected Avaliacao() { }

    public Avaliacao(
        int idAvaliacao,
        int idAtendimento,
        DateTime data,
        string link,
        string observacao)
    {
        IdAvaliacao = idAvaliacao;
        IdAtendimento = idAtendimento;
        Data = data;
        Link = link;
        Observacao = observacao;
    }

    [Key]
    public int IdAvaliacao { get; private set; }
    [ForeignKey(nameof(Atendimento))]
    public int IdAtendimento { get; private set; }
    public DateTime Data { get; private set; }
    public string Link { get; private set; } = null!;
    public string Observacao { get; private set; } = null!;

    public virtual Atendimento? Atendimento { get; private set; }

    public static Avaliacao Criar(
        int idAtendimento,
        DateTime data,
        string link,
        string observacao)
    {
        return new Avaliacao
        {
            IdAtendimento = idAtendimento,
            Data = data,
            Link = link,
            Observacao = observacao
        };
    }
}
