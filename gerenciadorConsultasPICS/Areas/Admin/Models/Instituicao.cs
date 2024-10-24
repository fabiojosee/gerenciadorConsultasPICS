using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
    public class Instituicao
    {
        public Instituicao(int idInstituicao, string nome, string descricao, Int16 idEstado, int idCidade, string cnpj, string cep, string email)
        {
            this.idInstituicao = idInstituicao;
            this.nome = nome;
            this.descricao = descricao;
            this.idEstado = idEstado;
            this.idCidade = idCidade;
            this.cnpj = cnpj;
            this.cep = cep;
            this.email = email;
        }

        [Key]
        public int idInstituicao { get; private set; }
        public string nome { get; private set; }
        public string descricao { get; private set; }
        public Int16 idEstado { get; private set; }
        public int idCidade { get; private set; }
        public string cnpj { get; private set; }
        public string cep { get; private set; }
        public string email { get; private set; }
    }
}
