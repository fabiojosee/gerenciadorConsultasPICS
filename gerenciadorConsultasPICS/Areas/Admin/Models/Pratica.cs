using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
    public class Pratica
    {
        protected Pratica() { }

        public Pratica(short idPratica, string nome, string? descricao)
        {
            this.idPratica = idPratica;
            this.nome = nome;
            this.descricao = descricao;
        }

        [Key]
        public short idPratica { get; private set; }
        public string nome { get; private set; }
        public string? descricao { get; private set; }

        public void Atualizar(string nome, string? descricao)
        {
            this.nome = nome;
            this.descricao = descricao;
        }

        public static class PraticaFactory
        {
            public static Pratica CriarPratica(string nome, string? descricao)
            {
                return new Pratica()
                {
                    nome = nome,
                    descricao = descricao
                };
            }
        }
    }
}
