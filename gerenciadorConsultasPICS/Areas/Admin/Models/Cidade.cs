using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
    public class Cidade
    {
        public Cidade(int idCidade, Int16 idEstado, string nome)
        {
            this.idCidade = idCidade;
            this.idEstado = idEstado;
            this.nome = nome;
        }

        [Key]
        public int idCidade { get; private set; }
        public Int16 idEstado { get; private set; }
        public string nome { get; private set; }
    }
}
