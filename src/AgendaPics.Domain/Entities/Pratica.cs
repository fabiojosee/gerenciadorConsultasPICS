using System.ComponentModel.DataAnnotations;

namespace AgendaPics.Domain.Entities;

public class Pratica : Entity<short>
{
    protected Pratica() { }

    public Pratica(short idPratica, string nome, string? descricao)
    {
        IdPratica = idPratica;
        Nome = nome;
        Descricao = descricao;
    }

    [Key]
    public short IdPratica { get; private set; }
    public string Nome { get; private set; } = null!;
    public string? Descricao { get; private set; }

    public virtual ICollection<PraticaInstituicao> PraticasInstituicoes { get; private set; } = new List<PraticaInstituicao>();

    public void Atualizar(string nome, string? descricao)
    {
        Nome = nome;
        Descricao = descricao;
    }

    public static Pratica Criar(string nome, string? descricao)
    {
        return new Pratica
        {
            Nome = nome,
            Descricao = descricao
        };
    }
}
