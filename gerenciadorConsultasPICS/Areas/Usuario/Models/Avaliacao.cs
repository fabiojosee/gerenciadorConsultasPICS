using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Usuario.Models
{
    public class Avaliacao
    {
        protected Avaliacao() { }

        public Avaliacao(
            int idAvaliacao,
            int idAtendimento,
            DateTime data,
            string link,
            string observacao)
        {
            this.idAvaliacao = idAvaliacao;
            this.idAtendimento = idAtendimento;
            this.data = data;
            this.link = link;
            this.observacao = observacao;
        }

        [Key]
        public int idAvaliacao { get; private set; }
        public int idAtendimento { get; private set; }
        public DateTime data { get; private set; }
        public string link { get; private set; }
        public string observacao { get; private set; }

        public static class AvaliacaoFactory
        {
            public static Avaliacao CriarAvaliacao(
                int idAtendimento,
                DateTime data,
                string link,
                string observacao)
            {
                return new Avaliacao()
                {
                    idAtendimento = idAtendimento,
                    data = data,
                    link = link,
                    observacao = observacao
                };
            }
        }
    }
}
