using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
    public class Estado
    {
        public Estado(Int16 idEstado, string nome, string sigla)
        {
            this.idEstado = idEstado;
            this.nome = nome;
            this.sigla = sigla;
        }

        [Key]
        public Int16 idEstado { get; private set; }
        public string nome { get; private set; }
        public string sigla { get; private set; }
    }
}
