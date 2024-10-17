using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
    public class Pratica
    {
        public Pratica(Int16 idPratica, string nome, string descricao)
        {
            this.idPratica = idPratica;
            this.nome = nome;
            this.descricao = descricao;
        }

        [Key]
        public Int16 idPratica { get; private set; }
        public string nome { get; private set; }
        public string descricao { get; private set; }
    }
}
