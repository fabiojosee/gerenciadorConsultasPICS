using System.ComponentModel.DataAnnotations;

namespace AgendaPics.Domain.Entities;

public class Estado : Entity<short>
{
    protected Estado() { }

    public Estado(short idEstado, string nome, string sigla)
    {
        IdEstado = idEstado;
        Nome = nome;
        Sigla = sigla;
    }

    [Key]
    public short IdEstado { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Sigla { get; private set; } = null!;

    public virtual ICollection<Cidade> Cidades { get; private set; } = new List<Cidade>();
}
