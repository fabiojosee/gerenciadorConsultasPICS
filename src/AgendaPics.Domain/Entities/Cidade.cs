using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class Cidade : Entity<int>
{
    protected Cidade() { }

    public Cidade(int idCidade, short idEstado, string nome)
    {
        IdCidade = idCidade;
        IdEstado = idEstado;
        Nome = nome;
    }

    [Key]
    public int IdCidade { get; private set; }
    [ForeignKey(nameof(Estado))]
    public short IdEstado { get; private set; }
    public string Nome { get; private set; } = null!;

    public virtual Estado? Estado { get; private set; }
}
