using System.ComponentModel.DataAnnotations;

namespace AgendaPics.Domain.Entities;

public class TokenAcesso : Entity<int>
{
    protected TokenAcesso() { }

    public TokenAcesso(int id, string cpf, string token, DateTime dataExpiracao, bool utilizado)
    {
        IdTokenAcesso = id;
        Cpf = cpf;
        Token = token;
        DataExpiracao = dataExpiracao;
        Utilizado = utilizado;
    }

    [Key]
    public int IdTokenAcesso { get; private set; }
    public string Cpf { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime DataExpiracao { get; private set; }
    public bool Utilizado { get; private set; }

    public bool IsValido() => !Utilizado && DateTime.Now <= DataExpiracao;

    public void MarcarComoUtilizado()
    {
        Utilizado = true;
    }

    public static TokenAcesso Criar(string cpf, string token, TimeSpan validade)
    {
        return new TokenAcesso
        {
            Cpf = cpf,
            Token = token,
            DataExpiracao = DateTime.Now.Add(validade),
            Utilizado = false
        };
    }
}
